using System;
using System.Text.RegularExpressions;
using System.Web;
using eCentral.Core.Domain;
using eCentral.Core.Infrastructure;

namespace eCentral.Web.Framework.HttpModules
{
    /// <summary>
    /// Removes or adds the www subdomain from all requests 
    /// and makes a permanent redirection to the new location.
    /// </summary>
    public class WwwSubDomainModule : HttpModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// Check www status
        /// </summary>
        private static Regex wwwStatusRegex = new Regex(@"https?://www\.", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly string noWWW = "{0}://";

        private static Regex noWwwStatusRegex = new Regex("https?://", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly string withWWW = "{0}://www.";

        #endregion

        #region Implemented Interfaces

        /// <summary>
        /// Determines whether the module will be registered for discovery
        /// in partial trust environments or not.
        /// </summary>

        protected override bool SupportDiscoverability
        {
            get { return true; }
        }

        #endregion

        #region IHttpModule

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpApplication"></see> 
        ///     that provides access to the methods, properties, and events common to 
        ///     all application objects within an ASP.NET application.
        /// </param>
        protected override void OnInit(HttpApplication context)
        {
            // bind to begin request event
            context.BeginRequest += OnBeginRequest;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        void OnBeginRequest(object sender, EventArgs e)
        {
            // This should be done at the beginning of each HTTP request as
            // early as possible.  It is being done here because in the
            // list of HTTP modules defined in the web.config file, this
            // module (WwwSubdomainModule) is the first defined one so will
            // fire before any other modules.
            var domainSettings = EngineContext.Current.Resolve<DomainSettings>();

            if (domainSettings.wwwStatus == WWWStatus.Ignore)
                return;

            var CurrentContext = ((HttpApplication)sender).Context;
            if (CurrentContext.Request.HttpMethod != "GET" ||
                CurrentContext.Request.IsLocal)
            {
                return;
            }

            if (CurrentContext.Request.PhysicalPath.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            {
                System.Uri currentURl = CurrentContext.Request.Url;

                bool wwwStatus = wwwStatusRegex.IsMatch(currentURl.ToString());

                if ((domainSettings.wwwStatus == WWWStatus.Remove) && wwwStatus)
                {
                    PermanentRedirect(wwwStatusRegex.Replace(currentURl.ToString(),
                        string.Format(noWWW, CurrentContext.Request.Url.Scheme)), CurrentContext);
                }
                else if ((domainSettings.wwwStatus == WWWStatus.Require) && !wwwStatus)
                {
                    PermanentRedirect(noWwwStatusRegex.Replace(currentURl.ToString(), string.Format(withWWW, CurrentContext.Request.Url.Scheme)), CurrentContext);
                }
            }
        }

        #endregion
    }
}