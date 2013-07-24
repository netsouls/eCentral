using System.Web.Routing;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Cms
{
    public class WidgetModel : BaseModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}