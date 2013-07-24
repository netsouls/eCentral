using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Common
{
    public class CommonSettings : ISettings
    {
        public bool UseStoredProceduresIfSupported { get; set; }

        public bool EnableHttpCompression { get; set; }
    }
}