using System.IO;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Cms;
using eCentral.Services.Events;

namespace eCentral.Services.Cms
{
    /// <summary>
    /// Content service
    /// </summary>
    public partial class ContentService : IContentService
    {
        #region Constants

        private const string CONTENT_BY_ID_KEY   = "eCentral.content.id-{0}.culture-{1}";
        private const string CONTENT_PATTERN_KEY = "eCentral.content.";

        #endregion

        #region Fields

        private readonly ICacheManager cacheManager;
        private readonly IEventPublisher eventPublisher;
        private readonly ContentSettings contentSettings;
        private readonly IWebHelper webHelper;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        public ContentService(ICacheManager cacheManager,
            IEventPublisher eventPublisher, IWebHelper webHelper, ContentSettings contentSettings)
        {
            this.contentSettings  = contentSettings;
            this.cacheManager     = cacheManager;
            this.eventPublisher   = eventPublisher;
            this.webHelper        = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load content provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Html Content</returns>
        public virtual string LoadBySystemName(string systemName, string languageCulture)
        {
            Guard.IsNotNull(systemName, "systemname");
            Guard.IsNotNull(languageCulture, "languageculture");

            string key = string.Format(CONTENT_BY_ID_KEY, systemName, languageCulture);
            return this.cacheManager.Get(key, () =>
            {
                // read the html file and return the contents
                var filePath = Path.Combine(webHelper.MapPath(contentSettings.FileStorageVirtualPath),
                    Path.Combine(languageCulture, string.Format("{0}.html", systemName)));

                if (File.Exists(filePath))
                    return File.ReadAllText(filePath);

                return null;
            });
        }

        #endregion
    }
}
