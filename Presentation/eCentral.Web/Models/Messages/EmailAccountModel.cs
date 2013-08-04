using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Messages;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Messages
{
    [Validator(typeof(EmailAccountValidator))]
    public class EmailAccountModel : BaseEntityModel
    {
        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.DisplayName")]
        [AllowHtml]
        public string DisplayName { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.Host")]
        [AllowHtml]
        public string Host { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.Port")]
        public int Port { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.Password")]
        [AllowHtml]
        public string Password { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.EnableSsl")]
        public bool EnableSsl { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.UseDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }

        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.IsDefaultEmailAccount")]
        public bool IsDefaultEmailAccount { get; set; }


        [SiteResourceDisplayName("Configuration.EmailAccounts.Fields.SendTestEmailTo")]
        [AllowHtml]
        public string SendTestEmailTo { get; set; }

    }
}