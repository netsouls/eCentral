using System;
using System.Web;
using System.Web.Mvc;

namespace eCentral.Web.Framework
{
    public class NoCacheControlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            if (filterContext.IsChildAction)
                return;

            HttpResponseBase response = filterContext.HttpContext.Response;

            response.Expires         = 0;
            response.ExpiresAbsolute = DateTime.Now.AddMinutes(-1.0);
            response.CacheControl    = "Private";

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
        }   
    }
}
