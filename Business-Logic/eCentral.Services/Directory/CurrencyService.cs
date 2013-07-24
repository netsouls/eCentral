using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Directory;
using eCentral.Services.Events;
using eCentral.Core.Plugins;
using eCentral.Services.Users;

namespace eCentral.Services.Directory
{
    /// <summary>
    /// Currency service
    /// </summary>
    public partial class CurrencyService : ICurrencyService
    {
        #region Constants
        private const string CURRENCIES_ALL_KEY     = "eCentral.currency.all-{0}";
        private const string CURRENCIES_BY_ID_KEY   = "eCentral.currency.id-{0}";
        private const string CURRENCIES_PATTERN_KEY = "eCentral.currency.";
        #endregion

        #region Fields

        private readonly IRepository<Currency> currencyRepository;
        private readonly ICacheManager cacheManager;
        private readonly IUserService userService;
        private readonly CurrencySettings currencySettings;
        private readonly IPluginFinder pluginFinder;
        private readonly IEventPublisher eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="currencyRepository">Currency repository</param>
        /// <param name="customerService">Customer service</param>
        /// <param name="currencySettings">Currency settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="eventPublisher">Event published</param>
        public CurrencyService(ICacheManager cacheManager,
            IRepository<Currency> currencyRepository,
            IUserService userService,
            CurrencySettings currencySettings,
            IPluginFinder pluginFinder,
            IEventPublisher eventPublisher)
        {
            this.cacheManager       = cacheManager;
            this.currencyRepository = currencyRepository;
            this.userService        = userService;
            this.currencySettings   = currencySettings;
            this.pluginFinder       = pluginFinder;
            this.eventPublisher     = eventPublisher;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public virtual IList<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            var exchangeRateProvider = LoadActiveExchangeRateProvider();
            return exchangeRateProvider.GetCurrencyLiveRates(exchangeRateCurrencyCode);
        }

        /// <summary>
        /// Deletes currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void Delete(Currency currency)
        {
            Guard.IsNotNull(currency, "currency");

            //update appropriate users (their currency)
            //it can take a lot of time if you have thousands of associated customers
            var users = this.userService.GetUsersByCurrencyId(currency.RowId);
            foreach (var user in users)
            {
                user.CurrencyId = Guid.Empty;
                this.userService.Update(user);
            }

            this.currencyRepository.Delete(currency);

            this.cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(currency);
        }

        /// <summary>
        /// Gets a currency
        /// </summary>
        /// <param name="currencyId">Currency identifier</param>
        /// <returns>Currency</returns>
        public virtual Currency GetById(Guid currencyId)
        {
            if (currencyId.IsEmpty())
                return null;

            string key = string.Format(CURRENCIES_BY_ID_KEY, currencyId);
            return this.cacheManager.Get(key, () => this.currencyRepository.GetById(currencyId));
        }

        /// <summary>
        /// Gets a currency by code
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <returns>Currency</returns>
        public virtual Currency GetByCode(string currencyCode)
        {
            if (String.IsNullOrEmpty(currencyCode))
                return null;
            return GetAll(true).FirstOrDefault(c => c.CurrencyCode.ToLower() == currencyCode.ToLower());
        }

        /// <summary>
        /// Gets all currencies
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Currency collection</returns>
        public virtual IList<Currency> GetAll(bool showHidden = false)
        {
            string key = string.Format(CURRENCIES_ALL_KEY, showHidden);
            return this.cacheManager.Get(key, () =>
            {
                var query = from c in this.currencyRepository.Table
                            orderby c.DisplayOrder
                            where showHidden || c.Published
                            select c;
                var currencies = query.ToList();
                return currencies;
            });
        }

        /// <summary>
        /// Inserts a currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void Insert(Currency currency)
        {
            Guard.IsNotNull(currency, "currency");

            this.currencyRepository.Insert(currency);

            this.cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(currency);
        }

