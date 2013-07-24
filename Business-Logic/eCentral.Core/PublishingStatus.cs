using eCentral.Core.Attributes;

namespace eCentral.Core
{
    /// <summary>
    /// Publishing status of data
    /// </summary>
    public enum PublishingStatus : short
    {
        [FriendlyNameString("All")]
        All = 0,

        /// <summary>
        /// Draft Mode
        /// </summary>
        [FriendlyNameString("Draft")]
        Draft = 1,

        /// <summary>
        // Pending Approval
        /// </summary>
        [FriendlyNameString("Pending Approval")]
        PendingApproval = 2,

        /// <summary>
        // Active
        /// </summary>
        [FriendlyNameString("Active")]
        Active = 3,

        /// <summary>
        // Archived
        /// </summary>
        [FriendlyNameString("Archived")]
        Archived = 4,

        /// <summary>
        // Pending Deletion
        /// </summary>
        [FriendlyNameString("Pending Deletion")]
        PendingDeletion = 5
    }
}
