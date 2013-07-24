using System;
using System.IO;
using System.Web;
using eCentral.Core;
using eCentral.Core.Infrastructure;
using eCentral.Services.Helpers;
using eCentral.Services.Web;

namespace eCentral.Web.Framework.HttpHandlers
{
    /// <summary>
    /// Renders the javascript files, minify, compress and cache
    /// </summary>    
    public class JsHttpHandler : HttpHandlerBase
    {
        #region Constants and Fields

        private static string defaultPath = EngineContext.Current.Resolve<IVirtualPathProvider>().ToAbsolute("~/js.axd");
        private static string idParameterName = "id";

        private readonly IHttpResponseCacher _httpResponseCacher;
        private readonly IHttpResponseCompressor _httpResponseCompressor;
        private readonly IVirtualPathProvider _virtualPathProvider;

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

        /// <summary>
        /// Gets or sets the name of the id parameter.
        /// </summary>
        /// <value>The name of the id parameter.</value>
        public static string IdParameterName
        {
            get
            {
                return idParameterName;
            }

            set
            {
                Guard.IsNotNullOrEmpty(value, "value");

                idParameterName = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="WebAssetHttpHandler"/> class.
        /// </summary>
        /// <param name="assetRegistry">The asset registry.</param>
        /// <param name="httpResponseCompressor">The HTTP response compressor.</param>
        /// <param name="httpResponseCacher">The HTTP response cacher.</param>
        public JsHttpHandler(IHttpResponseCompressor httpResponseCompressor, IHttpResponseCacher httpResponseCacher,
            IVirtualPathProvider virtualPathProvider)
        {
            _httpResponseCompressor = httpResponseCompressor;
            _httpResponseCacher = httpResponseCacher;
            _virtualPathProvider = virtualPathProvider;
        }

        public JsHttpHandler() :
            this(EngineContext.Current.Resolve<IHttpResponseCompressor>(),
            EngineContext.Current.Resolve<IHttpResponseCacher>(), EngineContext.Current.Resolve<IVirtualPathProvider>())
        {
        }

        #region Methods

        /// <summary>
        /// Enables a JsHttpHandler object to process of requests.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ProcessRequest(HttpContextBase context)
        {
            string id = context.Request.QueryString[IdParameterName];

            if (!string.IsNullOrEmpty(id))
            {
                HttpResponseBase response = context.Response;

                // set the content type 
                response.ContentType = "text/javascript";

                var hashValue = string.Format(".js?{0}", SiteVersion.CurrentVersionHashValue);

                id = id.Replace("_", "/").Replace(hashValue, ".js");

                // get the correct file and path name
                id = _virtualPathProvider.CombinePaths("~/library/js/", id);

                string content = _virtualPathProvider.ReadAllText(id);

                if (!string.IsNullOrEmpty(content))
                {
                    _httpResponseCompressor.Compress(context);

                    // Write
                    using (StreamWriter sw = new StreamWriter((response.OutputStream)))
                    {
                        sw.Write(content);
                    }

                    // cache the contents
                    _httpResponseCacher.Cache(context, TimeSpan.FromDays(7));
                }
            }
        }

        #endregion

        #region Utilities

        #endregion
    }
}