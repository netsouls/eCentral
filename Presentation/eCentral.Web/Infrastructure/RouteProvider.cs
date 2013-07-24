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

            //login
            routes.MapLocalizedRoute(SystemRouteNames.Login,
                SystemRouteUrls.Login,
                new { controller = "Security", action = "Login" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapLocalizedRoute(SystemRouteNames.Logout,
                SystemRouteUrls.Logout,
                new { controller = "Security", action = "Logout" },
                new[] { "eCentral.Web.Controllers" });

            // security settings
            routes.MapLocalizedRoute(SystemRouteNames.PasswordRecovery,
                SystemRouteUrls.PasswordRecovery,
                new { controller = "Security", action = "PasswordRecovery" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapLocalizedRoute(SystemRouteNames.ChangePassword,
                SystemRouteUrls.ChangePassword,
                new { controller = "Security", action = "ChangePassword" },
                new[] { "eCentral.Web.Controllers" });

            #region Users 

            routes.MapRoute(SystemRouteNames.User,
                SystemRouteUrls.User,
                new { controller = "User", action = "Index" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.UserAdd,
                SystemRouteUrls.UserAdd,
                new { controller = "User", action = "Add" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.UserEdit,
                SystemRouteUrls.UserEdit,
                new { controller = "User", action = "Edit" },
                new { rowId = new GuidConstraint(true) },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.UserChangeStatus,
                SystemRouteUrls.UserChangeStatus,
                new { controller = "User", action = "ChangeStatus" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.UserProfile,
                SystemRouteUrls.UserProfile,
                new { controller = "User", action = "View", rowId = UrlParameter.Optional },
                new { rowId = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            #endregion

            #region Clients

            routes.MapRoute(SystemRouteNames.Client,
                SystemRouteUrls.Client,
                new { controller = "Client", action = "Index" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.ClientAdd,
                SystemRouteUrls.ClientAdd,
                new { controller = "Client", action = "Add" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.ClientEdit,
                SystemRouteUrls.ClientEdit,
                new { controller = "Client", action = "Edit" },
                new { rowId = new GuidConstraint(true) },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.ClientChangeStatus,
                SystemRouteUrls.ClientChangeStatus,
                new { controller = "Client", action = "ChangeStatus" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.ClientView,
                SystemRouteUrls.ClientView,
                new { controller = "Client", action = "View" },
                new { rowId = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            #endregion

            #region Branch Offices

            routes.MapRoute(SystemRouteNames.BranchOffice,
                SystemRouteUrls.BranchOffice,
                new { controller = "BranchOffice", action = "Index" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.BranchOfficeAdd,
                SystemRouteUrls.BranchOfficeAdd,
                new { controller = "BranchOffice", action = "Add" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.BranchOfficeEdit,
                SystemRouteUrls.BranchOfficeEdit,
                new { controller = "BranchOffice", action = "Edit" },
                new { rowId = new GuidConstraint(true) },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.BranchOfficeChangeStatus,
                SystemRouteUrls.BranchOfficeChangeStatus,
                new { controller = "BranchOffice", action = "ChangeStatus" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.BranchOfficeView,
                SystemRouteUrls.BranchOfficeView,
                new { controller = "BranchOffice", action = "View" },
                new { rowId = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            #endregion

            #region Companies

            routes.MapRoute(SystemRouteNames.Company,
                SystemRouteUrls.Company,
                new { controller = "Company", action = "Index" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.CompanyAdd,
                SystemRouteUrls.CompanyAdd,
                new { controller = "Company", action = "Add" },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.CompanyEdit,
                SystemRouteUrls.CompanyEdit,
                new { controller = "Company", action = "Edit" },
                new { rowId = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            routes.MapRoute(SystemRouteNames.CompanyView,
                SystemRouteUrls.CompanyView,
                new { controller = "Company", action = "View" },
                new { rowId = new GuidConstraint(false) },
                new[] { "eCentral.Web.Controllers" });

            #endregion

            #region Configurations

            #endregion

            #region System

            #endregion

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
