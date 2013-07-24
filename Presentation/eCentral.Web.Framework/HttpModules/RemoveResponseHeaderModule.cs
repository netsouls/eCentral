using System;
using System.Collections.Generic;
using System.Web;

namespace eCentral.Web.Framework.HttpModules
{
    /// <summary>
    /// Removes extra header information 
    /// </summary>
    public class RemoveResponseHeaderModule : HttpModuleBase
    {
        #region Constants and Fields

        /// <summary>
        /// List of Headers to remove
        /// </summary>
        private List<string> headersToCloak;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveResponseHeaderModule"/> class.
        /// </summary>
        public RemoveResponseHeaderModule()
        {
            this.headersToCloak = new List<string>
                                      {
                                              "Server",
                                              "X-AspNet-Version",
                                              "X-AspNetMvc-Version",
                                              "X-Powered-By",
                                              "X-Miniprofiler-Ids",
                                      };
        }

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
            context.PreSendRequestHeaders += new EventHandler(OnPreSendRequestHeaders);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Handles the PreSendRequestHeaders event of the context control.
        /// </summary>
        void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            //this.headersToCloak.ForEach(h => HttpContext.Current.Response.Headers.Remove(h));
        }

        #endregion
    }
}