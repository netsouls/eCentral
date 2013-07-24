using System;
using System.Collections.Generic;
using System.Text;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Logging;

namespace eCentral.Core.Domain.Clients
{
    /// <summary>
    /// Represents a client
    /// </summary>
    public partial class Client : BaseEntity, IAuditHistory
    {
        public virtual string ClientName { get; set; }

        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the current publishing status
        /// </summary>
        public virtual int CurrentPublishingStatusId { get; set; }

        public virtual Guid? AddressId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        public virtual DateTime UpdatedOn { get; set; }

        #region Custom properties

        /// <summary>
        /// Gets the VAT number status
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
        /// Gets or sets the Address
        /// </summary>
        public virtual Address Address { get; set; }

        private ICollection<ActivityLog> _auditHistory;

        /// <summary>
        /// Gets the activity log
        /// </summary>
        public virtual ICollection<ActivityLog> AuditHistory
        {
            get { return _auditHistory ?? (_auditHistory = new List<ActivityLog>()); }
            protected set { _auditHistory = value; }
        }

        #endregion

        #region To String

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // set values 
            builder.Append(string.Format("Name: [{0}]", this.ClientName.Trim())).Append
             (string.Format(", Email: [{0}]", this.Email.Trim()));

            if (this.Address != null)
                builder.AppendFormat(", {0}", this.Address.ToString());

            return builder.ToString();
        }
        #endregion
    }
}