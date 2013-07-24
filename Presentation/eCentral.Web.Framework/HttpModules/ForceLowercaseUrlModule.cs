using System;
using System.Text.RegularExpressions;
using System.Web;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain;
using eCentral.Core.Infrastructure;

namespace eCentral.Web.Framework.HttpModules
{
    /// <summary>
    /// Removes or adds the www subdomain from all requests 
    /// and makes a permanent redirection to the new location.
    /// </summary>
    public class ForceLowercaseUrlModule : HttpModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// Regular expression to match upper case string
        /// </summary>
        private readonly Regex upperChars = new Regex(@"[A-Z]", RegexOptions.Compiled);

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
        protected override void OnInit(HttpApplication application)
        {
            // check if the database is installed or not
            // if the database is not installed, we do not need to call 
            // the httpModule as they are dependent upon the configuration settings
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            // bind to begin request event
            application.BeginRequest += OnBeginRequest;
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
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();

            if (domainSettings.urlLowercaseStatus == UrlLowercaseStatus.Ignore)
                return;

            var CurrentContext = ((HttpApplication)sender).Context;
            var uri = new Uri(webHelper.GetThisPageUrl(false, false));

            // ignore the lower case enforcement for ajax requests 
            // to check whether the current request is an ajax request.
            if (CurrentContext.Request.HttpMethod == "POST" || CurrentContext.Request.IsAjaxRequest() ||
                uri.AbsolutePath.EndsWith(".axd") || uri.AbsolutePath.EndsWith(".ashx") ||
                //uri.AbsolutePath.Contains("/Web-Admin") ||
                webHelper.IsStaticResource(CurrentContext.Request))
            {
                return;
            }

            if (upperChars.IsMatch(uri.ToString()))
            {
                // redirect the user
                var idealUrl = uri.ToString();
                if (idealUrl.EndsWith("/") && uri.AbsolutePath.LastIndexOf('/') > 0)
                {
                    idealUrl = idealUrl.Substring(0, idealUrl.LastIndexOf('/'));
                }

                // convert to lower case - we are converting to lower case before, so that the query string does not get converted.
                idealUrl = idealUrl.ToLowerInvariant();

                if (!string.IsNullOrEmpty(uri.Query))
                {
                    idealUrl += uri.Query;
                }

                if (uri.AbsoluteUri == idealUrl || uri.AbsoluteUri == idealUrl + "/")
                    return;

                PermanentRedirect(idealUrl, CurrentContext);
            }
        }

        #endregion
    }
}