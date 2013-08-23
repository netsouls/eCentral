using System.ComponentModel.DataAnnotations;
using eCentral.Web.Framework;
using eCentral.Web.Validators.Security;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Security
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public class PasswordRecoveryModel
    {
        [SiteResourceDisplayName("Security.Fields.Username")]
        public string UserName { get; set; }
    }
}