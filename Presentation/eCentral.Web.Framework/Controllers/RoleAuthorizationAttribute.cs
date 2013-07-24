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
    public class RoleAuthorizationAttribute : FilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Gets or set the role to authorize
        /// </summary>
        public string Role { get; set; }

        private void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }

        private IEnumerable<RoleAuthorizationAttribute> GetRoleAuthorizationAttributes(ActionDescriptor descriptor)
        {
            return descriptor.GetCustomAttributes(typeof(RoleAuthorizationAttribute), true)
                .Concat(descriptor.ControllerDescriptor.GetCustomAttributes(typeof(RoleAuthorizationAttribute), true))
                .OfType<RoleAuthorizationAttribute>();
        }

        private bool IsSecurePageRequested(AuthorizationContext filterContext)
        {
            var adminAttributes = GetRoleAuthorizationAttributes(filterContext.ActionDescriptor);
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
                {
                    if ( filterContext.HttpContext.Request.IsAjaxRequest())
                        filterContext.HttpContext.Items["RequestWasNotAuthorized"] = true;

                    this.HandleUnauthorizedRequest(filterContext);
                    return;
                }
            }
        }

        public virtual bool HasSecureAccess(AuthorizationContext filterContext)
        {
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            bool result = permissionService.AuthorizeRole(Role);
            return result;
        }
    }
}
