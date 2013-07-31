using System.Web.Mvc;
using eCentral.Core.Domain;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Setting
{
    public partial class GeneralCommonSettingsModel : BaseModel
    {
        public GeneralCommonSettingsModel()
        {
            SiteInformationSettings = new SiteInformationSettingsModel();
            SeoSettings = new SeoSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
            DomainSettings = new DomainSettingsModel();
        }

        public SiteInformationSettingsModel SiteInformationSettings { get; set; }
        public DomainSettingsModel DomainSettings { get; set; }
        public SeoSettingsModel SeoSettings { get; set; }
        public SecuritySettingsModel SecuritySettings { get; set; }
        
        #region Nested classes

        public partial class DomainSettingsModel : BaseModel
        {
            [SiteResourceDisplayName("Settings.GeneralCommon.WWWStatus")]
            public WWWStatus WWWStatus { get; set; }
            public SelectList WWWStatusValues { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.UseSSL")]
            public SSLStatus SSLStatus { get; set; }
            public SelectList SSLStatusValues { get; set; }
        }

        public partial class SiteInformationSettingsModel : BaseModel
        {
            [SiteResourceDisplayName("Settings.GeneralCommon.SiteName")]
            [AllowHtml]
            public string SiteName { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.Blurb")]
            [AllowHtml]
            public string SiteNameBlurb { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.SiteUrl")]
            public string SiteUrl { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.SiteClosed")]
            public bool SiteClosed { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.SiteClosedAllowForWebAdmins")]
            public bool SiteClosedAllowForWebAdmins { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.ApplicationState")]
            public ApplicationState ApplicationState { get; set; }
            public SelectList ApplicationStateValues { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.SupportEmailAddress")]
            public string SupportEmailAddress { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.DisplayMiniProfilerInPublicSite")]
            public bool DisplayMiniProfilerInPublicSite { get; set; }
        }

        public partial class SeoSettingsModel : BaseModel
        {
            [SiteResourceDisplayName("Settings.GeneralCommon.PageTitleSeparator")]
            [AllowHtml]
            public string PageTitleSeparator { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.DefaultTitle")]
            [AllowHtml]
            public string DefaultTitle { get; set; }
        }

        public partial class SecuritySettingsModel : BaseModel
        {
            [SiteResourceDisplayName("Settings.GeneralCommon.EncryptionKey")]
            [AllowHtml]
            public string EncryptionKey { get; set; }

            [SiteResourceDisplayName("Settings.GeneralCommon.HideAdminMenuItemsBasedOnPermissions")]
            public bool HideMenuItemsBasedOnPermissions { get; set; }
        }

        #endregion
    }
}