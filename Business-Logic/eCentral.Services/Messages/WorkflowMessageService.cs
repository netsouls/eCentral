using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Domain.Users;
using eCentral.Services.Localization;
using eCentral.Services.Users;

namespace eCentral.Services.Messages
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly IMessageTemplateService messageTemplateService;
        private readonly IQueuedEmailService queuedEmailService;
        private readonly ILanguageService languageService;
        private readonly ITokenizer tokenizer;
        private readonly IEmailAccountService emailAccountService;
        private readonly IMessageTokenProvider messageTokenProvider;
        private readonly IWebHelper webHelper;
        private readonly EmailAccountSettings emailAccountSettings;

        #endregion

        #region Ctor

        public WorkflowMessageService(IMessageTemplateService messageTemplateService,
            IQueuedEmailService queuedEmailService, ILanguageService languageService,
            ITokenizer tokenizer, IEmailAccountService emailAccountService,
            IMessageTokenProvider messageTokenProvider, IWebHelper webHelper,
            EmailAccountSettings emailAccountSettings)
        {
            this.messageTemplateService = messageTemplateService;
            this.queuedEmailService     = queuedEmailService;
            this.languageService        = languageService;
            this.tokenizer              = tokenizer;
            this.emailAccountService    = emailAccountService;
            this.messageTokenProvider   = messageTokenProvider;
            this.webHelper              = webHelper;
            this.emailAccountSettings = emailAccountSettings;
        }

        #endregion

        #region Methods

        #region user workflow

        /// <summary>
        /// Sends 'New user' notification message to a store owner
        /// </summary>
        /// <param name="user">user instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual Guid SendUserRegisteredNotificationMessage(User user, Guid languageId)
        {
            Guard.IsNotNull(user, "user");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.AdminNotification", languageId);
            if (messageTemplate == null)
                return Guid.Empty;

            var userTokens = GenerateTokens(user);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail      = emailAccount.Email;
            var toName       = emailAccount.DisplayName;
            return SendNotification(messageTemplate, emailAccount,
                languageId, userTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">user instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual Guid SendUserWelcomeMessage(User user, Guid languageId)
        {
            Guard.IsNotNull(user, "user"); 
            
            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.WelcomeMessage", languageId);
            if (messageTemplate == null)
                return Guid.Empty;

            var userTokens = GenerateTokens(user);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail      = user.Username;
            var toName       = user.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, userTokens, 
                toEmail, toName);
        }

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">user instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual Guid SendUserEmailValidationMessage(User user, Guid languageId)
        {
            Guard.IsNotNull(user, "user");

            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.EmailValidationMessage", languageId);
            if (messageTemplate == null)
                return Guid.Empty;

            var userTokens = GenerateTokens(user);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail      = user.Username;
            var toName       = user.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, userTokens,
                toEmail, toName);
        }

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">user instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual Guid SendUserPasswordRecoveryMessage(User user, Guid languageId)
        {
            Guard.IsNotNull(user, "user");
            
            languageId = EnsureLanguageIsActive(languageId);

            var messageTemplate = GetLocalizedActiveMessageTemplate("User.PasswordRecovery", languageId);
            if (messageTemplate == null)
                return Guid.Empty;

            var userTokens = GenerateTokens(user);

            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);
            var toEmail      = user.Username;
            var toName       = user.GetFullName();
            return SendNotification(messageTemplate, emailAccount,
                languageId, userTokens,
                toEmail, toName);
        }

        #endregion

        #endregion

        #region Utilities

        private Guid SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, Guid languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName)
        {
            //retrieve localized message template data
            var bcc     = messageTemplate.GetLocalized((mt) => mt.BccEmailAddresses, languageId);
            var subject = messageTemplate.GetLocalized((mt) => mt.Subject, languageId);
            var body    = messageTemplate.GetLocalized((mt) => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = tokenizer.Replace(subject, tokens, false);
            var bodyReplaced    = tokenizer.Replace(body, tokens, true);

            // retrieve the main localized master template
            var masterTemplate = GetLocalizedActiveMessageTemplate("Templates.Master", languageId);
            if (masterTemplate != null)
            {
                // set master template tokens
                var masterBody = masterTemplate.GetLocalized((mt) => mt.Body, languageId);
                messageTokenProvider.AddMasterTokens((IList<Token>)tokens, bodyReplaced);
                bodyReplaced = tokenizer.Replace(masterBody, tokens, true);
            }

            var email = new QueuedEmail()
            {
                Priority       = 5,
                From           = emailAccount.Email,
                FromName       = emailAccount.DisplayName,
                To             = toEmailAddress,
                ToName         = toName,
                CC             = string.Empty,
                Bcc            = bcc,
                Subject        = subjectReplaced,
                Body           = bodyReplaced,
                CreatedOn      = DateTime.UtcNow,
                EmailAccountId = emailAccount.RowId
            };

            queuedEmailService.Insert(email);
            return email.RowId;
        }

        private IList<Token> GenerateTokens(User user)
        {
            var tokens = new List<Token>();
            messageTokenProvider.AddDefaultTokens(tokens);
            messageTokenProvider.AddUserTokens(tokens, user);
            return tokens;
        }

        private MessageTemplate GetLocalizedActiveMessageTemplate(string messageTemplateName, Guid languageId)
        {
            var messageTemplate = messageTemplateService.GetByName(messageTemplateName);
            if (messageTemplate == null)
                return null;

            //var isActive = messageTemplate.GetLocalized((mt) => mt.IsActive, languageId);
            //use
            var isActive = messageTemplate.IsActive;
            if (!isActive)
                return null;

            return messageTemplate;
        }

        private EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, Guid languageId)
        {
            var emailAccounId = messageTemplate.GetLocalized(mt => mt.EmailAccountId, languageId);
            var emailAccount  = emailAccountService.GetById(emailAccounId);
            if (emailAccount  == null)
                emailAccount  = emailAccountService.GetById(emailAccountSettings.DefaultEmailAccountId);
            if (emailAccount  == null)
                emailAccount  = emailAccountService.GetAll().FirstOrDefault();
            return emailAccount;

        }

        private Guid EnsureLanguageIsActive(Guid languageId)
        {
            var language = languageService.GetById(languageId);
            if (language == null || !language.Published)
                language = languageService.GetAll().FirstOrDefault();
            return language.RowId;
        }

        #endregion
    }
}
