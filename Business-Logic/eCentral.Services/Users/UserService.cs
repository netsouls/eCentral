using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Users;
using eCentral.Services.Events;

namespace eCentral.Services.Users
{
    /// <summary>
    /// user service
    /// </summary>
    public partial class UserService : IUserService
    {
        #region Constants

        private const string USERROLES_ALL_KEY           = "eCentral.userrole.all-{0}";
        private const string USERROLES_BY_ID_KEY         = "eCentral.userrole.id-{0}";
        private const string USERROLES_BY_SYSTEMNAME_KEY = "eCentral.userrole.systemname-{0}";
        private const string USERROLES_PATTERN_KEY       = "eCentral.userrole.";

        #endregion

        #region Fields

        private readonly IRepository<User> userRepository;
        private readonly IRepository<UserRole> userRoleRepository;
        private readonly IRepository<GenericAttribute> attributeRepository;
        private readonly ICacheManager cacheManager;
        private readonly IEventPublisher eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="userRepository">user repository</param>
        /// <param name="userRoleRepository">user role repository</param>
        /// <param name="attributeRepository">attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        public UserService(ICacheManager cacheManager,
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<GenericAttribute> attributeRepository, 
            IEventPublisher eventPublisher)
        {
            this.cacheManager            = cacheManager;
            this.userRepository          = userRepository;
            this.userRoleRepository      = userRoleRepository;
            this.attributeRepository     = attributeRepository;
            this.eventPublisher          = eventPublisher;
        }

        #endregion

        #region Methods

