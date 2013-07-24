using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Directory;
using eCentral.Services.Events;

namespace eCentral.Services.Directory
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Constants
        
        private const string COUNTRIES_ALL_KEY      = "eCentral.country.all-{0}";
        private const string COUNTRIES_BILLING_KEY  = "eCentral.country.billing-{0}";
        private const string COUNTRIES_SHIPPING_KEY = "eCentral.country.shipping-{0}";
        private const string COUNTRIES_BY_ID_KEY    = "eCentral.country.id-{0}";
        private const string COUNTRIES_PATTERN_KEY  = "eCentral.country.";
        
        #endregion
        
        #region Fields
        
        private readonly IRepository<Country> countryRepository;
        private readonly IEventPublisher eventPublisher;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="countryRepository">Country repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CountryService(ICacheManager cacheManager,
            IRepository<Country> countryRepository,
            IEventPublisher eventPublisher)
        {
            this.cacheManager      = cacheManager;
            this.countryRepository = countryRepository;
            this.eventPublisher    = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void Delete(Country country)
        {
            Guard.IsNotNull(country, "country");

            this.countryRepository.Delete(country);

            this.cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(country);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAll(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_ALL_KEY, showHidden);
            return this.cacheManager.Get(key, () =>
            {
                var query = from c in this.countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where showHidden || c.Published
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public virtual Country GetById(Guid countryId)
        {
            if (countryId.IsEmpty())
                return null;

            string key = string.Format(COUNTRIES_BY_ID_KEY, countryId);
            return this.cacheManager.Get(key, () =>
            {
                var category = this.countryRepository.GetById(countryId);
                return category;
            });
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            var query = from c in this.countryRepository.Table
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();

            return country;
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetByThreeLetterIsoCode(string threeLetterIsoCode)
        {
            var query = from c in this.countryRepository.Table
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void Insert(Country country)
        {
            Guard.IsNotNull(country, "country");

            this.countryRepository.Insert(country);

            this.cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(country);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void Update(Country country)
        {
            Guard.IsNotNull(country, "country");

            this.countryRepository.Update(country);

            this.cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(country);
        }

        #endregion
    }
}