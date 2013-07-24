using System;
using System.Web.Mvc;

namespace eCentral.Web.Framework
{
    public static class UrlHelperExtensions
    {
        public static string LogOn(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Login", "Account", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Login", "Account");
        }

        public static string LogOff(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Logout", "Account", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Logout", "Account");
        }

        /// <summary>
        /// Application image url
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ImageUrl(this UrlHelper urlHelper, string url)
        {
            return urlHelper.Content(string.Format("~/library/images/{0}", url));
        }

        /// <summary>
        /// Returns the edit url
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="routeName"></param>
        /// <returns></returns>
        public static string EditUrl ( this UrlHelper urlHelper, string routeName)
        {
            return urlHelper.RouteUrl(routeName, new { rowId = Guid.Empty.ToString() }).Replace(Guid.Empty.ToString(), string.Empty);
        }
    }
}
