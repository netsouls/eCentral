using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Users;

namespace eCentral.Services.Security
{
    /// <summary>
    /// Permission service
    /// </summary>
    public partial class PermissionService : IPermissionService
    {
        #region Constants
        private const string PERMISSIONS_ALL_KEY     = "eCentral.permission.all";
        private const string PERMISSIONS_PATTERN_KEY = "eCentral.permission.";
        #endregion

        #region Fields

        private readonly IRepository<PermissionRecord> permissionPecordRepository;
        private readonly IUserService userService;
        private readonly IWorkContext workContext;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="permissionPecordRepository">Permission repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="cacheManager">Cache manager</param>
        public PermissionService(IRepository<PermissionRecord> permissionPecordRepository,
            IUserService userService,
            IWorkContext workContext, ICacheManager cacheManager)
        {
            this.permissionPecordRepository = permissionPecordRepository;
            this.userService                = userService;
            this.workContext                = workContext;
            this.cacheManager               = cacheManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void Delete(PermissionRecord permission)
        {
            Guard.IsNotNull(permission, "permission");

            permissionPecordRepository.Delete(permission);

            cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="permissionId">Permission identifier</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetById(Guid permissionId)
        {
            if (permissionId.IsEmpty())
                return null;

            return permissionPecordRepository.GetById(permissionId);
        }

        /// <summary>
        /// Gets a permission
        /// </summary>
        /// <param name="systemName">Permission system name</param>
        /// <returns>Permission</returns>
        public virtual PermissionRecord GetBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;


            //Little performance optimization hack here.
            //We know that this method is used only in admin area menu when a lot of requests are made
            //so let's just load all of them (cached) and find required one
            return GetAll()
                .Where(p => systemName.Equals(p.SystemName, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();


            //string key = string.Format(PERMISSIONS_BY_SYSTEMNAME_KEY, systemName);
            //return cacheManager.Get(key, () =>
            //{
            //    var query = from pr in permissionPecordRepository.Table
            //                orderby pr.Id
            //                where pr.SystemName == systemName
            //                select pr;
            //    var permission = query.FirstOrDefault();
            //    return permission;
            //});
        }

        /// <summary>
        /// Gets all permissions
        /// </summary>
        /// <returns>Permissions</returns>
        public virtual IList<PermissionRecord> GetAll()
        {
            string key = string.Format(PERMISSIONS_ALL_KEY);
            return cacheManager.Get(key, () =>
            {
                var query = from cr in permissionPecordRepository.Table
                            orderby cr.Name
                            select cr;
                var permissions = query.ToList();
                return permissions;
            });
        }

        /// <summary>
        /// Inserts a permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void Insert(PermissionRecord permission)
        {
            Guard.IsNotNull(permission, "permission");

            permissionPecordRepository.Insert(permission);

            cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the permission
        /// </summary>
        /// <param name="permission">Permission</param>
        public virtual void Update(PermissionRecord permission)
        {
            Guard.IsNotNull(permission, "permission");

            permissionPecordRepository.Update(permission);

            cacheManager.RemoveByPattern(PERMISSIONS_PATTERN_KEY);
        }

        /// <summary>
        /// Install permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void Install(IPermissionProvider permissionProvider)
        {
            //install new permissions
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetBySystemName(permission.SystemName);
                if (permission1 == null)
                {
                    //new permission (install it)
                    permission1 = new PermissionRecord()
                    {
                        Name       = permission.Name,
                        SystemName = permission.SystemName,
                        Category   = permission.Category,
                    };


                    //default customer role mappings
                    var defaultPermissions = permissionProvider.GetDefaultPermissions();
                    foreach (var defaultPermission in defaultPermissions)
                    {
                        var userRole = userService.GetUserRoleBySystemName(defaultPermission.UserRoleSystemName);
                        if (userRole == null)
                        {
                            //new role (save it)
                            userRole = new UserRole()
                            {
                                Name       = defaultPermission.UserRoleSystemName,
                                Active     = true,
                                SystemName = defaultPermission.UserRoleSystemName
                            };
                            userService.InsertUserRole(userRole);
                        }


                        var defaultMappingProvided = (from p in defaultPermission.PermissionRecords
                                                      where p.SystemName == permission1.SystemName
                                                      select p).Any();
                        var mappingExists = (from p in userRole.PermissionRecords
                                             where p.SystemName == permission1.SystemName
                                             select p).Any();
                        if (defaultMappingProvided && !mappingExists)
                        {
                            permission1.UserRoles.Add(userRole);
                        }
                    }

                    //save new permission
                    Insert(permission1);
                }
            }
        }

        /// <summary>
        /// Uninstall permissions
        /// </summary>
        /// <param name="permissionProvider">Permission provider</param>
        public virtual void Uninstall(IPermissionProvider permissionProvider)
        {
            var permissions = permissionProvider.GetPermissions();
            foreach (var permission in permissions)
            {
                var permission1 = GetBySystemName(permission.SystemName);
                if (permission1 != null)
                {
                    Delete(permission1);
                }
            }
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permissionRecordSystemName">Permission record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(string permissionRecordSystemName)
        {
            if (String.IsNullOrEmpty(permissionRecordSystemName))
                return false;

            var permission = GetBySystemName(permissionRecordSystemName);
            return Authorize(permission);
        }

        /// <summary>
        /// Authorize role
        /// </summary>
        /// <param name="userRoleName">User Role record system name</param>
        /// <returns>true - authorized; otherwise, false</returns>        
        public virtual bool AuthorizeRole(string userRoleName)
        {
            if (string.IsNullOrEmpty(userRoleName))
                return false;

            var role = userService.GetUserRoleBySystemName(userRoleName);
            return AuthorizeRole(role);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <returns>true - authorized; otherwise, false</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public virtual bool Authorize(PermissionRecord permission)
        {
            return Authorize(permission, workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize role
        /// </summary>
        /// <param name="role">User Role record</param>
        /// <returns>true - authorized; otherwise, false</returns>        
        public virtual bool AuthorizeRole(UserRole role)
        {
            return AuthorizeRole(role, workContext.CurrentUser);
        }

        /// <summary>
        /// Authorize permission
        /// </summary>
        /// <param name="permission">Permission record</param>
        /// <param name="customer">Customer</param>
        /// <returns>true - authorized; otherwise, false</returns>
        public virtual bool Authorize(PermissionRecord permission, User user)
        {
            if (permission == null)
                return false;

            if (user == null)
                return false;

            var userRoles = user.UserRoles.Where(cr => cr.Active);
            foreach (var role in userRoles)
                foreach (var permission1 in role.PermissionRecords)
                    if (permission1.SystemName.Equals(permission.SystemName, StringComparison.InvariantCultureIgnoreCase))
                        return true;

            return false;
        }

        /// <summary>
        /// Authorize role
        /// </summary>
        /// <param name="role">User Role record</param>
        /// <param name="user">User</param>
        /// <returns>true - authorized; otherwise, false</returns>        
        public virtual bool AuthorizeRole(UserRole role, User user)
        {
            if (role == null)
                return false;

            if (user == null)
                return false;

            var userRoles = user.UserRoles.Where(ur => ur.Active);
            foreach (var userRole in userRoles)
                if (userRole.SystemName.Equals(role.SystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        #endregion
    }
}