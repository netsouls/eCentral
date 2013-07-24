using System.Web.Mvc;
using eCentral.Core.Infrastructure;

namespace eCentral.Web.Framework.UI
{
    public static class LayoutExtensions
    {
        public static void AddTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddTitleParts(parts);
        }
        public static void AppendTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendTitleParts(parts);
        }
        public static MvcHtmlString SiteTitle(this HtmlHelper html, bool addDefaultTitle, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendTitleParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateTitle(addDefaultTitle)));
        }


        public static void AddScriptParts(this HtmlHelper html, params string[] parts)
        {
            AddScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AddScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddScriptParts(location, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, params string[] parts)
        {
            AppendScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendScriptParts(location, parts);
        }
        public static MvcHtmlString SiteScripts(this HtmlHelper html, params string[] parts)
        {
            return SiteScripts(html, ResourceLocation.Head, parts);
        }
        public static MvcHtmlString SiteScripts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendScriptParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateScripts(location));
        }



        public static void AddCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AddCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AddCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddCssFileParts(location, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AppendCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendCssFileParts(location, parts);
        }
        public static MvcHtmlString SiteCssFiles(this HtmlHelper html, params string[] parts)
        {
            return SiteCssFiles(html, ResourceLocation.Head, parts);
        }
        public static MvcHtmlString SiteCssFiles(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendCssFileParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateCssFiles(location));
        }



        public static void AddCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddCanonicalUrlParts(parts);
        }
        public static void AppendCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendCanonicalUrlParts(parts);
        }
        public static MvcHtmlString SiteCanonicalUrls(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = EngineContext.Current.Resolve<IPageTitleBuilder>();
            html.AppendCanonicalUrlParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateCanonicalUrls());
        }
    }
}
