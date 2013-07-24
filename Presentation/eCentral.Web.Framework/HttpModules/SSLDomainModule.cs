using System;
using System.Web;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain;
using eCentral.Core.Infrastructure;

namespace eCentral.Web.Framework.HttpModules
{
    /// <summary>
    /// Removes or adds the ssl from all requests 
    /// and makes a permanent redirection to the new location.
    /// </summary>    
    public class SSLDomainModule : HttpModuleBase
    {
        #region Constants and Fields

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

            var CurrentContext = ((HttpApplication)sender).Context;
            if (domainSettings.sslStatus == SSLStatus.Ignore ||
                CurrentContext.Request.IsLocal)
                return;

            // ignore the lower case enforcement for ajax requests 
            // to check whether the current request is an ajax request.
            if (CurrentContext.Request.HttpMethod != "GET")
            {
                return;
            }

            if (!webHelper.IsCurrentConnectionSecured())
            {
                string xredir__, xqstr__;

                xredir__ = "https://" + CurrentContext.Request.ServerVariables["SERVER_NAME"];
                xredir__ += CurrentContext.Request.ServerVariables["SCRIPT_NAME"];
                xqstr__ = CurrentContext.Request.ServerVariables["QUERY_STRING"];

                if (xqstr__ != "")
                    xredir__ = xredir__ + "?" + xqstr__;

                PermanentRedirect(xredir__, CurrentContext);
            }
        }

        #endregion
    }
}