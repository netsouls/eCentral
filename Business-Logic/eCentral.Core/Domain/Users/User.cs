using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Domain.Logging;

namespace eCentral.Core.Domain.Users
{
    /// <summary>
    /// Represents a user
    /// </summary>
    public partial class User : BaseEntity, IAuditHistory
    {
        private ICollection<UserRole> _userRoles;
        private ICollection<UserLoginHistory> _userLoginHistory;
        private ICollection<ActivityLog> _activityLog;
        
        public User()
        {
            this.UserGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets or sets the user Guid
        /// </summary>
        public virtual Guid UserGuid { get; set; }

        public virtual string Username { get; set; }
        public virtual string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the users current publishing status
        /// </summary>
        public virtual int CurrentPublishingStatusId { get; set; }

        /// <summary>
        /// Gets or sets the language identifier
        /// </summary>
        public virtual Guid? LanguageId { get; set; }

        /// <summary>
        /// Gets or sets the currency identifier
        /// </summary>
        public virtual Guid? CurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the time zone identifier
        /// </summary>
        public virtual string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the number of invalid password attempts
        /// </summary>
        public virtual int FailedPasswordAttemptCount { get; set; }

        /// <summary>
        /// Gets or sets the date and time for the last password change
        /// </summary>
        public virtual DateTime? LastPasswordChangeDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public virtual DateTime? LastActivityDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        #region Custom properties

        /// <summary>
        /// Gets or set the Current publishing status
        /// </summary>
        public virtual PublishingStatus CurrentPublishingStatus
        {
            get
            {
                return (PublishingStatus)this.CurrentPublishingStatusId;
            }
            set
            {
                this.CurrentPublishingStatusId = (int)value;
            }
        }

        #endregion

        #region Navigation properties

        /// <summary>
        /// Gets or sets the language
        /// </summary>
        public virtual Language Language { get; set; }

        /// <summary>
        /// Gets or sets the currency
        /// </summary>
        public virtual Currency Currency { get; set; }

        /// <summary>
        /// Gets or sets the user roles
        /// </summary>
        public virtual ICollection<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new List<UserRole>()); }
            protected set { _userRoles = value; }
        }

        /// <summary>
        /// Gets the user login history
        /// </summary>
        public virtual ICollection<UserLoginHistory> UserLoginHistory
        {
            get { return _userLoginHistory ?? (_userLoginHistory = new List<UserLoginHistory>()); }
            protected set { _userLoginHistory = value; }
        }

        private ICollection<ActivityLog> _auditHistory;

        /// <summary>
        /// Gets the activity log
        /// </summary>
        public virtual ICollection<ActivityLog> AuditHistory
        {
            get { return _auditHistory ?? (_auditHistory = new List<ActivityLog>()); }
            set { _auditHistory = value; }
        }

        /// <summary>
        /// Gets this user activity log
        /// </summary>
        public virtual ICollection<ActivityLog> ActivityLog
        {
            get { return _activityLog ?? (_activityLog = new List<ActivityLog>()); }
            protected set { _activityLog = value; }            
        }
        
        #endregion

        #region To String

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // set values 
            builder.Append(string.Format("Username: [{0}]", this.Username.Trim())).Append
             (string.Format(", Roles: [{0}]", this.UserRoles.Select(ur => ur.Name).ToDelimitedString(", ")));

            return builder.ToString();
        }

        #endregion
    }
}