using System;
using System.Linq;
using eCentral.Core.Data;
using eCentral.Core.Domain.Users;
using eCentral.Services.Helpers;

namespace eCentral.Services.Users
{
    /// <summary>
    /// User report service
    /// </summary>
    public partial class UserReportService : IUserReportService
    {
        #region Fields

        private readonly IRepository<User> userRepository;
        private readonly IUserService userService;
        private readonly IDateTimeHelper dateTimeHelper;

        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="userRepository">User repository</param>
        /// <param name="userService">User service</param>
        /// <param name="dateTimeHelper">Date time helper</param>
        public UserReportService(IRepository<User> userRepository,
            IUserService userService,
            IDateTimeHelper dateTimeHelper)
        {
            this.userRepository = userRepository;
            this.userService    = userService;
            this.dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a report of users registered in the last days
        /// </summary>
        /// <param name="days">Users registered in the last days</param>
        /// <returns>Number of registered users</returns>
        public virtual int GetRegisteredUsersReport(int days)
        {
            DateTime date = dateTimeHelper.ConvertToUserTime(DateTime.Now).AddDays(-days);

            var registeredRole = userService.GetUserRoleBySystemName(SystemUserRoleNames.Users);
            if (registeredRole == null)
                return 0;

            var query = from u in userRepository.Table
                        from ur in u.UserRoles
                        where ur.RowId == registeredRole.RowId &&
                        u.CreatedOn >= date
                        select u;
            int count = query.Count();
            return count;
        }

        #endregion
    }
}
