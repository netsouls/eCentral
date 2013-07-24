using System;
using eCentral.Core.Domain.Users;

namespace eCentral.Core.Domain.Logging
{
    /// <summary>
    /// Represents an user login log record
    /// </summary>
    public partial class UserLoginHistory : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public virtual Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user IP Address
        /// </summary>
        public virtual string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the login count comment
        /// </summary>
        public virtual int Count{ get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime LoginDate { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the user
        /// </summary>
        public virtual User User { get; set; }

        #endregion
    }
}
