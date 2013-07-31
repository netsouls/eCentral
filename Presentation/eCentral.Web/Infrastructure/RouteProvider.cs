using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core;
using eCentral.Web.Framework.Localization;
using eCentral.Web.Framework.Mvc.Routes;

namespace eCentral.Web.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //home page
            routes.MapLocalizedRoute(SystemRouteNames.HomePage,
                "",
                new { controller = "Home", action = "Index"},
                new[] { "eCentral.Web.Controllers" });

            #region Routes per Controller 
            
            routes.MapRoute(
                "Configuration", // Route name
                "configuration/{controller}/{action}/{rowId}", // Route Pattern
                new { controller = "Home", action = "Index", rowId = UrlParameter.Optional }, // Default values for parameters
                new { controller = @"(EmailAccount|MessageTemplate|Setting)" }); //Restriction for controller and action

            routes.MapRoute(
                "Administration", // Route name
                "administration/{controller}/{action}/{rowId}", // Route Pattern
                new { controller = "Home", action = "Index", rowId = UrlParameter.Optional }, // Default values for parameters
                new { controller = @"(Company|BranchOffice|Client|User)" }); //Restriction for controller and action

            routes.MapRoute(
                "SystemInfo", // Route name
                "system-info/{action}/{rowId}", // Route Pattern
                new { controller = "System", action = "Index", rowId = UrlParameter.Optional }); //Restriction for controller and action

            #endregion

            //login
            routes.MapLocalizedRoute(SystemRouteNames.Login,
                SystemRouteUrls.Login,
                new { controller = "Security", action = "Login" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapLocalizedRoute(SystemRouteNames.Logout,
                SystemRouteUrls.Logout,
                new { controller = "Security", action = "Logout" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapLocalizedRoute(SystemRouteNames.AccountActivation,
                SystemRouteUrls.AccountActivation,
                new { controller = "Security", action = "AccountActivation" },
                new { userId = new GuidConstraint(false), token = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            routes.MapLocalizedRoute(SystemRouteNames.PasswordRecovery,
                SystemRouteUrls.PasswordRecovery,
                new { controller = "Security", action = "PasswordRecovery" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapLocalizedRoute(SystemRouteNames.PasswordRecoveryConfirm,
                SystemRouteUrls.PasswordRecoveryConfirm,
                new { controller = "Security", action = "PasswordRecoveryConfirm" },
                new { userId = new GuidConstraint(false), token = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            /*routes.MapLocalizedRoute(SystemRouteNames.ChangePassword,
                SystemRouteUrls.ChangePassword,
                new { controller = "Security", action = "ChangePassword" },
                new[] { "eCentral.Web.Controllers" });*/

            //some AJAX links
            routes.MapRoute(SystemRouteNames.GetStatesByCountry,
                SystemRouteUrls.GetStatesByCountry,
                new { controller = "Country", action = "GetStatesByCountryId" },
                new[] { "Nop.Web.Controllers" });

            //upload image
            routes.MapRoute(SystemRouteNames.AsyncUpload, 
                SystemRouteUrls.AsyncUpload,
                new {controller = "Media", action = "AsyncUpload"},
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.Search,
                SystemRouteUrls.Search,
                new { controller = "Search", action = "Index" },
                new[] { "eCentral.Web.Controllers" });

            //static pages - error, 404 and site clsoed
            routes.MapRoute(SystemRouteNames.SiteClosed,
                SystemRouteUrls.SiteClosed,
                new { controller = "Static", action = "SiteClosed" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.PageNotFound,
                SystemRouteUrls.PageNotFound,
                new { controller = "Static", action = "PageNotFound" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.Error,
                SystemRouteUrls.Error,
                new { controller = "Static", action = "Error" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.AccessDenied,
                SystemRouteUrls.AccessDenied,
                new { controller = "Static", action = "AccessDenied" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{rowId}", // URL with parameters
                new { controller = "Home", action = "Index", area = "", rowId = UrlParameter.Optional },
                new { controller = @"(Common|Widget)" }); //Restriction for controller and action
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