        #region users
        
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
        public virtual IPagedList<User> GetAll(DateTime? registrationFrom,
           DateTime? registrationTo, Guid[] userRoleIds, string username,
           int pageIndex, int pageSize)
        {
            var query = this.userRepository.Table;
            if (registrationFrom.HasValue)
                query = query.Where(c => registrationFrom.Value <= c.CreatedOn);
            if (registrationTo.HasValue)
                query = query.Where(c => registrationTo.Value >= c.CreatedOn);
            
            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.RowId).Intersect(userRoleIds).Count() > 0);

            if (!String.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            query = query.OrderByDescending(c => c.CreatedOn);

            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>User collection</returns>
        public virtual IList<User> GetAll(PublishingStatus status = PublishingStatus.Active)
        {
            var query = from u in this.userRepository.Table
                        orderby u.Username
                        where (status == PublishingStatus.All || u.CurrentPublishingStatusId == (int)status)
                        select u;
            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Gets online users
        /// </summary>
        /// <param name="lastActivityFromUtc">user last activity date (from)</param>
        /// <param name="userRoleIds">A list of user role identifiers to filter by (at least one match); pass null or empty list in order to load all users; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>user collection</returns>
        public virtual IPagedList<User> GetOnline(DateTime lastActivityFromUtc,
            Guid[] userRoleIds, int pageIndex, int pageSize)
        {
            var query = this.userRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDate);
            
            if (userRoleIds != null && userRoleIds.Length > 0)
                query = query.Where(c => c.UserRoles.Select(cr => cr.RowId).Intersect(userRoleIds).Count() > 0);
            
            query = query.OrderByDescending(c => c.LastActivityDate);
            var users = new PagedList<User>(query, pageIndex, pageSize);
            return users;
        }

        /// <summary>
        /// Gets all users by user role id
        /// </summary>
        /// <param name="userRoleId">user role identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>user collection</returns>
        public virtual IList<User> GetByUserRoleId(Guid userRoleId, PublishingStatus publishingStatus = PublishingStatus.Active)
        {
            var query = from c in this.userRepository.Table
                        from cr in c.UserRoles
                        where ( publishingStatus == PublishingStatus.All ||  c.CurrentPublishingStatus == publishingStatus) &&
                            cr.RowId == userRoleId
                        orderby c.CreatedOn descending
                        select c;
            
            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <returns>A user</returns>
        public virtual User GetById(Guid userId)
        {
            Guard.IsNotEmpty(userId, "userId");

            var user = this.userRepository.GetById(userId);
            return user;
        }

        /// <summary>
        /// Gets a user by GUID
        /// </summary>
        /// <param name="userGuid">user GUID</param>
        /// <returns>A user</returns>
        public virtual User GetByGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return null;

            var query = from c in this.userRepository.Table
                        where c.UserGuid == userGuid
                        orderby c.RowId
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get users by identifiers
        /// </summary>
        /// <param name="userIds">User identifiers</param>
        /// <returns>User</returns>
        public virtual IList<User> GetByIds(Guid[] userIds)
        {
            if (userIds == null || userIds.Length == 0)
                return new List<User>();

            var query = from u in this.userRepository.Table
                        where userIds.Contains(u.RowId)
                        select u;
            var users = query.ToList();
            //sort by passed identifiers
            var sortedUsers = new List<User>();
            foreach (Guid id in userIds)
            {
                var user = users.Find(x => x.RowId == id);
                if (user != null)
                    sortedUsers.Add(user);
            }
            return sortedUsers;
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>user</returns>
        public virtual User GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in this.userRepository.Table
                        orderby c.RowId
                        where c.Username == username
                        select c;
            var user = query.FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Get users by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>users</returns>
        public virtual IList<User> GetUsersByLanguageId(Guid languageId)
        {
            var query = this.userRepository.Table;
            if (!languageId.IsEmpty())
                query = query.Where(c => c.LanguageId.HasValue && c.LanguageId.Value == languageId);
            else
                query = query.Where(c => !c.LanguageId.HasValue);
            query = query.OrderBy(c => c.RowId);
            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Get users by currency identifier
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>users</returns>
        public virtual IList<User> GetUsersByCurrencyId(Guid currencyId)
        {
            var query = this.userRepository.Table;
            if (!currencyId.IsEmpty())
                query = query.Where(c => c.CurrencyId.HasValue && c.CurrencyId.Value == currencyId);
            else
                query = query.Where(c => !c.CurrencyId.HasValue);
            query = query.OrderBy(c => c.RowId);
            var users = query.ToList();
            return users;
        }

        /// <summary>
        /// Check whether the entity is unique
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>user</returns>
        public bool IsUnique(object uniqueValue)
        {
            var isUnique = false;

            if (uniqueValue != null)
            {
                string userName = uniqueValue.ToString().Trim();
                if (CommonHelper.IsValidEmail(userName))
                {
                    var user = this.GetByUsername(userName);
                    if (user == null)
                    {
                        isUnique = true;
                    }
                }
            }

            return isUnique;
        }

        /// <summary>
        /// Insert a user
        /// </summary>
        /// <param name="user">user</param>
        public virtual void Insert(User user)
        {
            Guard.IsNotNull(user, "User");

            this.userRepository.Insert(user);

            //event notification
            this.eventPublisher.EntityInserted(user);
        }
        
        /// <summary>
        /// Updates the user
        /// </summary>
        /// <param name="user">user</param>
        public virtual void Update(User user)
        {
            Guard.IsNotNull(user, "User");

            this.userRepository.Update(user);

            //event notification
            this.eventPublisher.EntityUpdated(user);
        }

        #endregion
        
        #region user roles

        /// <summary>
        /// Delete a user role
        /// </summary>
        /// <param name="userRole">user role</param>
        public virtual void DeleteUserRole(UserRole userRole)
        {
            Guard.IsNotNull(userRole, "UserRole");

            if (userRole.IsSystemRole)
                throw new SiteException("System role could not be deleted");

            this.userRoleRepository.Delete(userRole);

            this.cacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(userRole);
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="userRoleId">user role identifier</param>
        /// <returns>user role</returns>
        public virtual UserRole GetUserRoleById(Guid userRoleId)
        {
            if (userRoleId.IsEmpty())
                return null;

            string key = string.Format(USERROLES_BY_ID_KEY, userRoleId);
            return this.cacheManager.Get(key, () =>
            {
                var userRole = this.userRoleRepository.GetById(userRoleId);
                return userRole;
            });
        }

        /// <summary>
        /// Gets a user role
        /// </summary>
        /// <param name="systemName">user role system name</param>
        /// <returns>user role</returns>
        public virtual UserRole GetUserRoleBySystemName(string systemName)
        {
            if (String.IsNullOrWhiteSpace(systemName))
                return null;

            string key = string.Format(USERROLES_BY_SYSTEMNAME_KEY, systemName);
            return this.cacheManager.Get(key, () =>
            {
                var query = from cr in this.userRoleRepository.Table
                            orderby cr.RowId
                            where cr.SystemName == systemName
                            select cr;
                var userRole = query.FirstOrDefault();
                return userRole;
            });
        }

        /// <summary>
        /// Gets all user roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>user role collection</returns>
        public virtual IList<UserRole> GetAllUserRoles(bool showHidden = false)
        {
            string key = string.Format(USERROLES_ALL_KEY, showHidden);
            return this.cacheManager.Get(key, () =>
            {
                var query = from cr in this.userRoleRepository.Table
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                var userRoles = query.ToList();
                return userRoles;
            });
        }
        
        /// <summary>
        /// Inserts a user role
        /// </summary>
        /// <param name="userRole">user role</param>
        public virtual void InsertUserRole(UserRole userRole)
        {
            Guard.IsNotNull(userRole, "userRole");

            this.userRoleRepository.Insert(userRole);

            this.cacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(userRole);
        }

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="userRole">user role</param>
        public virtual void UpdateUserRole(UserRole userRole)
        {
            Guard.IsNotNull(userRole, "UserRole");
            if (userRole == null)
                throw new ArgumentNullException("userRole");

            this.userRoleRepository.Update(userRole);

            this.cacheManager.RemoveByPattern(USERROLES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(userRole);
        }

        #endregion

        #endregion
    }
}