using System.Text;
using eCentral.Core.Configuration;

namespace eCentral.Core.Domain
{
    public class SiteInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a site name
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// Gets or sets the site name blurb
        /// </summary>
        public string SiteNameBlurb { get; set; }

        /// <summary>
        /// Gets or sets a site URL
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether site is closed
        /// </summary>
        public bool SiteClosed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether web administrators can visit a closed site
        /// </summary>
        public bool SiteClosedAllowForWebAdmins { get; set; }

        /// <summary>
        /// Gets or sets the state of the application
        /// </summary>
        public ApplicationState ApplicationState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether mini profiler should be displayed in public site (used for debugging)
        /// </summary>
        public bool DisplayMiniProfilerInPublicSite { get; set; }

        /// <summary>
        /// Gets or sets the support email address
        /// </summary>
        public string SupportEmailAddress { get; set; }

        #region To String

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // set values 
            builder.AppendFormat("SiteName: [{0}]", this.SiteName.Trim())
                .AppendFormat(", SiteNameBlurb: [{0}]", this.SiteNameBlurb.Trim())
                .AppendFormat(", SiteUrl: [{0}]", this.SiteUrl.Trim())
                .AppendFormat(", SupportEmailAddress: [{0}]", this.SupportEmailAddress.Trim())
                .AppendFormat(", ApplicationState: [{0}]", this.ApplicationState.ToString())
                .AppendFormat(", SiteClosed: [{0}]", this.SiteClosed.ToString())
                .AppendFormat(", SiteClosedAllowForWebAdmins: [{0}]", this.SiteClosedAllowForWebAdmins.ToString())
                .AppendFormat(", DisplayMiniProfilerInPublicSite: [{0}]", this.DisplayMiniProfilerInPublicSite.ToString());

            return builder.ToString();
        }

        #endregion
    }

    /// <summary>
    /// State the application is deployed at
    /// </summary>
    public enum ApplicationState : int
    {
        /// <summary>
        /// When the application is on Development server
        /// </summary>        
        Development = 10,

        /// <summary>
        /// When the application is on staging Server 
        /// </summary>
        Staging = 20,

        /// <summary>
        /// When the application is deployed on the production server
        /// </summary>
        Production = 30
    }
}
