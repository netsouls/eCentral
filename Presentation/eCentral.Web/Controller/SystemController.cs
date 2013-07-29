using System;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Localization;
using eCentral.Services.Security;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.System;

namespace eCentral.Web.Controllers
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class SystemController : BaseController
    {
        #region Fields

        private readonly ILocalizationService localizationService;
        private readonly IWorkContext workContext;
        private readonly IWebHelper webHelper;
        private readonly IPermissionService permissionService;
        
        #endregion

        #region Ctor

        public SystemController(ILocalizationService localizationService,
            IWorkContext workContext, IWebHelper webHelper, 
            IPermissionService permissionService)
        {
            this.localizationService = localizationService;
            this.workContext = workContext;
            this.webHelper = webHelper;
            this.permissionService = permissionService;
        }

        #endregion

        #region methods
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageMaintenance)]
        public ActionResult Index()
        {
            return RedirectToAction("Info");
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageMaintenance)]
        public ActionResult Info()
        {
            var model = new SystemInfoModel();
            model.SiteVersion = SiteVersion.CurrentVersion;
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
            }
            catch (Exception) { }
            try
            {
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
            }
            catch (Exception) { }
            try
            {
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch (Exception) { }
            model.ServerTimeZone = TimeZone.CurrentTimeZone.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;

            return View(model);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageMaintenance)]
        public ActionResult ClearCache()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();

            SuccessNotification("Application cache has been cleared.");
            return RedirectToRoute(SystemRouteNames.HomePage);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageMaintenance)]
        public ActionResult RestartApplication()
        {
            webHelper.RestartAppDomain();

            SuccessNotification("Application successfully restarted.");
            return RedirectToRoute(SystemRouteNames.HomePage);
        }

        #endregion
    }
}
