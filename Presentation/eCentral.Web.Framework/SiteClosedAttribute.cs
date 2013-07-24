using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain;
using eCentral.Core.Infrastructure;
using eCentral.Services.Users;

namespace eCentral.Web.Framework
{
    public class SiteClosedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var urlHelper   = new UrlHelper(filterContext.HttpContext.RequestContext());
            var currentUrl = filterContext.HttpContext.Request.RawUrl.ToLowerInvariant();

            var siteInformationSettings = EngineContext.Current.Resolve<SiteInformationSettings>();

            if (siteInformationSettings.SiteClosed)
            {
                if (currentUrl.IsCaseInsensitiveEqual(urlHelper.RouteUrl(SystemRouteNames.SiteClosed)))
                    return;

                // allowed urls
                List<string> allowedUrls = new List<string>();
                allowedUrls.Add(urlHelper.RouteUrl(SystemRouteNames.Login));
                allowedUrls.Add(urlHelper.RouteUrl(SystemRouteNames.Logout));
                
                if (siteInformationSettings.SiteClosedAllowForWebAdmins && allowedUrls.Contains(currentUrl))
                    return;

                // check for user access
                var currentUser = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;
                var isWebmaster = false;
                if (currentUser != null)
                    isWebmaster = currentUser.IsAdministrator();

                if (siteInformationSettings.SiteClosedAllowForWebAdmins && isWebmaster)
                {
                    //do nothing - allow admin access
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(SystemRouteNames.SiteClosed, new RouteValueDictionary());
                }
            }
        }
    }
}
