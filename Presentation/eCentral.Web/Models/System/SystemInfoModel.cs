using System;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.System
{
    public class SystemInfoModel : BaseModel
    {
        [SiteResourceDisplayName("System.SystemInfo.ASPNETInfo")]
        public string AspNetInfo { get; set; }

        [SiteResourceDisplayName("System.SystemInfo.IsFullTrust")]
        public string IsFullTrust { get; set; }

        [SiteResourceDisplayName("System.SystemInfo.SiteVersion")]
        public string SiteVersion { get; set; }

        [SiteResourceDisplayName("System.SystemInfo.OperatingSystem")]
        public string OperatingSystem { get; set; }

        [SiteResourceDisplayName("System.SystemInfo.ServerLocalTime")]
        public DateTime ServerLocalTime { get; set; }

        [SiteResourceDisplayName("System.SystemInfo.ServerTimeZone")]
        public string ServerTimeZone { get; set; }

        [SiteResourceDisplayName("System.SystemInfo.UTCTime")]
        public DateTime UtcTime { get; set; }
    }
}