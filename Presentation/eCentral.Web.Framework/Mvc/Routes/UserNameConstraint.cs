using System.Web;
using System.Web.Routing;
using eCentral.Core;

namespace eCentral.Web.Framework.Mvc.Routes
{
    public class UserNameConstraint : IRouteConstraint
    {
        public UserNameConstraint()
        {
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values.ContainsKey(parameterName))
            {
                string stringValue = values[parameterName] as string;

                if (!string.IsNullOrEmpty(stringValue))
                {
                    return CommonHelper.IsValidEmail(stringValue);
                }
            }

            return false;
        }
    }
}
