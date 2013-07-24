using System.Collections.Generic;
using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Messages
{
    public class SmsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets system names of active SMS providers
        /// </summary>
        public List<string> ActiveSmsProviderSystemNames { get; set; }
    }
}
