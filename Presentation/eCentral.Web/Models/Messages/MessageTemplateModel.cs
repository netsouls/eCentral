using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Messages;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Messages
{
    [Validator(typeof(MessageTemplateValidator))]
    public class MessageTemplateModel : BaseEntityModel
    {
        public MessageTemplateModel()
        {
            AvailableEmailAccounts = new List<EmailAccountModel>();
        }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.BccEmailAddresses")]
        [AllowHtml]
        public string BccEmailAddresses { get; set; }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.IsActive")]
        [AllowHtml]
        public bool IsActive { get; set; }

        [SiteResourceDisplayName("Configuration.MessageTemplates.Fields.EmailAccount")]
        public Guid EmailAccountId { get; set; }

        public IList<EmailAccountModel> AvailableEmailAccounts { get; set; }
    }
}