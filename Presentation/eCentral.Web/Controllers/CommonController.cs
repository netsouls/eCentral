using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain;
using eCentral.Core.Domain.Media;
using eCentral.Services.Localization;
using eCentral.Web.Models.Common;
using FileSystem = System.IO;

namespace eCentral.Web.Controllers
{
    public class CommonController : BaseController
    {
        #region Fields

        private readonly ILocalizationService localizationService;
        private readonly IWorkContext workContext;
        private readonly ICacheManager cacheManager;
        private readonly IWebHelper webHelper;

        private readonly SiteInformationSettings siteInformationSettings;
        private readonly MediaSettings mediaSettings;
        
        #endregion

        #region Ctor 

        public CommonController(ILocalizationService localizationService,
            IWorkContext workContext, IWebHelper webHelper, ICacheManager cacheManager,
            MediaSettings mediaSettings, SiteInformationSettings siteInformationSettings)
        {
            this.localizationService     = localizationService;
            this.workContext             = workContext;
            this.webHelper               = webHelper;
            this.mediaSettings           = mediaSettings;
            this.siteInformationSettings = siteInformationSettings;
            this.cacheManager            = cacheManager;
        }

        #endregion

        #region favicon

        [ChildActionOnly]
        public ActionResult Favicon()
        {
            var model = new FaviconModel()
            {
                Uploaded = FileSystem.File.Exists(Request.PhysicalApplicationPath + "library/images/favicon.ico"),
                FaviconUrl = webHelper.AbsoluteWebRoot + "library/images"
            };
            return PartialView(model);
        }

        #endregion 

        #region Menu Items

        [ChildActionOnly]
        public ActionResult Header()
        {
            var model = new HeaderModel()
            {
                DashboardActive = Url.RouteUrl(SystemRouteNames.HomePage).IsCaseInsensitiveEqual(Request.RawUrl),
                GravtarSize = mediaSettings.AvatarPictureSize, 
                SupportEmailAddress = siteInformationSettings.SupportEmailAddress
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Navigation()
        {
            // no model passed currently as not required
            return PartialView();
        }

        [ChildActionOnly]
        public ActionResult Breadcrumb ()
        {
            // no model passed currently as not required
            return PartialView();
        }

        #endregion
    }
}
