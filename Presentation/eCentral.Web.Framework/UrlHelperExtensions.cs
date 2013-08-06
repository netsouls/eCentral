using System;
using System.Web.Mvc;
using System.Web.Routing;

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

        public static MvcHtmlString BackActionLink(this UrlHelper htmlHelper, string linkText, string actionName)
        {
            return htmlHelper.BackActionLink(linkText, actionName, null, new RouteValueDictionary());
        }

        public static MvcHtmlString BackActionLink(this UrlHelper urlHelper, string linkText, string actionName, string controllerName)
        {
            return urlHelper.BackActionLink(linkText, actionName, controllerName, new RouteValueDictionary());
        }

        public static MvcHtmlString BackActionLink(this UrlHelper urlHelper, string linkText, string actionName, RouteValueDictionary routeValues)
        {
            return urlHelper.BackActionLink(linkText, actionName, null, routeValues);
        }

        public static MvcHtmlString BackActionLink(this UrlHelper urlHelper, string linkText, string actionName, string controllerName, object routeValues)
        {
            return urlHelper.BackActionLink(linkText, actionName, controllerName, new RouteValueDictionary(routeValues));
        }

        public static MvcHtmlString BackActionLink(this UrlHelper urlHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues)
        {
            var linkUrl = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, urlHelper.RouteCollection, urlHelper.RequestContext, true);

            var div = new TagBuilder("div");
            div.AddCssClass("sidebar-widget");

            var title = new TagBuilder("h5");
            title.AddCssClass("title");

            var anchor = new TagBuilder("a");
            anchor.Attributes.Add("href", linkUrl);
            var icon = new TagBuilder("span");
            icon.AddCssClass("icon icomoon-icon-enter-4 white");
            anchor.InnerHtml = linkText + icon.ToString(TagRenderMode.Normal);
            title.InnerHtml = anchor.ToString(TagRenderMode.Normal);
            div.InnerHtml = title.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(div.ToString(TagRenderMode.Normal));
        }
    }
}
