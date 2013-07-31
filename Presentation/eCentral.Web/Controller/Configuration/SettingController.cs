using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Configuration;
using eCentral.Services.Security;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.Setting;

namespace eCentral.Web.Controllers.Configuration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class SettingController : BaseController
    {
        #region Fields

        private readonly IPermissionService permissionService;
        private readonly SiteInformationSettings siteInformationSettings;
        private readonly DomainSettings domainSettings;
        private readonly SeoSettings seoSettings;
        private readonly SecuritySettings securitySettings;
        private readonly ISettingService settingService;
        
        #endregion Fields

        #region Constructors

        public SettingController(IPermissionService permissionService,
            SiteInformationSettings siteInformationSettings, 
            DomainSettings domainSettings, SeoSettings seoSettings,
            SecuritySettings securitySettings, ISettingService settingService)
        {
            this.permissionService       = permissionService;
            this.siteInformationSettings = siteInformationSettings;
            this.domainSettings          = domainSettings;
            this.securitySettings        = securitySettings;
            this.seoSettings             = seoSettings;
            this.settingService          = settingService;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageSettings)]
        public ActionResult Index()
        {
            //set page timeout to 5 minutes
            this.Server.ScriptTimeout = 300;

            var model = new GeneralCommonSettingsModel();
            // site information
            model.SiteInformationSettings.SiteName = siteInformationSettings.SiteName;
            model.SiteInformationSettings.SiteNameBlurb = siteInformationSettings.SiteNameBlurb;
            model.SiteInformationSettings.SiteUrl = siteInformationSettings.SiteUrl;
            model.SiteInformationSettings.SupportEmailAddress = siteInformationSettings.SupportEmailAddress;
            model.SiteInformationSettings.DisplayMiniProfilerInPublicSite = siteInformationSettings.DisplayMiniProfilerInPublicSite;
            model.SiteInformationSettings.SiteClosed = siteInformationSettings.SiteClosed;
            model.SiteInformationSettings.SiteClosedAllowForWebAdmins = siteInformationSettings.SiteClosedAllowForWebAdmins;
            model.SiteInformationSettings.ApplicationStateValues = siteInformationSettings.ApplicationState.ToSelectList();
            
            // domain settings
            model.DomainSettings.WWWStatusValues = domainSettings.wwwStatus.ToSelectList();
            model.DomainSettings.SSLStatusValues = domainSettings.sslStatus.ToSelectList();

            // seo settings
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;

            // security settings
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            model.SecuritySettings.HideMenuItemsBasedOnPermissions = securitySettings.HideMenuItemsBasedOnPermissions;            

            return View(model);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageSettings)]
        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult Index( GeneralCommonSettingsModel model)
        {
            //site information
            siteInformationSettings.SiteName = model.SiteInformationSettings.SiteName;
            siteInformationSettings.SiteNameBlurb = model.SiteInformationSettings.SiteNameBlurb;

            if (model.SiteInformationSettings.SiteUrl == null)
                model.SiteInformationSettings.SiteUrl = "";
            siteInformationSettings.SiteUrl = model.SiteInformationSettings.SiteUrl;
            //ensure we have "/" at the end
            if (!siteInformationSettings.SiteUrl.EndsWith("/"))
                siteInformationSettings.SiteUrl += "/";

            siteInformationSettings.DisplayMiniProfilerInPublicSite = model.SiteInformationSettings.DisplayMiniProfilerInPublicSite;
            siteInformationSettings.ApplicationState = model.SiteInformationSettings.ApplicationState;
            siteInformationSettings.SiteClosed = model.SiteInformationSettings.SiteClosed;
            siteInformationSettings.SiteClosedAllowForWebAdmins = model.SiteInformationSettings.SiteClosedAllowForWebAdmins;
            siteInformationSettings.SupportEmailAddress = model.SiteInformationSettings.SupportEmailAddress;            
            settingService.Save(siteInformationSettings); //save site information

            // security settings
            securitySettings.HideMenuItemsBasedOnPermissions = model.SecuritySettings.HideMenuItemsBasedOnPermissions;            
            settingService.Save(securitySettings); // save security information

            // domain settings
            domainSettings.wwwStatus = model.DomainSettings.WWWStatus; 
            domainSettings.sslStatus = model.DomainSettings.SSLStatus;
            settingService.Save(domainSettings); // domain settings

            // seo settings
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            settingService.Save(seoSettings); // seo settings

            SuccessNotification("The settings have been updated successfully.");
            return RedirectToAction(SystemRouteNames.Index);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageSettings)]
        public ActionResult Advanced()
        {
            return View();
        }

        
        #endregion
    }
}
