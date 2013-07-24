using System.Web;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Domain.Media;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Domain.Users;
using eCentral.Services.Common;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Services.Messages;
using eCentral.Services.Security;
using eCentral.Services.Users;
using eCentral.Web.Models.Common;

namespace eCentral.Web.Controllers
{
    public class CommonController : BaseController
    {
        #region Fields

        private readonly ILanguageService languageService;
        private readonly ICurrencyService currencyService;
        private readonly ILocalizationService localizationService;
        private readonly IWorkContext workContext;
        private readonly IQueuedEmailService queuedEmailService;
        private readonly IEmailAccountService emailAccountService;
        private readonly IUserService userService;
        private readonly ICacheManager cacheManager;
        private readonly IWebHelper webHelper;
        private readonly IPermissionService permissionService;
        private readonly IMobileDeviceHelper mobileDeviceHelper;
        private readonly HttpContextBase httpContext;

        private readonly UserSettings userSettings;
        private readonly SiteInformationSettings siteInformationSettings;
        private readonly EmailAccountSettings emailAccountSettings;
        private readonly CommonSettings commonSettings;
        private readonly LocalizationSettings localizationSettings;
        private readonly MediaSettings mediaSettings;
        
        #endregion

        #region Ctor 

        public CommonController(ILanguageService languageService,
            ICurrencyService currencyService, ILocalizationService localizationService,
            IWorkContext workContext,
            IQueuedEmailService queuedEmailService, IEmailAccountService emailAccountService,            
            IUserService userService, IWebHelper webHelper, ICacheManager cacheManager,
            IPermissionService permissionService, IMobileDeviceHelper mobileDeviceHelper,
            HttpContextBase httpContext, UserSettings userSettings, MediaSettings mediaSettings,
            SiteInformationSettings siteInformationSettings, EmailAccountSettings emailAccountSettings,
            CommonSettings commonSettings, LocalizationSettings localizationSettings)
        {
            this.languageService         = languageService;
            this.currencyService         = currencyService;
            this.localizationService     = localizationService;
            this.workContext             = workContext;
            this.queuedEmailService      = queuedEmailService;
            this.emailAccountService     = emailAccountService;
            this.userService             = userService;
            this.webHelper               = webHelper;
            this.permissionService       = permissionService;
            this.mobileDeviceHelper      = mobileDeviceHelper;
            this.mediaSettings           = mediaSettings;
            this.httpContext             = httpContext;
            this.userSettings            = userSettings;
            this.siteInformationSettings = siteInformationSettings;
            this.emailAccountSettings    = emailAccountSettings;
            this.cacheManager            = cacheManager;
            this.commonSettings          = commonSettings;
            this.localizationSettings    = localizationSettings;
        }

        #endregion

        #region favicon

        [ChildActionOnly]
        public ActionResult Favicon()
        {
            var model = new FaviconModel()
            {
                Uploaded = System.IO.File.Exists(Request.PhysicalApplicationPath + "library/images/favicon.ico"),
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
