using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Logging;
using eCentral.Services.Common;
using eCentral.Services.Companies;
using eCentral.Services.Users;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Common;

namespace eCentral.Web.Controllers
{
    [Compress]
    [NoCacheControl]
    public class BaseController : SiteController
    {
        #region Utilities

        [NonAction]
        protected AddressModel AddressModel(string address1, string address2, string city,
            string phoneNumber, string faxNumber, string zipPostalCode, string countryId, Guid? stateProvinceId)
        {
            return new AddressModel
            {
                Address1        = string.IsNullOrEmpty(address1) ? string.Empty : address1,
                Address2        = string.IsNullOrEmpty(address2) ? string.Empty : address2,
                City            = string.IsNullOrEmpty(city) ? string.Empty : city,
                PhoneNumber     = string.IsNullOrEmpty(phoneNumber) ? string.Empty : phoneNumber,
                FaxNumber       = string.IsNullOrEmpty(faxNumber) ? string.Empty : faxNumber,
                ZipPostalCode   = string.IsNullOrEmpty(zipPostalCode) ? string.Empty : zipPostalCode,
                CountryId       = new Guid(countryId),
                StateProvinceId = stateProvinceId
            };
        }

        /// <summary>
        /// Sets the audit history model
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void PrepareAuditHistoryModel ( IAudiHistoryModel model, IAuditHistory entity, bool showTime = false )
        {
            var activityLog = entity.AuditHistory
                .OrderByDescending(al => al.CreatedOn).FirstOrDefault();
            
            if (activityLog != null)
            {
                model.LastUpdated = "{0} [{1}]".FormatWith(activityLog.User.Username,
                    activityLog.CreatedOn.ToString(showTime ? StateKeyManager.DateTimeFormat : StateKeyManager.DateFormat));
            }
            else
                model.LastUpdated = string.Empty;

            model.PublishingStatus = entity.CurrentPublishingStatus.GetFriendlyName();
        }

        /// <summary>
        /// Prepare branch office association
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        [NonAction]
        protected void PrepareOfficeAssociationModel(IBranchOfficeAssociation model, BaseEntity entity, 
            IBranchOfficeService officeService, ICacheManager cacheManager)
        {
            var associatedOffices = entity.GetAttribute<List<Guid>>(SystemAttributeNames.AssociatedBrancOffices);

            if (associatedOffices != null && associatedOffices.Count > 0)
            {
                var availableOffices = PrepareSelectList(officeService, cacheManager);
                model.Offices = availableOffices
                    .Where(o => associatedOffices.Contains(new Guid(o.Value)))
                    .Select(o => o.Text)
                    .ToList();
            }
            else
                model.Offices = new List<string>();
        }

        /// <summary>
        /// Prepare branch office association
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        [NonAction]
        protected string PrepareOfficeAssociationModel(List<Guid> associatedOffices, 
            IBranchOfficeService officeService, ICacheManager cacheManager)
        {
            if (associatedOffices != null && associatedOffices.Count > 0)
            {
                var availableOffices = PrepareSelectList(officeService, cacheManager);
                return availableOffices
                    .Where(o => associatedOffices.Contains(new Guid(o.Value)))
                    .Select(o => o.Text)
                    .ToDelimitedString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Sets the list of all users in the system
        /// </summary>
        [NonAction]
        protected IList<SelectListItem> PrepareSelectList(IUserService userService, ICacheManager cacheManager,
            Guid selectedUserId, PublishingStatus status = PublishingStatus.Active)
        {
            string cacheKey = ModelCacheEventUser.USERS_MODEL_KEY.FormatWith(
                "SelectList.{0}".FormatWith(status.ToString()));

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var users = userService.GetAll(status)
                    .Select(user =>
                    {
                        return new SelectListItem()
                        {
                            Value = user.RowId.ToString(),
                            Text = user.GetFullName(),
                            Selected = user.RowId.Equals(selectedUserId)
                        };
                    })
                    .ToList();

                return users;
            });

            return cacheModel;
        }

        /// <summary>
        /// Sets the list of all users in the system
        /// </summary>
        [NonAction]
        protected IList<SelectListItem> PrepareSelectList(IBranchOfficeService officeService, ICacheManager cacheManager,
            PublishingStatus status = PublishingStatus.Active)
        {
            string cacheKey = ModelCacheEventUser.OFFICE_MODEL_KEY.FormatWith(
                "SelectList.{0}".FormatWith(status.ToString()));

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var offices = officeService.GetAll(status)
                    .Select(office =>
                    {
                        return new SelectListItem()
                        {
                            Value = office.RowId.ToString(),
                            Text = office.BranchName
                        };
                    })
                    .ToList();

                return offices;
            });

            return cacheModel;
        }

        #endregion
    }
}