using System;
using eCentral.Core.Domain.Users;

namespace eCentral.Core.Domain.Logging
{
    /// <summary>
    /// Represents an audit history log record
    /// </summary>
    public partial class AuditHistory : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the type identifier
        /// </summary>
        public virtual Guid ActivityLogTypeId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the old version
        /// </summary>
        public string VersionControl { get; set; }

        /// <summary>
        /// Gets or Sets the comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the activity log type
        /// </summary>
        public virtual ActivityLogType ActivityLogType { get; set; }

        /// <summary>
        /// Gets the user
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}
