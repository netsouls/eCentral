using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Clients;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Logging;
using eCentral.Services.Events;
using eCentral.Services.Logging;

namespace eCentral.Services.Clients
{
    /// <summary>
    /// user service
    /// </summary>
    public partial class ClientService : IClientService
    {
        #region Constants

        private const string CLIENTS_ALL_KEY = "eCentral.clients.all-{0}";
        private const string CLIENTS_BY_ID_KEY = "eCentral.clients.id-{0}";
        private const string CLIENTS_PATTERN_KEY = "eCentral.clients.";

        #endregion

        #region Fields

        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<ActivityLog> auditLogRepository;
        private readonly ICacheManager cacheManager;
        private readonly IEventPublisher eventPublisher;
        private readonly IUserActivityService userActivityService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="eventPublisher">Event published</param>
        public ClientService(ICacheManager cacheManager,
            IRepository<Client> clientRepository, IUserActivityService userActivityService,
            IRepository<GenericAttribute> attributeRepository,IRepository<ActivityLog> auditLogRepository,
            IEventPublisher eventPublisher) 
        {
            this.cacheManager            = cacheManager;
            this.clientRepository        = clientRepository;
            this.eventPublisher          = eventPublisher;
            this.auditLogRepository      = auditLogRepository;
            this.userActivityService     = userActivityService;
        }

        #endregion

        #region Methods

        #region clients
        
        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="dateFrom">registration from; null to load all users</param>
        /// <param name="dateTo">registration to; null to load all users</param>
        /// <param name="clientName">ClientName; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>user collection</returns>
        public virtual IPagedList<Client> GetAll(DateTime? dateFrom,
           DateTime? dateTo, string name,
           int pageIndex, int pageSize)
        {
            var query = this.clientRepository.Table;
            if (dateFrom.HasValue)
                query = query.Where(c => dateFrom.Value <= c.CreatedOn);
            if (dateTo.HasValue)
                query = query.Where(c => dateTo.Value >= c.CreatedOn);
            
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.ClientName.Contains(name));
            query = query.OrderByDescending(c => c.CreatedOn);

            var clients = new PagedList<Client>(query, pageIndex, pageSize);
            return clients;
        }

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Client collection</returns>
        public virtual IList<Client> GetAll(PublishingStatus status = PublishingStatus.Active)
        {
            string key = string.Format(CLIENTS_ALL_KEY, status.ToString());
            return this.cacheManager.Get(key, () =>
            {
                var query = from c in this.clientRepository.Table
                            orderby c.ClientName
                            where (status == PublishingStatus.All || c.CurrentPublishingStatusId == (int)status)
                            select c;
                var clients = query.ToList();
                return clients;
            });
        }

        /// <summary>
        /// Gets a user
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <returns>A user</returns>
        public virtual Client GetById(Guid clientId)
        {
            if (clientId.IsEmpty())
                return null;

            string key = string.Format(CLIENTS_BY_ID_KEY, clientId);
            return this.cacheManager.Get(key, () =>
            {
                var client = this.clientRepository.GetById(clientId);
                return client;
            });
        }

        /// <summary>
        /// Check whether the entity is unique
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>user</returns>
        public virtual bool IsUnique(object uniqueValue)
        {
            return IsUnique(uniqueValue, Guid.Empty);
        }

        /// <summary>
        /// Insert a client
        /// </summary>
        /// <param name="client">client</param>
        public virtual void Insert(Client client)
        {
            Guard.IsNotNull(client, "client");

            // add audit history
            client.AuditHistory.Add
             (
                userActivityService.InsertActivity( SystemActivityLogTypeNames.Add, 
                    client.ToString(), string.Empty)
             );

            this.clientRepository.Insert(client);

            this.cacheManager.RemoveByPattern(CLIENTS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(client);
        }
        
        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="client">client</param>
        public virtual void Update(Client client)
        {
            Guard.IsNotNull(client, "client");

            // add audit history
            client.AuditHistory.Add
             (
                userActivityService.InsertActivity(SystemActivityLogTypeNames.Update,
                    client.ToString(), string.Empty)
             );

            this.clientRepository.Update(client);

            this.cacheManager.RemoveByPattern(CLIENTS_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(client);
        }

        public virtual bool ChangeStatus (Guid clientId, PublishingStatus publishingStatus)
        {
            Guard.IsNotEmpty(clientId, "clientId");

            // retrieve the client
            var client = GetById(clientId);

            if (client != null)
            {
                client.CurrentPublishingStatus = publishingStatus;
                client.UpdatedOn = DateTime.UtcNow;

                client.AuditHistory.Add
                (
                    userActivityService.InsertActivity(SystemActivityLogTypeNames.ChangePublishingStatus,
                        publishingStatus.ToString(), string.Empty)
                );

                this.clientRepository.Update(client);
                this.cacheManager.RemoveByPattern(CLIENTS_PATTERN_KEY);
                //event notification
                this.eventPublisher.EntityUpdated(client);

                return true;
            }

            return false;
        }

        #endregion


        #region Utilities

        /// <summary>
        /// Check whether the entity is unique
        /// </summary>
        /// <param name="uniqueValue">Unique identity value of the repository</param>
        /// <returns>user</returns>
        private bool IsUnique(object uniqueValue, Guid clientId)
        {
            var isUnique = false;

            if (uniqueValue != null)
            {
                string clientName = uniqueValue.ToString().Trim();

                var clients = this.GetAll(PublishingStatus.All);

                if (clients.Count > 0)
                {
                    var client = clients.FirstOrDefault(item => item.ClientName.IsCaseInsensitiveEqual(clientName) &&
                        !item.RowId.Equals(clientId));

                    if (client == null)
                        isUnique = true;
                }
                else
                    isUnique = true;

            }

            return isUnique;
        }

        #endregion

        #endregion
    }
}