using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core.Domain.Cms;
using eCentral.Services.Cms;
using eCentral.Web.Models.Cms;

namespace eCentral.Web.Controllers
{
    public class WidgetController : BaseController
    {
		#region Fields

        private readonly IWidgetService widgetService;

        #endregion

		#region Constructors

        public WidgetController(IWidgetService widgetService)
        {
            this.widgetService = widgetService;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(WidgetZone widgetZone)
        {
            //model
            var model = new List<WidgetModel>();
            var widgets = widgetService.GetAllByZone(widgetZone);
            
            foreach (var widget in widgets)
            {
                var widgetPlugin = widgetService.LoadWidgetPluginBySystemName(widget.PluginSystemName);
                if (widgetPlugin == null || !widgetPlugin.PluginDescriptor.Installed)
                    continue;   //don't throw an exception. just process next widget.

                var widgetModel = new WidgetModel();

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                widgetPlugin.GetDisplayWidgetRoute(widget.RowId, out actionName, out controllerName, out routeValues);
                widgetModel.ActionName = actionName;
                widgetModel.ControllerName = controllerName;
                widgetModel.RouteValues = routeValues;

                model.Add(widgetModel);
            }

            return PartialView(model);
        }

        #endregion
    }
}
