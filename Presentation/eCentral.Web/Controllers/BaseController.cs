using System;
using eCentral.Services.Companies;
using eCentral.Web.Infrastructure.Cache;
using System.Collections.Generic;
using eCentral.Services.Users;
using eCentral.Core.Caching;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Logging;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Framework.Mvc;
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