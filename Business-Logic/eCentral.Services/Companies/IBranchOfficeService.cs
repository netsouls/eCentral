using System;
using System.Collections.Generic;
using eCentral.Core;
using eCentral.Core.Domain.Companies;

namespace eCentral.Services.Companies
{
    /// <summary>
    /// Branch Office service interface
    /// </summary>
    public partial interface IBranchOfficeService : IPropertyValidatorService
    {
        #region Clients

        /// <summary>
        /// Gets all branch offices
        /// </summary>
        /// <returns>Office collection</returns>
        IList<BranchOffice> GetAll(PublishingStatus status = PublishingStatus.Active);

        /// <summary>
        /// Gets a branch office
        /// </summary>
        /// <param name="officeId">office identifier</param>
        /// <returns>A branch office</returns>
        BranchOffice GetById(Guid officeId);

        /// <summary>
        /// Insert a branch office
        /// </summary>
        /// <param name="branchOffice">branchOffice</param>
        void Insert(BranchOffice branchOffice);

        /// <summary>
        /// Updates the branch office
        /// </summary>
        /// <param name="branchOffice">branchOffice</param>
        void Update(BranchOffice branchOffice);

        /// <summary>
        /// Change the status
        /// </summary>
        /// <param name="officeId">office identifier</param>
        /// <param name="publishingStatus">publishing status</param>
        /// <returns></returns>
        bool ChangeStatus(Guid officeId, PublishingStatus publishingStatus);

        #endregion
    }
}