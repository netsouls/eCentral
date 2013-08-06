using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Logging;
using eCentral.Services.Security;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.System;

namespace eCentral.Web.Controllers.SystemMaintenance
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    [PermissionAuthorization(Permission = SystemPermissionNames.ManageMaintenance)]
    public class InfoController : BaseController
    {
        #region Fields

        private readonly IWorkContext workContext;
        private readonly IWebHelper webHelper;
        private readonly IUserActivityService activityService;
        private readonly IPermissionService permissionService;

        #endregion

        #region Ctor

        public InfoController(IWorkContext workContext, IWebHelper webHelper, 
            IUserActivityService activityService, IPermissionService permissionService)
        {
            this.workContext         = workContext;
            this.webHelper           = webHelper;
            this.activityService     = activityService;
            this.permissionService   = permissionService;
        }

        #endregion

        public ActionResult Index()
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

        public ActionResult ClearCache()
        {
            var cacheManager = new MemoryCacheManager();
            cacheManager.Clear();

            // set this in the activity log
            activityService.InsertActivity(SystemActivityLogTypeNames.ClearCache,
                string.Empty, string.Empty);

            SuccessNotification("Application cache has been cleared.");
            return RedirectToRoute(SystemRouteNames.HomePage);
        }

        public ActionResult ReinstallPermissions()
        {
            var permissionProviders = new List<Type>();
            permissionProviders.Add(typeof(StandardPermissionProvider));

            foreach (var providerType in permissionProviders)
            {
                dynamic provider = Activator.CreateInstance(providerType);
                permissionService.Uninstall(provider);
                permissionService.Install(provider);
            }

            // set this in the activity log
            activityService.InsertActivity(SystemActivityLogTypeNames.ReinstallPermissions,
                string.Empty, string.Empty);

            SuccessNotification("User permissions have been re-installed");
            return RedirectToRoute(SystemRouteNames.HomePage);
        }

        public ActionResult RestartApplication()
        {
            webHelper.RestartAppDomain();

            SuccessNotification("Application successfully restarted.");
            return RedirectToRoute(SystemRouteNames.HomePage);
        }
    }
}