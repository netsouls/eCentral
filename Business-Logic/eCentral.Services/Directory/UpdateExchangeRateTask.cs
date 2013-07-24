using System;
using eCentral.Core.Configuration;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Infrastructure;
using eCentral.Services.Configuration;
using eCentral.Services.Tasks;

namespace eCentral.Services.Directory
{
    /// <summary>
    /// Represents a task for updating exchange rates
    /// </summary>
    public partial class UpdateExchangeRateTask : ITask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var currencySettings = EngineContext.Current.Resolve<IConfigurationProvider<CurrencySettings>>().Settings;
            if (!currencySettings.AutoUpdateEnabled)
                return;

            long lastUpdateTimeTicks = currencySettings.LastUpdateTime;
            DateTime lastUpdateTime  = DateTime.FromBinary(lastUpdateTimeTicks);
            lastUpdateTime           = DateTime.SpecifyKind(lastUpdateTime, DateTimeKind.Utc);

            if (lastUpdateTime.AddHours(1) < DateTime.UtcNow)
            {
                //update rates each one hour
                var currencyService = EngineContext.Current.Resolve<ICurrencyService>();
                var exchangeRates = currencyService.GetCurrencyLiveRates(currencyService.GetById(currencySettings.PrimaryExchangeRateCurrencyId).CurrencyCode);

                foreach (var exchageRate in exchangeRates)
                {
                    var currency = currencyService.GetByCode(exchageRate.CurrencyCode);
                    if (currency != null)
                    {
                        currency.Rate      = exchageRate.Rate;
                        currency.UpdatedOn = DateTime.UtcNow;
                        currencyService.Update(currency);
                    }
                }

                //save new update time value
                currencySettings.LastUpdateTime = DateTime.UtcNow.ToBinary();
                var settingService = EngineContext.Current.Resolve<ISettingService>();
                settingService.Save(currencySettings);
            }
        }
    }
}
