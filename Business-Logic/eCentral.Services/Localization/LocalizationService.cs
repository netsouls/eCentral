using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Localization;
using eCentral.Services.Events;
using eCentral.Services.Logging;

namespace eCentral.Services.Localization
{
    /// <summary>
    /// Provides information about localization
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Constants
        private const string LOCALSTRINGRESOURCES_ALL_KEY             = "eCentral.lsr.all-{0}";
        private const string LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY = "eCentral.lsr.{0}-{1}";
        private const string LOCALSTRINGRESOURCES_PATTERN_KEY         = "eCentral.lsr.";
        #endregion

        #region Fields

        private readonly IRepository<LocaleStringResource> lsrRepository;
        private readonly IWorkContext workContext;
        private readonly ILogger logger;
        private readonly ICacheManager cacheManager;
        private readonly LocalizationSettings localizationSettings;
        private readonly IEventPublisher eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="logger">Logger</param>
        /// <param name="this.workContext">Work context</param>
        /// <param name="this.lsrRepository">Locale string resource repository</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LocalizationService(ICacheManager cacheManager,
            ILogger logger, IWorkContext workContext,
            IRepository<LocaleStringResource> lsrRepository, LocalizationSettings localizationSettings, IEventPublisher eventPublisher)
        {
            this.cacheManager         = cacheManager;
            this.logger               = logger;
            this.workContext          = workContext;
            this.lsrRepository        = lsrRepository;
            this.localizationSettings = localizationSettings;
            this.eventPublisher       = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void DeleteLocaleStringResource(LocaleStringResource localeStringResource)
        {
            Guard.IsNotNull(localeStringResource, "localeStringResource");

            this.lsrRepository.Delete(localeStringResource);

            //cache
            this.cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(localeStringResource);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceById(Guid localeStringResourceId)
        {
            if (localeStringResourceId.IsEmpty())
                return null;

            var localeStringResource = this.lsrRepository.GetById(localeStringResourceId);

            return localeStringResource;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName)
        {
            if (this.workContext.WorkingLanguage != null)
                return GetLocaleStringResourceByName(resourceName, this.workContext.WorkingLanguage.RowId);

            return null;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName, Guid languageId,
            bool logIfNotFound = true)
        {
            LocaleStringResource localeStringResource = null;

            if (this.localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records

                // using an empty string so the request can still be logged
                if (string.IsNullOrEmpty(resourceName))
                    resourceName = string.Empty;
                resourceName = resourceName.Trim().ToLowerInvariant();

                var resources = GetAllResourcesByLanguageId(languageId);
                if (resources.ContainsKey(resourceName))
                {
                    var localeStringResourceId = resources[resourceName].RowId;
                    localeStringResource = this.lsrRepository.GetById(localeStringResourceId);
                }
            }
            else
            {
                //gradual loading
                var query = from lsr in this.lsrRepository.Table
                            orderby lsr.ResourceName
                            where lsr.LanguageId == languageId && lsr.ResourceName == resourceName
                            select lsr;
                localeStringResource = query.FirstOrDefault();
            }
            if (localeStringResource == null && logIfNotFound)
                this.logger.Warning(string.Format("Resource string ({0}) not found. Language ID = {1}", resourceName, languageId));
            return localeStringResource;
        }

        /// <summary>
        /// Gets json locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        public virtual Dictionary<string, LocaleStringResource> GetJsonResourcesByLanguageId(Guid languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId + "-IsJsonResource");
            return this.cacheManager.Get(key, () =>
                                              {
                                                  var query = from l in this.lsrRepository.Table
                                                              orderby l.ResourceName
                                                              where l.LanguageId == languageId && 
                                                                    l.IsJsonResource == true
                                                              select l;
                                                  var localeStringResourceDictionary =
                                                      query.ToDictionary(s => s.ResourceName.ToLowerInvariant());
                                                  return localeStringResourceDictionary;
                                              });
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        public virtual Dictionary<string, LocaleStringResource> GetAllResourcesByLanguageId(Guid languageId)
        {
            string key = string.Format(LOCALSTRINGRESOURCES_ALL_KEY, languageId);
            return this.cacheManager.Get(key, () =>
                                              {
                                                  var query = from l in this.lsrRepository.Table
                                                              orderby l.ResourceName
                                                              where l.LanguageId == languageId
                                                              select l;
                                                  var localeStringResourceDictionary =
                                                      query.ToDictionary(s => s.ResourceName.ToLowerInvariant());
                                                  return localeStringResourceDictionary;
                                              });
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void InsertLocaleStringResource(LocaleStringResource localeStringResource)
        {
            Guard.IsNotNull(localeStringResource, "localeStringResource");

            this.lsrRepository.Insert(localeStringResource);

            //cache
            cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityInserted(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            Guard.IsNotNull(localeStringResource, "localeStringResource");

            this.lsrRepository.Update(localeStringResource);

            //cache
            cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityUpdated(localeStringResource);
        }
        
        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey)
        {
            if (workContext.WorkingLanguage != null)
                return GetResource(resourceKey, workContext.WorkingLanguage.RowId);
            
            return "";
        }
        
        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        public virtual string GetResource(string resourceKey, Guid languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            string result = string.Empty;
            var resourceKeyValue = resourceKey;
            if (resourceKeyValue == null)
                resourceKeyValue = string.Empty;
            resourceKeyValue = resourceKeyValue.Trim().ToLowerInvariant();
            if (localizationSettings.LoadAllLocaleRecordsOnStartup)
            {
                //load all records
                var resources = GetAllResourcesByLanguageId(languageId);

                if (resources.ContainsKey(resourceKeyValue))
                {
                    var lsr = resources[resourceKeyValue];
                    if (lsr != null)
                        result = lsr.ResourceValue;
                }
            }
            else
            {
                //gradual loading
                string key = string.Format(LOCALSTRINGRESOURCES_BY_RESOURCENAME_KEY, languageId, resourceKeyValue);
                string lsr = cacheManager.Get(key, () =>
                {
                    var query = from l in this.lsrRepository.Table
                                where l.ResourceName == resourceKeyValue
                                && l.LanguageId == languageId
                                select l.ResourceValue;
                    return query.FirstOrDefault();
                });

                if (lsr != null) 
                    result = lsr;
            }
            if (String.IsNullOrEmpty(result))
            {
                if (logIfNotFound)
                    logger.Warning(string.Format("Resource string ({0}) is not found. Language ID = {1}", resourceKey, languageId));
                
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    result = defaultValue;
                }
                else
                {
                    if (!returnEmptyIfNotFound)
                        result = resourceKey;
                }
            }
            return result;
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            cacheManager.RemoveByPattern(LOCALSTRINGRESOURCES_PATTERN_KEY);
        }

        #endregion
    }
}
