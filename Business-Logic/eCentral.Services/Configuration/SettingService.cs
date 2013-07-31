using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Configuration;
using eCentral.Core.Data;
using eCentral.Core.Domain.Logging;
using eCentral.Services.Logging;
using eCentral.Core.Domain.Configuration;
using eCentral.Core.Infrastructure;
using eCentral.Services.Events;

namespace eCentral.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Constants
        private const string SETTINGS_ALL_KEY = "eCentral.setting.all";
        #endregion

        #region Fields

        private readonly IRepository<Setting> settingRepository;
        private readonly IEventPublisher eventPublisher;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="settingRepository">Setting repository</param>
        public SettingService(ICacheManager cacheManager, IEventPublisher eventPublisher,
            IRepository<Setting> settingRepository)
        {
            this.cacheManager      = cacheManager;
            this.eventPublisher    = eventPublisher;
            this.settingRepository = settingRepository;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void Insert(Setting setting, bool clearCache = true)
        {
            Guard.IsNotNull(setting, "Setting");

            this.settingRepository.Insert(setting);

            //cache
            if (clearCache)
                this.cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            this.eventPublisher.EntityInserted(setting);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void Update(Setting setting, bool clearCache = true)
        {
            Guard.IsNotNull(setting, "setting");

            this.settingRepository.Update(setting);

            //cache
            if (clearCache)
                this.cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(setting);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>Setting</returns>
        public virtual Setting GetById(Guid settingId)
        {
            if (settingId.Equals(Guid.Empty))
                return null;

            var setting = this.settingRepository.GetById(settingId);
            return setting;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public virtual T GetByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAll();
            if (settings.ContainsKey(key)) {
                var setting = settings[key];
                return setting.As<T>();
            }
            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void Set<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            
            var settings = GetAll();
            
            Setting setting = null;
            string valueStr = CommonHelper.GetCustomTypeConverter(typeof(T)).ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                //update
                setting = settings[key];
                //little hack here because of EF issue
                setting           = GetById(setting.RowId);
                setting.Value     = valueStr;
                setting.UpdatedOn = DateTime.UtcNow;

                Update(setting, clearCache);
            }
            else
            {
                //insert
                setting = new Setting()
                              {
                                  Name = key,
                                  Value = valueStr,
                                  CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow
                              };
                Insert(setting, clearCache);
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void Delete(Setting setting)
        {
            Guard.IsNotNull(setting, "setting");

            this.settingRepository.Delete(setting);

            //cache
            this.cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(setting);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IDictionary<string, Setting> GetAll()
        {
            //cache
            string key = string.Format(SETTINGS_ALL_KEY);
            return this.cacheManager.Get(key, () =>
            {
                var query = from s in this.settingRepository.Table
                            orderby s.Name
                            select s;
                var settings = query.ToDictionary(s => s.Name.ToLowerInvariant());

                return settings;
            });
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        public virtual void Save<T>(T settingInstance) where T : ISettings, new()
        {
            //We should be sure that an appropriate ISettings object will not be cached in IoC tool after updating (by default cached per HTTP request)
            EngineContext.Current.Resolve<IConfigurationProvider<T>>().SaveSettings(settingInstance);

            // add audit history
            EngineContext.Current.Resolve<IUserActivityService>().
                InsertActivity(SystemActivityLogTypeNames.EditSettings, settingInstance.ToString(), typeof(T).Name);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            this.cacheManager.RemoveByPattern(SETTINGS_ALL_KEY);
        }
        #endregion
    }
}