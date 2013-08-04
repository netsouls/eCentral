using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Logging;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Logging
{
    [Validator(typeof(ActivityLogTypeValidator))]
    public partial class ActivityLogTypeModel : BaseEntityModel
    {
        [SiteResourceDisplayName("AuditHistory.ActivityLogType.Fields.Name")]
        public string Name { get; set; }

        [SiteResourceDisplayName("AuditHistory.ActivityLogType.Fields.SystemKeyword")]
        public string SystemKeyword { get; set; }
    }
}