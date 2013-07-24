using System.Web;

namespace eCentral.Web.Framework.HttpHandlers
{
    /// <summary>
    /// Provides an abstract base class for <see cref="IHttpHandler"/>
    /// </summary>
    public abstract class HttpHandlerBase : IHttpHandler
    {
        /// <summary>
        /// Enables proceeCentral.ng of HTTP Web requests by a custom 
        ///     HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        /// <param name="context">
        /// An <see cref="T:System.Web.HttpContext"></see> object that provides 
        ///     references to the intrinsic server objects 
        ///     (for example, Request, Response, SeeCentral.on, and Server) used to service HTTP requests.
        /// </param>
        public void ProcessRequest(HttpContext context)
        {
            this.ProcessRequest((HttpContextBase) new HttpContextWrapper(context));
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref = "T:System.Web.IHttpHandler"></see> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref = "T:System.Web.IHttpHandler"></see> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// Enables proceeCentral.ng of HTTP Web requests by a custom 
        /// HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"></see> interface.
        /// </summary>
        public abstract void ProcessRequest(HttpContextBase context);
    }
}
