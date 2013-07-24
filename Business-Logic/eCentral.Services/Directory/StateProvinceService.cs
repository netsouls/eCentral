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
    /// State province service
    /// </summary>
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY     = "eCentral.stateprovince.all-{0}";
        private const string STATEPROVINCES_BY_ID_KEY   = "eCentral.stateprovince.id-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "eCentral.stateprovince.";
        #endregion

        #region Fields

        private readonly IRepository<StateProvince> stateProvinceRepository;
        private readonly IEventPublisher eventPublisher;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="stateProvinceRepository">State/province repository</param>
        /// <param name="eventPublisher">Event published</param>
        public StateProvinceService(ICacheManager cacheManager,
            IRepository<StateProvince> stateProvinceRepository,
            IEventPublisher eventPublisher)
        {
            this.cacheManager            = cacheManager;
            this.stateProvinceRepository = stateProvinceRepository;
            this.eventPublisher          = eventPublisher;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        public virtual void Delete(StateProvince stateProvince)
        {
            Guard.IsNotNull(stateProvince, "stateProvince");

            this.stateProvinceRepository.Delete(stateProvince);

            this.cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(stateProvince);
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetById(Guid stateProvinceId)
        {
            if (stateProvinceId.IsEmpty())
                return null;

            string key = string.Format(STATEPROVINCES_BY_ID_KEY, stateProvinceId);
            return this.cacheManager.Get(key, () =>
            {
                var category = this.stateProvinceRepository.GetById(stateProvinceId);
                return category;
            });
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetByAbbreviation(string abbreviation)
        {
            var query = from sp in this.stateProvinceRepository.Table
                        where sp.Abbreviation == abbreviation
                        select sp;
            var stateProvince = query.FirstOrDefault();
            return stateProvince;
        }
        
        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>State/province collection</returns>
        public virtual IList<StateProvince> GetByCountryId(Guid countryId, bool showHidden = false)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            return this.cacheManager.Get(key, () =>
            {
                var query = from sp in this.stateProvinceRepository.Table
                            orderby sp.Name
                            where sp.CountryId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var stateProvinces = query.ToList();
                return stateProvinces;
            });
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void Insert(StateProvince stateProvince)
        {
            Guard.IsNotNull(stateProvince, "stateProvince");

            this.stateProvinceRepository.Insert(stateProvince);

            this.cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(stateProvince);
        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void Update(StateProvince stateProvince)
        {
            Guard.IsNotNull(stateProvince, "stateProvince");

            this.stateProvinceRepository.Update(stateProvince);

            this.cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(stateProvince);
        }

        #endregion
    }
}
