using System;
using System.Collections.Generic;
using eCentral.Core.Domain.Cms;

namespace eCentral.Services.Cms
{
    /// <summary>
    /// Widget service interface
    /// </summary>
    public partial interface IWidgetService
    {
        /// <summary>
        /// Load widget plugin provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget plugin</returns>
        IWidgetPlugin LoadWidgetPluginBySystemName(string systemName);

        /// <summary>
        /// Load all widget plugins
        /// </summary>
        /// <returns>widget plugins</returns>
        IList<IWidgetPlugin> LoadAllWidgetPlugins();

        /// <summary>
        /// Delete widget
        /// </summary>
        /// <param name="widget">Widget</param>
        void Delete(Widget widget);
        
        /// <summary>
        /// Gets all widgets
        /// </summary>
        /// <returns>Widgets</returns>
        IList<Widget> GetAll();

        /// <summary>
        /// Gets all widgets
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <returns>Widgets</returns>
        IList<Widget> GetAllByZone(WidgetZone widgetZone);

        /// <summary>
        /// Gets a widget
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <returns>Widget</returns>
        Widget GetById(Guid widgetId);

        /// <summary>
        /// Inserts widget
        /// </summary>
        /// <param name="widget">Widget</param>
        void Insert(Widget widget);

        /// <summary>
        /// Updates the widget
        /// </summary>
        /// <param name="widget">Widget</param>
        void Update(Widget widget);
    }
}
