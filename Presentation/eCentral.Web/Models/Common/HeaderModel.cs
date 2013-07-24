using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Common
{
    public class HeaderModel: BaseModel
    {
        public bool DashboardActive { get; set; }

        public int GravtarSize { get; set; }

        public string SupportEmailAddress { get; set; }
    }
}