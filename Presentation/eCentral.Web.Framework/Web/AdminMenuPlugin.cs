using eCentral.Core.Plugins;
using Telerik.Web.Mvc.UI;

namespace eCentral.Web.Framework.Web
{
    public interface IAdminMenuPlugin : IPlugin
    {
        void BuildMenuItem(MenuItemBuilder menuItemBuilder);
    }
}
