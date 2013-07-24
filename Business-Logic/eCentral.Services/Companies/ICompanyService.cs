using System;
using System.Collections.Generic;
using eCentral.Core;
using eCentral.Core.Domain.Companies;

namespace eCentral.Services.Companies
{
    /// <summary>
    /// Company service interface
    /// </summary>
    public partial interface ICompanyService : IPropertyValidatorService
    {
        #region Companies

        /// <summary>
        /// Gets all Companies
        /// </summary>
        /// <returns>Country collection</returns>
        IList<Company> GetAll(PublishingStatus status = PublishingStatus.Active);

        /// <summary>
        /// Gets a company
        /// </summary>
        /// <param name="companyId">company identifier</param>
        /// <returns>A company</returns>
        Company GetById(Guid companyId);

        /// <summary>
        /// Insert a company
        /// </summary>
        /// <param name="company">company</param>
        void Insert(Company company);

        /// <summary>
        /// Updates the company
        /// </summary>
        /// <param name="company">company</param>
        void Update(Company company);

        #endregion
    }
}