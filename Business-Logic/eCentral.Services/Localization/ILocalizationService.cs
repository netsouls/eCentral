using System;
using System.Collections.Generic;
using eCentral.Core.Domain.Localization;

namespace eCentral.Services.Localization
{
    /// <summary>
    /// Localization manager interface
    /// </summary>
    public partial interface ILocalizationService
    {
        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        void DeleteLocaleStringResource(LocaleStringResource localeStringResource);

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>Locale string resource</returns>
        LocaleStringResource GetLocaleStringResourceById(Guid localeStringResourceId);

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <returns>Locale string resource</returns>
        LocaleStringResource GetLocaleStringResourceByName(string resourceName);

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <returns>Locale string resource</returns>
        LocaleStringResource GetLocaleStringResourceByName(string resourceName, Guid languageId,
            bool logIfNotFound = true);

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        Dictionary<string, LocaleStringResource> GetAllResourcesByLanguageId(Guid languageId);

        /// <summary>
        /// Gets JSON locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Locale string resource collection</returns>
        Dictionary<string, LocaleStringResource> GetJsonResourcesByLanguageId(Guid languageId);

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        void InsertLocaleStringResource(LocaleStringResource localeStringResource);

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        void UpdateLocaleStringResource(LocaleStringResource localeStringResource);

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>A string representing the requested resource string.</returns>
        string GetResource(string resourceKey);

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="logIfNotFound">A value indicating whether to log error if locale string resource is not found</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether to empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>A string representing the requested resource string.</returns>
        string GetResource(string resourceKey, Guid languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false);

        /// <summary>
        /// Clear cache
        /// </summary>
        void ClearCache();
    }
}
