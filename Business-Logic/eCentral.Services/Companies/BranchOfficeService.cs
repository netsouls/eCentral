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
    /// Branch office service
    /// </summary>
    public partial class BranchOfficeService : IBranchOfficeService
    {
        #region Constants

        private const string BRANCHOFFICE_ALL_KEY = "eCentral.branchoffice.all-{0}";
        private const string BRANCHOFFICE_BY_ID_KEY = "eCentral.branchoffice.id-{0}";
        private const string BRANCHOFFICE_PATTERN_KEY = "eCentral.branchoffice.";

        private const string USER_ACTIVITY_COMMENT = "Office:[{0}]";

        #endregion

        #region Fields

        private readonly IRepository<BranchOffice> dataRepository;
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
        public BranchOfficeService(ICacheManager cacheManager,
            IRepository<BranchOffice> dataRepository, IUserActivityService userActivityService,
            IRepository<GenericAttribute> attributeRepository,IRepository<ActivityLog> auditLogRepository,
            IEventPublisher eventPublisher) 
        {
            this.cacheManager            = cacheManager;
            this.dataRepository          = dataRepository;
            this.eventPublisher          = eventPublisher;
            this.auditLogRepository      = auditLogRepository;
            this.userActivityService     = userActivityService;
        }

        #endregion

        #region Methods

        #region Branch Offices

        /// <summary>
        /// Gets all branch offices
        /// </summary>
        /// <returns>Office collection</returns>
        public virtual IList<BranchOffice> GetAll(PublishingStatus status = PublishingStatus.Active)
        {
            string key = string.Format(BRANCHOFFICE_ALL_KEY, status.ToString());
            return this.cacheManager.Get(key, () =>
            {
                var query = from c in this.dataRepository.Table
                            orderby c.BranchName
                            where (status == PublishingStatus.All || c.CurrentPublishingStatusId == (int)status)
                            select c;
                var offices = query.ToList();
                return offices;
            });
        }

        /// <summary>
        /// Gets a branch office
        /// </summary>
        /// <param name="officeId">office identifier</param>
        /// <returns>A branch office</returns>
        public virtual BranchOffice GetById(Guid officeId)
        {
            if (officeId.IsEmpty())
                return null;

            string key = string.Format(BRANCHOFFICE_BY_ID_KEY, officeId);
            return this.cacheManager.Get(key, () =>
            {
                var branchOffice = this.dataRepository.GetById(officeId);
                return branchOffice;
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
        /// Insert a branch office
        /// </summary>
        /// <param name="branchOffice">branchOffice</param>
        public virtual void Insert(BranchOffice branchOffice)
        {
            Guard.IsNotNull(branchOffice, "branchOffice");

            // add audit history
            branchOffice.AuditHistory.Add
             (
                userActivityService.InsertActivity( SystemActivityLogTypeNames.Add, 
                    branchOffice.ToString(), USER_ACTIVITY_COMMENT, branchOffice.BranchName)
             );

            this.dataRepository.Insert(branchOffice);

            this.cacheManager.RemoveByPattern(BRANCHOFFICE_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityInserted(branchOffice);
        }

        /// <summary>
        /// Updates the branch office
        /// </summary>
        /// <param name="branchOffice">branchOffice</param>
        public virtual void Update(BranchOffice branchOffice)
        {
            Guard.IsNotNull(branchOffice, "branchOffice");

            // add audit history
            branchOffice.AuditHistory.Add
             (
                userActivityService.InsertActivity(SystemActivityLogTypeNames.Update,
                    branchOffice.ToString(), USER_ACTIVITY_COMMENT, branchOffice.BranchName)
             );

            this.dataRepository.Update(branchOffice);

            this.cacheManager.RemoveByPattern(BRANCHOFFICE_PATTERN_KEY);

            //event notification
            this.eventPublisher.EntityUpdated(branchOffice);
        }

        /// <summary>
        /// Change the status
        /// </summary>
        /// <param name="officeId">office identifier</param>
        /// <param name="publishingStatus">publishing status</param>
        /// <returns></returns>
        public virtual bool ChangeStatus(Guid officeId, PublishingStatus publishingStatus)
        {
            Guard.IsNotEmpty(officeId, "officeId");

            // retrieve the client
            var branchOffice = GetById(officeId);

            if (branchOffice != null)
            {
                branchOffice.CurrentPublishingStatus = publishingStatus;
                branchOffice.UpdatedOn = DateTime.UtcNow;

                branchOffice.AuditHistory.Add
                (
                    userActivityService.InsertActivity(SystemActivityLogTypeNames.ChangePublishingStatus,
                        publishingStatus.GetFriendlyName(), USER_ACTIVITY_COMMENT, branchOffice.BranchName)
                );

                this.dataRepository.Update(branchOffice);
                this.cacheManager.RemoveByPattern(BRANCHOFFICE_PATTERN_KEY);
                //event notification
                this.eventPublisher.EntityUpdated(branchOffice);

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
        private bool IsUnique(object uniqueValue, Guid officeId)
        {
            var isUnique = false;

            if (uniqueValue != null)
            {
                string officeName = uniqueValue.ToString().Trim();

                var offices = this.GetAll(PublishingStatus.All);

                if (offices.Count > 0)
                {
                    var branchOffice = offices.FirstOrDefault(item => item.BranchName.IsCaseInsensitiveEqual(officeName) &&
                        !item.RowId.Equals(officeId));

                    if (branchOffice == null)
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