using System.Web.Routing;

namespace eCentral.Web.Framework.Mvc.Routes
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(RouteCollection routeCollection);
    }
}
