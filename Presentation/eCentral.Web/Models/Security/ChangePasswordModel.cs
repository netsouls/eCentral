using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Security;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Security
{
    [Validator(typeof(ChangePasswordValidator))]
    public partial class ChangePasswordModel : BaseModel
    {
        [AllowHtml]
        [DataType(DataType.Password)]
        [SiteResourceDisplayName("Security.Fields.OldPassword")]
        public string OldPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [SiteResourceDisplayName("Security.Fields.NewPassword")]
        public string NewPassword { get; set; }

        [AllowHtml]
        [DataType(DataType.Password)]
        [SiteResourceDisplayName("Security.Fields.ConfirmPassword")]
        public string ConfirmNewPassword { get; set; }

        public string Result { get; set; }

    }
}