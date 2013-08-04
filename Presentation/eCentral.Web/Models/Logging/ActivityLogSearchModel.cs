using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Framework;

namespace eCentral.Web.Models.Logging
{
    public class ActivityLogSearchModel : BaseModel
    {
        public ActivityLogSearchModel()
        {
            ActivityLogType = new List<SelectListItem>();
            Users = new List<SelectListItem>();
        }

        [UIHint("DateNullable")]
        [SiteResourceDisplayName("AuditHistory.Fields.CreatedOnFrom")]
        public DateTime? CreatedOnFrom { get; set; }

        [UIHint("DateNullable")]
        [SiteResourceDisplayName("AuditHistory.Fields.CreatedOnTo")]
        public DateTime? CreatedOnTo { get; set; }

        [SiteResourceDisplayName("AuditHistory.Fields.ActivityLogType")]
        public Guid ActivityLogTypeId { get; set; }

        [SiteResourceDisplayName("AuditHistory.Fields.User")]
        public Guid UserId { get; set; }

        public IList<SelectListItem> ActivityLogType { get; set; }
        public IList<SelectListItem> Users { get; set; }
    }
}