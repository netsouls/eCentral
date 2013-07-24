using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core.Infrastructure;
using eCentral.Services.Security;

namespace eCentral.Web.Framework.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class PermissionAuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Gets or set the permission to authorize
        /// </summary>
        public string Permission { get; set; }

        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // redirect the user to the access denied page
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"controller", "Security"}, { "action", "AccessDenied"}, {"area", ""},  {"ReturnUrl", filterContext.HttpContext.Request.RawUrl}
                });
        }

        private IEnumerable<PermissionAuthorizationAttribute> GetPermissionAuthorizationAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(PermissionAuthorizationAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(PermissionAuthorizationAttribute), true))
                .OfType<PermissionAuthorizationAttribute>();
        }

        private bool IsSecurePageRequested(AuthorizationContext filterContext)
        {
            var adminAttributes = GetPermissionAuthorizationAttributes(filterContext.ActionDescriptor);
            if (adminAttributes != null && adminAttributes.Any())
                return true;
            return false;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
                throw new InvalidOperationException("You cannot use [SiteAuthorization] attribute when a child action cache is active");

            if (IsSecurePageRequested(filterContext))
            {
                if (!this.HasSecureAccess(filterContext))
                    this.HandleUnauthorizedRequest(filterContext);
            }
        }

        public virtual bool HasSecureAccess(AuthorizationContext filterContext)
        {
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            bool result = permissionService.Authorize(Permission);
            return result;
        }
    }
}
