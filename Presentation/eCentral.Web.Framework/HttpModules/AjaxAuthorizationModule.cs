using System;
using System.Web;
using System.Web.Mvc;

namespace eCentral.Web.Framework.HttpModules
{
    /// <summary>
    /// Checks for user authorization for an ajax request
    /// </summary>    
    public class AjaxAuthorizationModule : HttpModuleBase
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
            // bind to presend request headers
            application.PreSendRequestHeaders += HasSecureAccess;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreSendRquestHeaders event of the context control.
        /// </summary>
        void HasSecureAccess(object sender, EventArgs e)
        {
            var httpApplication = (HttpApplication)sender;
            var response = new HttpResponseWrapper(httpApplication.Response);
			var request = new HttpRequestWrapper(httpApplication.Request);
			var context = new HttpContextWrapper(httpApplication.Context);

            if (true.Equals(context.Items["RequestWasNotAuthorized"]) && request.IsAjaxRequest())
            {
                response.StatusCode = 401;
                response.ClearContent();
            }
        }

        #endregion
    }
}