using System.ComponentModel.DataAnnotations;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Security;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Security
{
    [Validator(typeof(LoginValidator))]
    public class LoginModel : BaseModel
    {
        [SiteResourceDisplayName("Security.Fields.Username")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [SiteResourceDisplayName("Security.Fields.Password")]
        public string Password { get; set; }

        [SiteResourceDisplayName("Security.Fields.RememberMe")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Get or set the error messages
        /// </summary>
        public string Result { get; set; }
    }
}