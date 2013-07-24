using System.Web;
using System.Web.Routing;

namespace eCentral.Core
{
    public static class HttpContextBaseExtensions
    {
        public static RequestContext RequestContext(this HttpContextBase instance)
        {
            return new RequestContext(instance, RouteTable.Routes.GetRouteData(instance) ?? new RouteData());
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            Guard.IsNotNull(request, "request");

            return ((request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest")));
        }
    }
}
