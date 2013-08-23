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
    /// port service
    /// </summary>
    public partial class PortService : IPortService
    {
        #region Constants
        
        private const string PORTS_ALL_KEY     = "eCentral.port.all-{0}";
        private const string PORTS_BY_ID_KEY   = "eCentral.port.id-{0}";
        private const string PORTS_PATTERN_KEY = "eCentral.port.";
        
        #endregion

        #region Fields

        private readonly IRepository<Port> dataRepository;
        private readonly IEventPublisher eventPublisher;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="portRepository">Port repository</param>
        /// <param name="eventPublisher">Event published</param>
        public PortService(ICacheManager cacheManager,
            IRepository<Port> dataRepository,
            IEventPublisher eventPublisher)
        {
            this.cacheManager            = cacheManager;
            this.dataRepository          = dataRepository;
            this.eventPublisher          = eventPublisher;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Deletes a port
        /// </summary>
        /// <param name="port">The port</param>
        public virtual void Delete(Port port)
        {
            Guard.IsNotNull(port, "port");

            this.dataRepository.Delete(port);

            this.cacheManager.RemoveByPattern(PORTS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityDeleted(port);
        }

        /// <summary>
        /// Gets a port
        /// </summary>
        /// <param name="portId">The port identifier</param>
        /// <returns>port</returns>
        public virtual Port GetById(Guid portId)
        {
            if (portId.IsEmpty())
                return null;

            string key = string.Format(PORTS_BY_ID_KEY, portId);
            return this.cacheManager.Get(key, () =>
            {
                var port = this.dataRepository.GetById(portId);
                return port;
            });
        }

        /// <summary>
        /// Gets a port
        /// </summary>
        /// <param name="abbreviation">The port abbreviation</param>
        /// <returns>port</returns>
        public virtual Port GetByAbbreviation(string abbreviation)
        {
            var query = from p in this.dataRepository.Table
                        where p.Abbreviation == abbreviation
                        select p;
            var port = query.FirstOrDefault();
            return port;
        }
        
        /// <summary>
        /// Gets a port collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>port collection</returns>
        public virtual IList<Port> GetByCountryId(Guid countryId)
        {
            string key = string.Format(PORTS_ALL_KEY, countryId);
            return this.cacheManager.Get(key, () =>
            {
                var query = from p in this.dataRepository.Table
                            orderby p.Name
                            where p.CountryId == countryId
                            select p;
                var ports = query.ToList();
                return ports;
            });
        }

        /// <summary>
        /// Inserts a port
        /// </summary>
        /// <param name="port">port</param>
        public virtual void Insert(Port port)
        {
            Guard.IsNotNull(port, "port");

            this.dataRepository.Insert(port);

            this.cacheManager.RemoveByPattern(PORTS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(port);
        }

        /// <summary>
        /// Updates a port
        /// </summary>
        /// <param name="port">port</param>
        public virtual void Update(Port port)
        {
            Guard.IsNotNull(port, "port");

            this.dataRepository.Update(port);

            this.cacheManager.RemoveByPattern(PORTS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(port);
        }

        #endregion
    }
}
