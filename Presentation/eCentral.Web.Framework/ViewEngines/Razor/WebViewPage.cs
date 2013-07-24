using System;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Infrastructure;
using eCentral.Services.Localization;
using eCentral.Web.Framework.Localization;

namespace eCentral.Web.Framework.ViewEngines.Razor
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {

        private ILocalizationService _localizationService;
        private Localizer _localizer;
        private IWorkContext _workContext;
        
        public Localizer T
        {
            get
            {
                if (_localizer == null)
                {
                    //default localizer
                    _localizer = (format, args) =>
                                     {
                                         var resFormat = _localizationService.GetResource(format);
                                         if (string.IsNullOrEmpty(resFormat))
                                         {
                                             return new LocalizedString(format);
                                         }
                                         return
                                             new LocalizedString((args == null || args.Length == 0)
                                                                     ? resFormat
                                                                     : string.Format(resFormat, args));
                                     };
                }
                return _localizer;
            }
        }

        public IWorkContext WorkContext
        {
            get
            {
                return _workContext;
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            if (DataSettingsHelper.DatabaseIsInstalled())
            {
                _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                _workContext = EngineContext.Current.Resolve<IWorkContext>();
            }
        }

        public HelperResult RenderWrappedSection(string name, object wrapperHtmlAttributes)
        {
            Action<TextWriter> action = delegate(TextWriter tw)
                                {
                                    var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(wrapperHtmlAttributes);
                                    var tagBuilder = new TagBuilder("div");
                                    tagBuilder.MergeAttributes(htmlAttributes);

                                    var section = RenderSection(name, false);
                                    if (section != null)
                                    {
                                        tw.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                                        section.WriteTo(tw);
                                        tw.Write(tagBuilder.ToString(TagRenderMode.EndTag));
                                    }
                                };
            return new HelperResult(action);
        }

        public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        {
            return IsSectionDefined(sectionName) ? RenderSection(sectionName) : defaultContent(new object());
        }

        public override string Layout
        {
            get
            {
                var layout = base.Layout;

                if (!string.IsNullOrEmpty(layout))
                {
                    var filename = System.IO.Path.GetFileNameWithoutExtension(layout);
                    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(ViewContext.Controller.ControllerContext, filename, "");

                    if (viewResult.View != null)
                    {
                        layout = (viewResult.View as RazorView).ViewPath;
                    }
                }

                return layout;
            }
            set
            {
                base.Layout = value;
            }
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}