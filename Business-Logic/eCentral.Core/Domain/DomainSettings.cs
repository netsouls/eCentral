using eCentral.Core.Configuration;

namespace eCentral.Core.Domain
{
    public class DomainSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether www status on the domain
        /// </summary>
        public WWWStatus wwwStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether secure socket layer is required or not on the domain
        /// </summary>
        public SSLStatus sslStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to redirect non-lower case url to lower-case url 
        /// </summary>
        public UrlLowercaseStatus urlLowercaseStatus { get; set;  }
    }

    /// <summary>
    /// Check for WWW status
    /// </summary>
    public enum WWWStatus
    {
        Ignore,
        Require,
        Remove
    }

    /// <summary>
    /// Check for URL Case
    /// </summary>
    public enum UrlLowercaseStatus
    {
        Ignore, 
        Require
    }

    public enum SSLStatus
    {
        /// <summary>
        /// Page should be secured
        /// </summary>
        Require,

        /// <summary>
        /// Ignore the ssl request
        /// </summary>
        Ignore,
    }
}
