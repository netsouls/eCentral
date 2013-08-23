using System;
using System.Collections.Generic;
using eCentral.Core.Domain.Directory;

namespace eCentral.Services.Directory
{
    /// <summary>
    /// port service interface
    /// </summary>
    public partial interface IPortService
    {
        /// <summary>
        /// Deletes a port
        /// </summary>
        /// <param name="port">The port</param>
        void Delete(Port port);

        /// <summary>
        /// Gets a port
        /// </summary>
        /// <param name="portId">The port identifier</param>
        /// <returns>port</returns>
        Port GetById(Guid portId);

        /// <summary>
        /// Gets a port
        /// </summary>
        /// <param name="abbreviation">The port abbreviation</param>
        /// <returns>port</returns>
        Port GetByAbbreviation(string abbreviation);
        
        /// <summary>
        /// Gets a port collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>port collection</returns>
        IList<Port> GetByCountryId(Guid countryId);

        /// <summary>
        /// Inserts a port
        /// </summary>
        /// <param name="port">port</param>
        void Insert(Port port);

        /// <summary>
        /// Updates a port
        /// </summary>
        /// <param name="port">port</param>
        void Update(Port port);
    }
}
