using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Infrastructure;
using eCentral.Services.Helpers;
using eCentral.Services.Localization;
using eCentral.Services.Web;
using ServiceStack.Text;

namespace eCentral.Web.Framework.HttpHandlers
{
    /// <summary>
    /// Renders the javascript language file, minify, compress and cache
    /// </summary>    
    public class i18nHttpHandler : HttpHandlerBase
    {
        #region Constants and Fields

        private static string defaultPath = EngineContext.Current.Resolve<IVirtualPathProvider>().ToAbsolute("~/i18n.axd?");

        private readonly IHttpResponseCacher _httpResponseCacher;
        private readonly IHttpResponseCompressor _httpResponseCompressor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the default path of the asset.
        /// </summary>
        /// <value>The default path.</value>
        public static string DefaultPath
        {
            get
            {
                return defaultPath;
            }
            set
            {
                Guard.IsNotNullOrEmpty(value, "value");
                defaultPath = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetHttpHandler"/> class.
        /// </summary>
        /// <param name="assetRegistry">The asset registry.</param>
        /// <param name="httpResponseCompressor">The HTTP response compressor.</param>
        /// <param name="httpResponseCacher">The HTTP response cacher.</param>
        public i18nHttpHandler(IHttpResponseCompressor httpResponseCompressor, IHttpResponseCacher httpResponseCacher)
        {
            _httpResponseCompressor = httpResponseCompressor;
            _httpResponseCacher     = httpResponseCacher;
        }

        public i18nHttpHandler() :
            this(EngineContext.Current.Resolve<IHttpResponseCompressor>(),
            EngineContext.Current.Resolve<IHttpResponseCacher>())
        {
        }

        #region Methods

        /// <summary>
        /// Enables a JsHttpHandler object to process of requests.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ProcessRequest(HttpContextBase context)
        {
            var languageId = EngineContext.Current.Resolve<IWorkContext>().WorkingLanguage.RowId;
            
            if (!languageId.IsEmpty())
            {
                HttpResponseBase response = context.Response;

                // set hte json object
                JsonCulture jsonCulture = new JsonCulture(languageId);

                // set the content type 
                response.ContentType = "text/javascript";

                // create the app engine i18n script
                string jsScript = "$.extend(appEngine, {i18n: " + jsonCulture.ToJsonString() + "});";

                var urlHelper   = new UrlHelper(context.RequestContext());
                JsonUrl jsonUrl = new JsonUrl(urlHelper);

                // implement the urls
                jsScript += "$.extend(appEngine, {path: " +  jsonUrl.ToJsonString() + "});";

                if (!string.IsNullOrEmpty(jsScript))
                {
                    _httpResponseCompressor.Compress(context);

                    // Write
                    using (StreamWriter sw = new StreamWriter((response.OutputStream)))
                    {
                        sw.Write(jsScript);
                    }

                    // cache the contents
                    _httpResponseCacher.Cache(context, TimeSpan.FromDays(7));
                }
            }
        }

        #endregion

        #region Utils 

        /// <summary>
        /// Represents the Url paths used by path.js
        /// </summary>
        internal sealed class JsonUrl
        {
            #region "Fields"

            private readonly Dictionary<string, string> urlDict = new Dictionary<string, string>();

            #endregion

            /// <summary>
            /// Creates a new JsonUrk instance.
            /// </summary>
            /// <remarks>
            /// 
            /// This class uses a dictionary as its basis for storing/caching its information. This makes it incredibly easy to extend
            /// without having to create/remove properties.
            /// 
            /// </remarks>
            public JsonUrl(UrlHelper helper)
            {
                Guard.IsNotNull(helper, "helper");

                var webHelp = EngineContext.Current.Resolve<IWebHelper>();
                // add the urls
                this.AddUrl("root", webHelp.RelativeWebRoot);
                this.AddUrl("image", helper.ImageUrl("{0}"));
            }

            #region Methods

            /// <summary>
            /// Adds a new translatable string resource to this JsonCulture.
            /// </summary>
            /// <param name="scriptKey">The key used to retrieve this value from clientside script.</param>
            /// <param name="resourceLabelKey">The key used to retrieve the translated value from global resource labels.</param>
            /// <returns>The translated string.</returns>
            private void AddUrl(string keyName, string keyValue)
            {
                this.urlDict.Add(keyName, keyValue);
            }

            /// <summary>
            /// Returns a JSON formatted string repressentation of this JsonCulture instance's culture labels.
            /// </summary>
            /// <returns></returns>
            public string ToJsonString()
            {
                return JsonSerializer.SerializeToString<Dictionary<string, string>>(this.urlDict);
            }

            #endregion
        }

        /// <summary>
        /// Represents the i18n culture used by i18n.js 
        /// </summary>
        internal sealed class JsonCulture
        {
            #region "Fields"

            private readonly Dictionary<string, string> translationDict = new Dictionary<string, string>();

            private ILocalizationService localizationService;
            
            #endregion

            /// <summary>
            /// Creates a new JsonCulture instance from the supplied language id.
            /// </summary>
            /// <remarks>
            /// 
            /// This class uses a dictionary as its basis for storing/caching its information. This makes it incredibly easy to extend
            /// without having to create/remove properties.
            /// 
            /// </remarks>
            public JsonCulture(Guid languageId)
            {
                Guard.IsNotNull(languageId, "languageId");

                this.localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                // get all the resources by language id
                var resources = localizationService.GetJsonResourcesByLanguageId(languageId);

                // iterate and add all resources
                resources.ForEach(action =>
                {
                    this.AddResource(action.Value);
                });
            }

            #region Methods 

            /// <summary>
            /// Adds a new translatable string resource to this JsonCulture.
            /// </summary>
            /// <param name="scriptKey">The key used to retrieve this value from clientside script.</param>
            /// <param name="resourceLabelKey">The key used to retrieve the translated value from global resource labels.</param>
            /// <returns>The translated string.</returns>
            private void AddResource(LocaleStringResource resource)
            {
                var resourceKeyName = resource.ResourceName;
                var resourceKeyValue = resourceKeyName;
                if (!string.IsNullOrEmpty(resource.ResourceValue))
                    resourceKeyValue = resource.ResourceValue;

                resourceKeyName = resourceKeyName.Replace(".", "_"); // hack to support javascript
                this.translationDict.Add(resourceKeyName, resourceKeyValue);
            }

            /// <summary>
            /// Returns a JSON formatted string repressentation of this JsonCulture instance's culture labels.
            /// </summary>
            /// <returns></returns>
            public string ToJsonString()
            {
                return JsonSerializer.SerializeToString<Dictionary<string, string>>(this.translationDict);
            }

            #endregion

        }

        #endregion
    }
}