        /// <summary>
        /// Updates the currency
        /// </summary>
        /// <param name="currency">Currency</param>
        public virtual void Update(Currency currency)
        {
            Guard.IsNotNull(currency, "currency");

            this.currencyRepository.Update(currency);

            this.cacheManager.RemoveByPattern(CURRENCIES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(currency);
        }



        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="exchangeRate">Currency exchange rate</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertCurrency(decimal amount, decimal exchangeRate)
        {
            if (amount != decimal.Zero && exchangeRate != decimal.Zero)
                return amount * exchangeRate;
            return decimal.Zero;
        }

        /// <summary>
        /// Converts currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertCurrency(decimal amount, Currency sourceCurrencyCode, Currency targetCurrencyCode)
        {
            decimal result = amount;
            if (sourceCurrencyCode.RowId == targetCurrencyCode.RowId)
                return result;
            if (result != decimal.Zero && sourceCurrencyCode.RowId != targetCurrencyCode.RowId)
            {
                result = ConvertToPrimaryExchangeRateCurrency(result, sourceCurrencyCode);
                result = ConvertFromPrimaryExchangeRateCurrency(result, targetCurrencyCode);
            }
            return result;
        }

        /// <summary>
        /// Converts to primary exchange rate currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertToPrimaryExchangeRateCurrency(decimal amount, Currency sourceCurrencyCode)
        {
            decimal result = amount;
            var primaryExchangeRateCurrency = GetById(this.currencySettings.PrimaryExchangeRateCurrencyId);
            if (result != decimal.Zero && sourceCurrencyCode.RowId != primaryExchangeRateCurrency.RowId)
            {
                decimal exchangeRate = sourceCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new SiteException(string.Format("Exchange rate not found for currency [{0}]", sourceCurrencyCode.Name));
                result = result / exchangeRate;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary exchange rate currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertFromPrimaryExchangeRateCurrency(decimal amount, Currency targetCurrencyCode)
        {
            decimal result = amount;
            var primaryExchangeRateCurrency = GetById(this.currencySettings.PrimaryExchangeRateCurrencyId);
            if (result != decimal.Zero && targetCurrencyCode.RowId != primaryExchangeRateCurrency.RowId)
            {
                decimal exchangeRate = targetCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new SiteException(string.Format("Exchange rate not found for currency [{0}]", targetCurrencyCode.Name));
                result = result * exchangeRate;
            }
            return result;
        }

        /// <summary>
        /// Converts to primary store currency 
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="sourceCurrencyCode">Source currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertToPrimaryStoreCurrency(decimal amount, Currency sourceCurrencyCode)
        {
            decimal result = amount;
            var primaryStoreCurrency = GetById(this.currencySettings.PrimaryCurrencyId);
            if (result != decimal.Zero && sourceCurrencyCode.RowId != primaryStoreCurrency.RowId)
            {
                decimal exchangeRate = sourceCurrencyCode.Rate;
                if (exchangeRate == decimal.Zero)
                    throw new SiteException(string.Format("Exchange rate not found for currency [{0}]", sourceCurrencyCode.Name));
                result = result / exchangeRate;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary store currency
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="targetCurrencyCode">Target currency code</param>
        /// <returns>Converted value</returns>
        public virtual decimal ConvertFromPrimaryStoreCurrency(decimal amount, Currency targetCurrencyCode)
        {
            decimal result = amount;
            var primaryStoreCurrency = GetById(this.currencySettings.PrimaryCurrencyId);
            result = ConvertCurrency(amount, primaryStoreCurrency, targetCurrencyCode);
            return result;
        }
       

        /// <summary>
        /// Load active exchange rate provider
        /// </summary>
        /// <returns>Active exchange rate provider</returns>
        public virtual IExchangeRateProvider LoadActiveExchangeRateProvider()
        {
            var exchangeRateProvider = LoadExchangeRateProviderBySystemName(this.currencySettings.ActiveExchangeRateProviderSystemName);
            if (exchangeRateProvider == null)
                exchangeRateProvider = LoadAllExchangeRateProviders().FirstOrDefault();
            return exchangeRateProvider;
        }

        /// <summary>
        /// Load exchange rate provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found exchange rate provider</returns>
        public virtual IExchangeRateProvider LoadExchangeRateProviderBySystemName(string systemName)
        {
            var descriptor = this.pluginFinder.GetPluginDescriptorBySystemName<IExchangeRateProvider>(systemName);
            if (descriptor != null)
                return descriptor.Instance<IExchangeRateProvider>();

            return null;
        }

        /// <summary>
        /// Load all exchange rate providers
        /// </summary>
        /// <returns>Exchange rate providers</returns>
        public virtual IList<IExchangeRateProvider> LoadAllExchangeRateProviders()
        {
            var exchangeRateProviders = this.pluginFinder.GetPlugins<IExchangeRateProvider>();
            return exchangeRateProviders
                .OrderBy(tp => tp.PluginDescriptor)
                .ToList();
        }
        #endregion
    }
}