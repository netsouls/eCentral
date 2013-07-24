using System;
using System.Collections.Generic;
using eCentral.Core;
using eCentral.Core.Domain.Users;

namespace eCentral.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public partial interface IUserService : IPropertyValidatorService
    {
        #region Users

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <param name="registrationFrom">user registration from; null to load all users</param>
        /// <param name="registrationTo">user registration to; null to load all users</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="username">Username; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>user collection</returns>
        IPagedList<User> GetAll(DateTime? registrationFrom,
           DateTime? registrationTo, Guid[] userRoleIds, string username,
           int pageIndex, int pageSize);

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>User collection</returns>
        IList<User> GetAll(PublishingStatus status = PublishingStatus.Active);

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">user last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>user collection</returns>
        IPagedList<User> GetOnline(DateTime lastActivityFromUtc,
            Guid[] userRoleIds, int pageIndex, int pageSize);

        /// <summary>
        /// Gets all users by user role id
        /// </summary>
        /// <param name="userRoleId">user role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>user collection</returns>
        IList<User> GetByUserRoleId(Guid userRoleId, PublishingStatus publishingStatus = PublishingStatus.Active);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="user">user</param>
        void Delete(User user);

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <returns>A user</returns>
        User GetById(Guid userId);

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">user GUID</param>
        /// <returns>A user</returns>
        User GetByGuid(Guid userGuid);

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="customerIds">Customer identifiers</param>
        /// <returns>Customers</returns>
        IList<User> GetByIds(Guid[] userIds);
        
        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>user</returns>
        User GetByUsername(string username);

        /// <summary>
        /// Get users by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>users</returns>
        IList<User> GetUsersByLanguageId(Guid languageId);

        /// <summary>
        /// Get users by currency identifier
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>users</returns>
        IList<User> GetUsersByCurrencyId(Guid currencyId);

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">user</param>
        void Insert(User user);

        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">user</param>
        void Update(User user);

        #endregion

        #region user roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">user role</param>
        void DeleteUserRole(UserRole userRole);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">user role identifier</param>
        /// <returns>user role</returns>
        UserRole GetUserRoleById(Guid userRoleId);

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">user role system name</param>
        /// <returns>user role</returns>
        UserRole GetUserRoleBySystemName(string systemName);

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>user role collection</returns>
        IList<UserRole> GetAllUserRoles(bool showHidden = false);

        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">user role</param>
        void InsertUserRole(UserRole userRole);

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">user role</param>
        void UpdateUserRole(UserRole userRole);

        #endregion

        #region user tags

        /// <summary>
        /// Deletes a user tag
        /// </summary>
        /// <param name="usertag">user tag</param>
        //void DeleteUserTag(UserTag usertag);

        /// <summary>
        /// Gets a user tag
        /// </summary>
        /// <param name="usertagId">user tag identifier</param>
        /// <returns>A user tag</returns>
        //UserTag GetUserTagById(Guid usertagId);

        /// <summary>
        /// Inserts a user tag
        /// </summary>
        /// <param name="usertag">user tag</param>
        //void InsertUserTag(UserTag usertag);

        /// <summary>
        /// Updates the user tag
        /// </summary>
        /// <param name="usertag">user tag</param>
        //void UpdateUserTag(UserTag usertag);

        #endregion
    }
}