using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Cms
{
    public class ContentSettings : ISettings
    {
        public string FileStorageVirtualPath { get; set; }
    }
}