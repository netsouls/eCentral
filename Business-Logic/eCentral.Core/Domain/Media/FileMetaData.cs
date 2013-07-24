
namespace eCentral.Core.Domain.Media
{
    /// <summary>
    /// Represents a picture
    /// </summary>
    public partial class FileMetaData : BaseEntity
    {
        /// <summary>
        /// Gets or sets the picture binary
        /// </summary>
        public virtual byte[] BinaryData { get; set; }

        /// <summary>
        /// Gets or sets the picture mime type
        /// </summary>
        public virtual string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the SEO friednly filename of the picture
        /// </summary>
        public virtual string SeoFilename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the picture is new
        /// </summary>
        public virtual bool IsNew { get; set; }

        /// <summary>
        /// Gets or Sets the file type
        /// </summary>
        public virtual int FileType { get; set; }
    }
}
