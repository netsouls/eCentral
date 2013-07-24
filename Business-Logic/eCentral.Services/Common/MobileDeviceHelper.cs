using System.Web;
using eCentral.Core;
using eCentral.Core.Domain;

namespace eCentral.Services.Common
{
    /// <summary>
    /// Mobile device helper
    /// </summary>
    public partial class MobileDeviceHelper : IMobileDeviceHelper
    {
        #region Fields

        private readonly SiteInformationSettings siteInformationSettings;
        private readonly IWorkContext workContext;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="storeInformationSettings">Store information settings</param>
        /// <param name="workContext">Work context</param>
        public MobileDeviceHelper(SiteInformationSettings siteInformationSettings,
            IWorkContext workContext)
        {
            this.siteInformationSettings = siteInformationSettings;
            this.workContext             = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a value indicating whether request is made by a mobile device
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <returns>Result</returns>
        public virtual bool IsMobileDevice(HttpContextBase httpContext)
        {
            return httpContext.Request.Browser.IsMobileDevice;
        }

        /// <summary>
        /// Returns a value indicating whether mobile devices support is enabled
        /// </summary>
        public virtual bool MobileDevicesSupported()
        {
            return true;
        }

        #endregion
    }
}