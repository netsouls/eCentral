using eCentral.Core;

namespace eCentral.Web.Models.Common
{
    public partial class ActionListModel
    {
        public string AddRouteUrl { get; set; }
        public string AddNewRouteName { get; set; }

        public string EditRouteName { get; set; }
        public string EditRouteUrl { get; set; }
        
        public string ChangeStatusRouteName { get; set; }
        public string ChangeStatusUrl { get; set; }

        public PublishingStatus[] PublishingStatus { get; set; }
    }
}