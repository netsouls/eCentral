using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Localization;
using eCentral.Services.Configuration;
using eCentral.Services.Events;
using eCentral.Services.Users;

namespace eCentral.Services.Localization
{
    /// <summary>
    /// Language service
    /// </summary>
    public partial class LanguageService : ILanguageService
    {
        #region Constants
        private const string LANGUAGES_ALL_KEY     = "eCentral.language.all-{0}";
        private const string LANGUAGES_BY_ID_KEY   = "eCentral.language.id-{0}";
        private const string LANGUAGES_PATTERN_KEY = "eCentral.language.";
        #endregion

        #region Fields

        private readonly IRepository<Language> languageRepository;
        private readonly IUserService userService;
        private readonly ICacheManager cacheManager;
        private readonly ISettingService settingService;
        private readonly LocalizationSettings localizationSettings;
        private readonly IEventPublisher eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="languageRepository">Language repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="settingService">Setting service</param>
        /// <param name="localizationSettings">Localization settings</param>
        /// <param name="eventPublisher">Event published</param>
        public LanguageService(ICacheManager cacheManager,
            IRepository<Language> languageRepository,
            IUserService userService,
            ISettingService settingService,
            LocalizationSettings localizationSettings,
            IEventPublisher eventPublisher)
        {
            this.cacheManager         = cacheManager;
            this.languageRepository   = languageRepository;
            this.userService          = userService;
            this.settingService       = settingService;
            this.localizationSettings = localizationSettings;
            this.eventPublisher       = eventPublisher;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Deletes a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void Delete(Language language)
        {
            Guard.IsNotNull(language, "language");
            
            //update default admin area language (if required)
            if (localizationSettings.DefaultLanguageId == language.RowId)
            {
                foreach (var activeLanguage in GetAll())
                {
                    if (activeLanguage.RowId != language.RowId)
                    {
                        localizationSettings.DefaultLanguageId = activeLanguage.RowId;
                        settingService.Save(localizationSettings);
                        break;
                    }
                }
            }
            
            //update appropriate users (their language)
            //it can take a lot of time if you have thousands of associated customers
            var users = userService.GetUsersByLanguageId(language.RowId);
            foreach (var user in users)
            {
                user.LanguageId = null;
                userService.Update(user);
            }

            languageRepository.Delete(language);

            //cache
            cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityDeleted(language);
        }

        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Language collection</returns>
        public virtual IList<Language> GetAll(bool showHidden = false)
        {
            string key = string.Format(LANGUAGES_ALL_KEY, showHidden);
            return cacheManager.Get(key, () =>
            {
                var query = from l in languageRepository.Table
                            orderby l.DisplayOrder
                            where showHidden || l.Published
                            select l;
                var languages = query.ToList();
                return languages;
            });
        }

        /// <summary>
        /// Gets a language
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Language</returns>
        public virtual Language GetById(Guid languageId)
        {
            if (languageId.IsEmpty())
                return null;

            string key = string.Format(LANGUAGES_BY_ID_KEY, languageId);
            return cacheManager.Get(key, () =>
                                              {
                                                  return languageRepository.GetById(languageId);
                                              });
        }

        /// <summary>
        /// Inserts a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void Insert(Language language)
        {
            Guard.IsNotNull(language, "language");

            languageRepository.Insert(language);

            //cache
            cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityInserted(language);
        }

        /// <summary>
        /// Updates a language
        /// </summary>
        /// <param name="language">Language</param>
        public virtual void Update(Language language)
        {
            Guard.IsNotNull(language, "language");
            
            //update language
            languageRepository.Update(language);

            //cache
            cacheManager.RemoveByPattern(LANGUAGES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityUpdated(language);
        }

        #endregion
    }
}
