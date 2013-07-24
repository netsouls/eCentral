
namespace eCentral.Core.Domain
{
    /// <summary>
    /// Publishing Status interface
    /// </summary>
    public interface IPublishingStatus
    {
        /// <summary>
        /// Gets or sets the current publishing status id
        /// </summary>
        int CurrentPublishingStatusId { get; set; }

        /// <summary>
        /// Gets the current publishing status
        /// </summary>
        PublishingStatus CurrentPublishingStatus { get; set;}
    }
}
