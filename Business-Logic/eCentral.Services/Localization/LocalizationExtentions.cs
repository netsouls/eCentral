using System;
using System.Linq.Expressions;
using System.Reflection;
using eCentral.Core;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Infrastructure;
using eCentral.Core.Plugins;

namespace eCentral.Services.Localization
{
    public static class LocalizationExtentions
    {
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector)
            where T : BaseEntity, ILocalizedEntity
        {
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            return GetLocalized(entity, keySelector, workContext.WorkingLanguage.RowId);
        }
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static string GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector, Guid languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true) 
            where T : BaseEntity, ILocalizedEntity
        {
            return GetLocalized<T, string>(entity, keySelector, languageId, returnDefaultValue, ensureTwoPublishedLanguages);
        }
        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>Localized property</returns>
        public static TPropType GetLocalized<T, TPropType>(this T entity,
            Expression<Func<T, TPropType>> keySelector, Guid languageId, 
            bool returnDefaultValue = true, bool ensureTwoPublishedLanguages = true)
            where T : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            var member = keySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    keySelector));
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException(string.Format(
                       "Expression '{0}' refers to a field, not a property.",
                       keySelector));
            }

            TPropType result = default(TPropType);
            string resultStr = string.Empty;

            //load localized value
            string localeKeyGroup = typeof(T).Name;
            string localeKey = propInfo.Name;

            if (!languageId.IsEmpty())
            {
                //ensure that we have at least two published languages
                bool loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var lService = EngineContext.Current.Resolve<ILanguageService>();
                    var totalPublishedLanguages = lService.GetAll(false).Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    var leService = EngineContext.Current.Resolve<ILocalizedEntityService>();
                    //old approach (commented)
                    //var props = leService.GetLocalizedProperties(entity.Id, localeKeyGroup);
                    //var prop = props.FirstOrDefault(lp => lp.LanguageId == languageId &&
                    //    lp.LocaleKeyGroup.Equals(localeKeyGroup, StringComparison.InvariantCultureIgnoreCase) && //should be culture invariant
                    //    lp.LocaleKey.Equals(localeKey, StringComparison.InvariantCultureIgnoreCase));//should be culture invariant
                    //if (prop != null)
                    //{
                    //    resultStr = prop.LocaleValue;
                    //    result = CommonHelper.To<TPropType>(resultStr);
                    //}

                    //new approach
                    resultStr = leService.GetLocalizedValue(languageId, entity.RowId, localeKeyGroup, localeKey);
                    if (!String.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (String.IsNullOrEmpty(resultStr) && returnDefaultValue)
            {
                var localizer = keySelector.Compile();
                result = localizer(entity);
            }
            
            return result;
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="workContext">Work context</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, IWorkContext workContext)
            where T : struct
        {
            if (workContext == null)
                throw new ArgumentNullException("workContext");

            return GetLocalizedEnum<T>(enumValue, localizationService, workContext.WorkingLanguage.RowId);
        }
        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized value</returns>
        public static string GetLocalizedEnum<T>(this T enumValue, ILocalizationService localizationService, Guid languageId)
            where T : struct
        {
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");

            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            //localized value
            string resourceName = string.Format("Enums.{0}.{1}", 
                typeof(T).ToString(), 
                //Convert.ToInt32(enumValue)
                enumValue.ToString());
            string result = localizationService.GetResource(resourceName, languageId, false, "", true);

            //set default value if required
            if (String.IsNullOrEmpty(result))
                result = CommonHelper.ConvertEnum(enumValue.ToString());

            return result;
        }

        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="resourceName">Resource name</param>
        public static void DeletePluginLocaleResource(this BasePlugin plugin,
            string resourceName)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
            DeletePluginLocaleResource(plugin, localizationService,
                languageService, resourceName);
        }
        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="resourceName">Resource name</param>
        public static void DeletePluginLocaleResource(this BasePlugin plugin,
            ILocalizationService localizationService, ILanguageService languageService,
            string resourceName)
        {
            //actually plugin instance is not required
            if (plugin == null)
                throw new ArgumentNullException("plugin");
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");

            foreach (var lang in languageService.GetAll(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.RowId, false);
                if (lsr != null)
                    localizationService.DeleteLocaleStringResource(lsr);
            }
        }
        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        public static void AddOrUpdatePluginLocaleResource(this BasePlugin plugin,
            string resourceName, string resourceValue)
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var languageService = EngineContext.Current.Resolve<ILanguageService>();
             AddOrUpdatePluginLocaleResource(plugin, localizationService,
                 languageService, resourceName, resourceValue);
        }
        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="plugin">Plugin</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="languageService">Language service</param>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        [System.Diagnostics.DebuggerStepThrough]
        public static void AddOrUpdatePluginLocaleResource(this BasePlugin plugin, 
            ILocalizationService localizationService, ILanguageService languageService, 
            string resourceName, string resourceValue)
        {
            //actually plugin instance is not required
            if (plugin == null)
                throw new ArgumentNullException("plugin");
            if (localizationService == null)
                throw new ArgumentNullException("localizationService");
            if (languageService == null)
                throw new ArgumentNullException("languageService");
            
            foreach (var lang in languageService.GetAll(true))
            {
                var lsr = localizationService.GetLocaleStringResourceByName(resourceName, lang.RowId, false);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource()
                    {
                        LanguageId = lang.RowId,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue, 
                        UpdatedOn = DateTime.UtcNow, 
                        CreatedOn = DateTime.UtcNow
                    };
                    localizationService.InsertLocaleStringResource(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    lsr.UpdatedOn = DateTime.UtcNow;
                    localizationService.UpdateLocaleStringResource(lsr);
                }
            }
        }
    }
}
