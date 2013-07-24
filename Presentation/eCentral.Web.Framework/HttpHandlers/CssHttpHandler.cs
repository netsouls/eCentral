using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using eCentral.Core;
using eCentral.Core.Infrastructure;
using eCentral.Services.Helpers;
using eCentral.Services.Web;

namespace eCentral.Web.Framework.HttpHandlers
{
    /// <summary>
    /// Renders the stylesheet files, minify, compress and cache
    /// </summary>    
    public class CssHttpHandler : HttpHandlerBase
    {
        #region Constants and Fields

        private static string defaultPath = EngineContext.Current.Resolve<IVirtualPathProvider>().ToAbsolute("~/css.axd");
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
        public CssHttpHandler(IHttpResponseCompressor httpResponseCompressor, IHttpResponseCacher httpResponseCacher,
            IVirtualPathProvider virtualPathProvider)
        {
            _httpResponseCompressor = httpResponseCompressor;
            _httpResponseCacher = httpResponseCacher;
            _virtualPathProvider = virtualPathProvider;
        }

        public CssHttpHandler() :
            this(EngineContext.Current.Resolve<IHttpResponseCompressor>(),
            EngineContext.Current.Resolve<IHttpResponseCacher>(), EngineContext.Current.Resolve<IVirtualPathProvider>())
        {
        }

        #region Methods

        /// <summary>
        /// Enables a CssHttpHandler object to process of requests.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void ProcessRequest(HttpContextBase context)
        {
            string id = context.Request.QueryString[IdParameterName];

            if (!string.IsNullOrEmpty(id))
            {
                HttpResponseBase response = context.Response;

                // set the content type 
                response.ContentType = "text/css";

                var hashValue = string.Format(".css?{0}", SiteVersion.CurrentVersionHashValue);

                id = id.Replace("_", "/").Replace(hashValue, ".css");

                // get the correct file and path name
                id = _virtualPathProvider.CombinePaths("~/library/css/", id);

                string content = _virtualPathProvider.ReadAllText(id);

                if (!string.IsNullOrEmpty(content))
                {
                    // check for @import url - if the style sheet is import other style sheets, 
                    // get those stylesheets and then render
                    content = ImportCss(content);

                    var relativePath = Path.Combine(EngineContext.Current.Resolve<IWebHelper>().RelativeWebRoot, "library/images");

                    // replace image paths
                    var relativePaths = FindDistinctRelativePathsIn(content);
                    relativePaths.ForEach(data =>
                    {
                        content = ReplaceRelativePathsIn(content, data, relativePath);
                    });

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

        private string ImportCss ( string css )
        {
            // get all the import matches 
            var matches = Regex.Matches(css, @"import url\([""']{0,1}(.+?)[""']{0,1}\)", RegexOptions.IgnoreCase);
            var content = string.Empty;

            if (matches.Count > 0)
            {
                // iterate through the matches and retrieve the css files
                foreach (Match match in matches)
                {        
                    // get the correct file and path name
                    var fileName = _virtualPathProvider.CombinePaths("~/library/css/", match.Groups[1].Value);

                    content += _virtualPathProvider.ReadAllText(fileName);
                }

                return content;
            }

            return css;
        }

        private string ReplaceRelativePathsIn(string css, string oldPath, string newPath)
        {
            var regex = new Regex(@"url\([""']{0,1}" + Regex.Escape(oldPath) + @"[""']{0,1}\)", RegexOptions.IgnoreCase);

            return regex.Replace(css, match =>
            {
                var path = match.Value.Replace(oldPath, string.Format("{0}/{1}?{2}", newPath, oldPath, SiteVersion.CurrentVersionHashValue));
                return path;
            });
        }

        private IEnumerable<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = Regex.Matches(css, @"url\([""']{0,1}(.+?)[""']{0,1}\)", RegexOptions.IgnoreCase);
            var matchesHash = new HashSet<string>();
            foreach (Match match in matches)
            {
                var path = match.Groups[1].Captures[0].Value;
                if (!path.StartsWith("/") && !path.StartsWith("http://") && !path.StartsWith("https://") && !path.StartsWith("data:") && !path.StartsWith("squishit://"))
                {
                    if (matchesHash.Add(path))
                    {
                        yield return path;
                    }
                }
            }
        }
    }
}