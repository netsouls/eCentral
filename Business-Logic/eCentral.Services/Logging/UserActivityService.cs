using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Logging;

namespace eCentral.Services.Logging
{
    /// <summary>
    /// User activity service
    /// </summary>
    public class UserActivityService : IUserActivityService
    {
        #region Constants
        
        private const string ACTIVITYTYPE_ALL_KEY     = "eCentral.activitytype.all";
        private const string ACTIVITYTYPE_BY_ID_KEY   = "eCentral.activitytype.id-{0}";
        private const string ACTIVITYTYPE_PATTERN_KEY = "eCentral.activitytype.";
        
        #endregion

        #region Fields

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager cacheManager;
        private readonly IRepository<ActivityLog> activityLogRepository;
        private readonly IRepository<ActivityLogType> activityLogTypeRepository;
        private readonly IRepository<UserLoginHistory> userLoginHistoryRepository;
        private readonly IWorkContext workContext;
        private readonly IWebHelper webHelper;
        #endregion
        
        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="activityLogRepository">Activity log repository</param>
        /// <param name="activityLogTypeRepository">Activity log type repository</param>
        /// <param name="workContext">Work context</param>
        public UserActivityService(ICacheManager cacheManager,
            IRepository<ActivityLog> activityLogRepository,
            IRepository<ActivityLogType> activityLogTypeRepository, 
            IRepository<UserLoginHistory> userLoginHistoryRepository,
            IWorkContext workContext, IWebHelper webHelper)
        {
            this.cacheManager               = cacheManager;
            this.activityLogRepository      = activityLogRepository;
            this.activityLogTypeRepository  = activityLogTypeRepository;
            this.userLoginHistoryRepository = userLoginHistoryRepository;
            this.workContext                = workContext;
            this.webHelper                  = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void InsertActivityType(ActivityLogType activityLogType)
        {
            Guard.IsNotNull(activityLogType, "activityLogType");

            activityLogTypeRepository.Insert(activityLogType);
            cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }

        /// <summary>
        /// Updates an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type item</param>
        public virtual void UpdateActivityType(ActivityLogType activityLogType)
        {
            Guard.IsNotNull(activityLogType, "activityLogType");

            activityLogTypeRepository.Update(activityLogType);
            cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }
                
        /// <summary>
        /// Deletes an activity log type item
        /// </summary>
        /// <param name="activityLogType">Activity log type</param>
        public virtual void DeleteActivityType(ActivityLogType activityLogType)
        {
            Guard.IsNotNull(activityLogType, "activityLogType");

            activityLogTypeRepository.Delete(activityLogType);
            cacheManager.RemoveByPattern(ACTIVITYTYPE_PATTERN_KEY);
        }
        
        /// <summary>
        /// Gets all activity log type items
        /// </summary>
        /// <returns>Activity log type collection</returns>
        public virtual IList<ActivityLogType> GetAllActivityTypes()
        {
            string key = string.Format(ACTIVITYTYPE_ALL_KEY);
            return cacheManager.Get(key, () =>
            {
                var query = from alt in activityLogTypeRepository.Table
                            orderby alt.Name
                            select alt;
                var activityLogTypes = query.ToList();
                return activityLogTypes;
            });
        }
        
        /// <summary>
        /// Gets an activity log type item
        /// </summary>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <returns>Activity log type item</returns>
        public virtual ActivityLogType GetActivityTypeById(Guid activityLogTypeId)
        {
            if (activityLogTypeId.IsEmpty())
                return null;

            string key = string.Format(ACTIVITYTYPE_BY_ID_KEY, activityLogTypeId);
            return cacheManager.Get(key, () =>
            {
                return activityLogTypeRepository.GetById(activityLogTypeId);
            });
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog InsertActivity(string systemKeyword, string version, string comment)
        {
            return InsertActivity(systemKeyword, version, comment, new object[0]);
        }

        /// <summary>
        /// Inserts an activity log item
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <param name="comment">The activity comment</param>
        /// <param name="commentParams">The activity comment parameters for string.Format() function.</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog InsertActivity(string systemKeyword, string version,
            string comment, params object[] commentParams)
        {
            if (workContext.CurrentUser == null)
                return null;

            var activityTypes = GetAllActivityTypes();
            var activityType = activityTypes.ToList().Find(at => at.SystemKeyword == systemKeyword);
            if (activityType == null)
                return null;

            comment = CommonHelper.EnsureNotNull(comment);
            comment = string.Format(comment, commentParams);
            comment = CommonHelper.EnsureMaximumLength(comment, 4000);
            
            var activity = new ActivityLog()
            {
                ActivityLogType = activityType, ActivityLogTypeId = activityType.RowId,
                User = workContext.CurrentUser, UserId = workContext.CurrentUser.RowId,
                Comments = comment, 
                VersionControl = version,
                CreatedOn = DateTime.UtcNow
            };

            activityLogRepository.Insert(activity);

            return activity;
        }


        /// <summary>
        /// Insets an user login activity
        /// </summary>
        /// <returns></returns>
        public virtual UserLoginHistory LoginHistory( )
        {
            if (workContext.CurrentUser == null)
                return null;

            // we need to get the user history for current date
            DateTime currentDate = DateTime.UtcNow;
            string ipAddress = webHelper.GetCurrentIpAddress();

            var loginHistory             = userLoginHistoryRepository.Single(data => (
                    data.UserId          == workContext.CurrentUser.RowId && 
                    data.LoginDate.Month == currentDate.Month && 
                    data.LoginDate.Year  == currentDate.Year && 
                    data.LoginDate.Day   == currentDate.Day));

            if (loginHistory != null)
            {
                // update the record
                loginHistory.Count     = loginHistory.Count + 1;
                loginHistory.IPAddress = ipAddress;
                loginHistory.LoginDate = currentDate;

                userLoginHistoryRepository.Update(loginHistory);
            }
            else
            {
                loginHistory = new UserLoginHistory 
                { 
                    UserId    = workContext.CurrentUser.RowId, 
                    Count     = 1 , 
                    IPAddress = ipAddress, 
                    LoginDate = currentDate
                };

                userLoginHistoryRepository.Insert(loginHistory);
            }

            return loginHistory;
        }

        /// <summary>
        /// Deletes an activity log item
        /// </summary>
        /// <param name="activityLog">Activity log type</param>
        public virtual void DeleteActivity(ActivityLog activityLog)
        {
            Guard.IsNotNull(activityLog, "activityLog");

            activityLogRepository.Delete(activityLog);
        }

        /// <summary>
        /// Gets all activity log items
        /// </summary>
        /// <param name="createdOnFrom">Log item creation from; null to load all customers</param>
        /// <param name="createdOnTo">Log item creation to; null to load all customers</param>
        /// <param name="email">Customer Email</param>
        /// <param name="username">Customer username</param>
        /// <param name="activityLogTypeId">Activity log type identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Activity log collection</returns>
        public virtual PagedList<ActivityLog> GetAllActivities(DateTime? createdOnFrom,
            DateTime? createdOnTo, string username, Guid activityLogTypeId,
            int pageIndex, int pageSize)
        {
            var query = activityLogRepository.Table;
            if (createdOnFrom.HasValue)
                query = query.Where(al => createdOnFrom.Value <= al.CreatedOn);
            if (createdOnTo.HasValue)
                query = query.Where(al => createdOnTo.Value >= al.CreatedOn);
            if (!activityLogTypeId.IsEmpty())
                query = query.Where(al => activityLogTypeId == al.ActivityLogTypeId);
            
            if (!String.IsNullOrEmpty(username))
            {
                query = query.Where(c => c.User.Username.Contains(username));
            }

            query = query.OrderByDescending(al => al.CreatedOn);

            var activityLog = new PagedList<ActivityLog>(query, pageIndex, pageSize);
            return activityLog;
        }
        
        /// <summary>
        /// Gets an activity log item
        /// </summary>
        /// <param name="activityLogId">Activity log identifier</param>
        /// <returns>Activity log item</returns>
        public virtual ActivityLog GetActivityById(Guid activityLogId)
        {
            if (activityLogId.IsEmpty())
                return null;
            
            var query = from al in activityLogRepository.Table
                        where al.RowId == activityLogId
                        select al;
            var activityLog = query.SingleOrDefault();
            return activityLog;
        }

        /// <summary>
        /// Clears activity log
        /// </summary>
        public virtual void ClearAllActivities()
        {            
            var activityLog = activityLogRepository.Table.ToList();
            foreach (var activityLogItem in activityLog)
                activityLogRepository.Delete(activityLogItem);
        }
        #endregion

    }
}
