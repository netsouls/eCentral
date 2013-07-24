using System;
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

        #endregion
    }
}