using System.Net;
using eCentral.Core.Domain;
using eCentral.Core.Infrastructure;
using eCentral.Services.Tasks;

namespace eCentral.Services.Common
{
    /// <summary>
    /// Represents a task for keeping the site alive
    /// </summary>
    public partial class KeepAliveTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var siteInformationSettings = EngineContext.Current.Resolve<SiteInformationSettings>();
            string url = siteInformationSettings.SiteUrl + "keepalive";
            using (var wc = new WebClient())
            {
                wc.DownloadString(url);
            }
        }
    }
}
