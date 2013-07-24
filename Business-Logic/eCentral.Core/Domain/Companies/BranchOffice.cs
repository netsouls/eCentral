using System;
using System.Collections.Generic;
using System.Text;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Logging;

namespace eCentral.Core.Domain.Companies
{
    /// <summary>
    /// Represents a branch office
    /// </summary>
    public partial class BranchOffice : BaseEntity, IAuditHistory
    {
        public virtual string BranchName { get; set; }

        public virtual string Abbreviation { get; set; }

        public virtual Guid? AddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the current publishing status
        /// </summary>
        public virtual int CurrentPublishingStatusId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity updated
        /// </summary>
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
            builder.Append(string.Format("Name: [{0}]", this.BranchName.Trim()))
                .AppendFormat("Abbreviation: [{0}]", this.Abbreviation.Trim());

            if (this.Address != null)
                builder.AppendFormat(", {0}", this.Address.ToString());

            return builder.ToString(); 
        }

        #endregion
    }
}