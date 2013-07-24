using System.Collections.Generic;
using System.Web.Mvc;

namespace eCentral.Web.Framework.Themes
{
    public class ThemeableRazorViewEngine : ThemeableBuildManagerViewEngine
    {
        public ThemeableRazorViewEngine()
        {
            AreaViewLocationFormats = new[]
                                          {
                                              //default
                                              "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                              "~/Areas/{2}/Views/Shared/{0}.cshtml"
                                          };

            AreaMasterLocationFormats = new[]
                                            {
                                                //default
                                                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml"
                                            };

            AreaPartialViewLocationFormats = new[]
                                                 {
                                                    //default
                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml"
                                                 };

            ViewLocationFormats = new[]
                                      {
                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml"
                                        };

            PartialViewLocationFormats = new[]
                                             {
                                                //default
                                                "~/Views/{1}/{0}.cshtml", 
                                                "~/Views/Shared/{0}.cshtml",
                                             };

            FileExtensions = new[] { "cshtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string layoutPath = null;
            var runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string layoutPath = masterPath;
            var runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }
    }
}
