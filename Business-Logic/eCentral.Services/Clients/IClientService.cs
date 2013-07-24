using System;
using System.Collections.Generic;
using eCentral.Core;
using eCentral.Core.Domain.Clients;

namespace eCentral.Services.Clients
{
    /// <summary>
    /// Client service interface
    /// </summary>
    public partial interface IClientService : IPropertyValidatorService
    {
        #region Clients

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <param name="dateFrom">registration from; null to load all users</param>
        /// <param name="dateTo">registration to; null to load all users</param>
        /// <param name="clientName">ClientName; null to load all users</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>user collection</returns>
        IPagedList<Client> GetAll(DateTime? dateFrom,
           DateTime? dateTo, string name,
           int pageIndex, int pageSize);

        /// <summary>
        /// Gets all clients
        /// </summary>
        /// <returns>Country collection</returns>
        IList<Client> GetAll(PublishingStatus status = PublishingStatus.Active);

        /// <summary>
        /// Gets a client
        /// </summary>
        /// <param name="userId">client identifier</param>
        /// <returns>A client</returns>
        Client GetById(Guid clientId);

        /// <summary>
        /// Insert a client
        /// </summary>
        /// <param name="client">client</param>
        void Insert(Client client);

        /// <summary>
        /// Updates the client
        /// </summary>
        /// <param name="user">user</param>
        void Update(Client client);

        /// <summary>
        /// Change the status
        /// </summary>
        /// <param name="clientId">client identifier</param>
        /// <param name="publishingStatus">publishing status</param>
        /// <returns></returns>
        bool ChangeStatus(Guid clientId, PublishingStatus publishingStatus);

        #endregion
    }
}