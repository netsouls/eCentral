using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Companies;
using eCentral.Core.Domain.Logging;
using eCentral.Services.Events;
using eCentral.Services.Logging;

namespace eCentral.Services.Companies
{
    /// <summary>
    /// company service
    /// </summary>
    public partial class CompanyService : ICompanyService
    {
        #region Constants

        private const string COMPANY_ALL_KEY = "eCentral.company.all-{0}";
        private const string COMPANY_BY_ID_KEY = "eCentral.company.id-{0}";
        private const string COMPANY_PATTERN_KEY = "eCentral.company.";

        #endregion

        #region Fields

        private readonly IRepository<Company> companyRepository;
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
        public CompanyService(ICacheManager cacheManager,
            IRepository<Company> companyRepository, IUserActivityService userActivityService,
            IRepository<GenericAttribute> attributeRepository, 
            IEventPublisher eventPublisher)
        {
            this.cacheManager             = cacheManager;
            this.companyRepository        = companyRepository;
            this.eventPublisher           = eventPublisher;
            this.userActivityService      = userActivityService;
        }

        #endregion

        #region Methods

        #region company

        /// <summary>
        /// Gets all companies
        /// </summary>
        /// <param name="status">A value indicating which status records</param>
        /// <returns>Company collection</returns>
        public virtual IList<Company> GetAll(PublishingStatus status = PublishingStatus.Active)
        {
            string key = string.Format(COMPANY_ALL_KEY, status.ToString());
            return this.cacheManager.Get(key, () =>
            {
                var query = from c in this.companyRepository.Table
                            orderby c.CompanyName
                            where (status == PublishingStatus.All || c.CurrentPublishingStatusId == (int)status)
                            select c;
                var companies = query.ToList();
                return companies;
            });
        }

        /// <summary>
        /// Gets a company
        /// </summary>
        /// <param name="companyId">company identifier</param>
        /// <returns>A company</returns>
        public virtual Company GetById(Guid companyId)
        {
            if (companyId.IsEmpty())
                return null;

            string key = string.Format(COMPANY_BY_ID_KEY, companyId);
            return this.cacheManager.Get(key, () =>
            {
                var company = this.companyRepository.GetById(companyId);
                return company;
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
        public virtual void Insert(Company company)
        {
            Guard.IsNotNull(company, "company");

            // add audit history
            company.AuditHistory.Add
             (
                userActivityService.InsertActivity( SystemActivityLogTypeNames.Add,
                    company.ToString(), StateKeyManager.COMPANY_ACTIVITY_COMMENT, company.CompanyName)
             );

            this.companyRepository.Insert(company);

            this.cacheManager.RemoveByPattern(COMPANY_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(company);
        }
        
        /// <summary>
        /// Updates the company
        /// </summary>
        /// <param name="company">company</param>
        public virtual void Update(Company company)
        {
            Guard.IsNotNull(company, "company");

            // add audit history
            company.AuditHistory.Add
             (
                userActivityService.InsertActivity(SystemActivityLogTypeNames.Update,
                    company.ToString(), StateKeyManager.COMPANY_ACTIVITY_COMMENT, company.CompanyName)
             );

            this.companyRepository.Update(company);

            this.cacheManager.RemoveByPattern(COMPANY_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(company);
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
                string companyName = uniqueValue.ToString().Trim();

                var companies = this.GetAll(PublishingStatus.All);

                if (companies.Count > 0)
                {
                    var company = companies.FirstOrDefault(item => item.CompanyName.IsCaseInsensitiveEqual(companyName) &&
                        !item.RowId.Equals(clientId));

                    if (company == null)
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