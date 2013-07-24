using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Common
{
    public class SeoSettings : ISettings
    {
        public string PageTitleSeparator { get; set; }
        
        public string DefaultTitle { get; set; }        
    }
}