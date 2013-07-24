using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Cms;
using eCentral.Core.Plugins;
using eCentral.Services.Events;

namespace eCentral.Services.Cms
{
    /// <summary>
    /// Widget service
    /// </summary>
    public partial class WidgetService : IWidgetService
    {
        #region Constants

        private const string WIDGETS_BY_ID_KEY   = "eCentral.widget.id-{0}";
        private const string WIDGETS_ALL_KEY     = "eCentral.widget.all";
        private const string WIDGETS_PATTERN_KEY = "eCentral.widget.";

        #endregion

        #region Fields

        private readonly IRepository<Widget> widgetRepository;
        private readonly ICacheManager cacheManager;
        private readonly IPluginFinder pluginFinder;
        private readonly IEventPublisher eventPublisher;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="widgetRepository">Widget repository</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        public WidgetService(ICacheManager cacheManager,
            IRepository<Widget> widgetRepository, 
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher)
        {
            this.cacheManager     = cacheManager;
            this.widgetRepository = widgetRepository;
            this.pluginFinder     = pluginFinder;
            this.eventPublisher   = eventPublisher;
        }

        #endregion

        #region Methods



        /// <summary>
        /// Load widget plugin provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found widget plugin</returns>
        public virtual IWidgetPlugin LoadWidgetPluginBySystemName(string systemName)
        {
            var descriptor = this.pluginFinder.GetPluginDescriptorBySystemName<IWidgetPlugin>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IWidgetPlugin>();

            return null;
        }

        /// <summary>
        /// Load all widget plugins
        /// </summary>
        /// <returns>widget plugins</returns>
        public virtual IList<IWidgetPlugin> LoadAllWidgetPlugins()
        {
            return this.pluginFinder.GetPlugins<IWidgetPlugin>().ToList();
        }


        /// <summary>
        /// Delete widget
        /// </summary>
        /// <param name="widget">Widget</param>
        public virtual void Delete(Widget widget)
        {
            Guard.IsNotNull(widget, "Widget");

            this.widgetRepository.Delete(widget);

            this.cacheManager.RemoveByPattern(WIDGETS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(widget);
        }

        /// <summary>
        /// Gets all widgets
        /// </summary>
        /// <returns>Widgets</returns>
        public virtual IList<Widget> GetAll()
        {
            string key = WIDGETS_ALL_KEY;
            return this.cacheManager.Get(key, () =>
            {
                var query = from w in this.widgetRepository.Table
                            orderby w.DisplayOrder
                            select w;
                return query.ToList();
            });
        }

        /// <summary>
        /// Gets all widgets by zone
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <returns>Widgets</returns>
        public virtual IList<Widget> GetAllByZone(WidgetZone widgetZone)
        {
            var allWidgets = GetAll();
            var widgets = allWidgets
                .Where(w => w.WidgetZone == widgetZone)
                .OrderBy(w => w.DisplayOrder)
                .ToList();
            return widgets;
        }

        /// <summary>
        /// Gets a widget
        /// </summary>
        /// <param name="widgetId">Widget identifier</param>
        /// <returns>Widget</returns>
        public virtual Widget GetById(Guid widgetId)
        {
            if (widgetId.Equals(Guid.Empty))
                return null;

            string key = string.Format(WIDGETS_BY_ID_KEY, widgetId);
            return this.cacheManager.Get(key, () =>
            {
                var widget = this.widgetRepository.GetById(widgetId);
                return widget;
            });
        }

        /// <summary>
        /// Inserts widget
        /// </summary>
        /// <param name="widget">Widget</param>
        public virtual void Insert(Widget widget)
        {
            Guard.IsNotNull(widget, "Widget");

            this.widgetRepository.Insert(widget);

            //cache
            this.cacheManager.RemoveByPattern(WIDGETS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(widget);
        }

        /// <summary>
        /// Updates the widget
        /// </summary>
        /// <param name="widget">Widget</param>
        public virtual void Update(Widget widget)
        {
            Guard.IsNotNull(widget, "Widget");

            this.widgetRepository.Update(widget);

            //cache
            this.cacheManager.RemoveByPattern(WIDGETS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(widget);
        }
        
        #endregion
    }
}
