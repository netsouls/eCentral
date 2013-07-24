using System;
using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Directory
{
    public class CurrencySettings : ISettings
    {
        public Guid PrimaryCurrencyId { get; set; }
        public Guid PrimaryExchangeRateCurrencyId { get; set; }
        public string ActiveExchangeRateProviderSystemName { get; set; }
        public bool AutoUpdateEnabled { get; set; }
        public long LastUpdateTime { get; set; }
    }
}