using System;
using System.ComponentModel.DataAnnotations;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Security;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Security
{
    [Validator(typeof(AccountActivationValidator))]
    public class AccountActivationModel : BaseModel
    {
        [SiteResourceDisplayName("Security.Fields.Username")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [SiteResourceDisplayName("Security.Fields.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [SiteResourceDisplayName("Security.Fields.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Get or set the user name
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// Get or set the user identifier
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Get or set the user token
        /// </summary>
        public Guid Token { get; set; }
    }
}