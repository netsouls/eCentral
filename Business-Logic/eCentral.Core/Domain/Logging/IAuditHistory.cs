using System.Collections.Generic;

namespace eCentral.Core.Domain.Logging
{
    /// <summary>
    /// Audit History interface
    /// </summary>
    public interface IAuditHistory : IPublishingStatus
    {
        /// <summary>
        /// Gets the activity log
        /// </summary>
        ICollection<ActivityLog> AuditHistory { get; }
    }
}
