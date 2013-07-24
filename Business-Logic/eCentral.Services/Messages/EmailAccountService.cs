using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain.Messages;
using eCentral.Services.Events;

namespace eCentral.Services.Messages
{
    public partial class EmailAccountService:IEmailAccountService
    {
        private readonly IRepository<EmailAccount> emailAccountRepository;
        private readonly EmailAccountSettings emailAccountSettings;
        private readonly IEventPublisher eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="emailAccountRepository">Email account repository</param>
        /// <param name="emailAccountSettings"></param>
        /// <param name="eventPublisher">Event published</param>
        public EmailAccountService(IRepository<EmailAccount> emailAccountRepository,
            EmailAccountSettings emailAccountSettings, IEventPublisher eventPublisher)
        {
            this.emailAccountRepository = emailAccountRepository;
            this.emailAccountSettings   = emailAccountSettings;
            this.eventPublisher         = eventPublisher;
        }

        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void Insert(EmailAccount emailAccount)
        {
            Guard.IsNotNull(emailAccount, "EmailAccount");

            emailAccount.Email       = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host        = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username    = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password    = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email       = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host        = emailAccount.Host.Trim();
            emailAccount.Username    = emailAccount.Username.Trim();
            emailAccount.Password    = emailAccount.Password.Trim();

            emailAccount.Email       = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host        = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username    = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password    = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            emailAccountRepository.Insert(emailAccount);

            //event notification
            eventPublisher.EntityInserted(emailAccount);
        }

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void Update(EmailAccount emailAccount)
        {
            Guard.IsNotNull(emailAccount, "EmailAccount");

            emailAccount.Email       = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host        = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username    = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password    = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email       = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host        = emailAccount.Host.Trim();
            emailAccount.Username    = emailAccount.Username.Trim();
            emailAccount.Password    = emailAccount.Password.Trim();

            emailAccount.Email       = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host        = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username    = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password    = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            emailAccountRepository.Update(emailAccount);

            //event notification
            eventPublisher.EntityUpdated(emailAccount);
        }

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void Delete(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException("emailAccount");

            if (GetAll().Count == 1)
                throw new SiteException("You cannot delete this email account. At least one account is required.");

            emailAccountRepository.Delete(emailAccount);

            //event notification
            eventPublisher.EntityDeleted(emailAccount);
        }

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>Email account</returns>
        public virtual EmailAccount GetById(Guid emailAccountId)
        {
            if (emailAccountId.IsEmpty())
                return null;

            var emailAccount = emailAccountRepository.GetById(emailAccountId);
            return emailAccount;
        }

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>Email accounts list</returns>
        public virtual IList<EmailAccount> GetAll()
        {
            var query = from ea in emailAccountRepository.Table
                        orderby ea.RowId
                        select ea;
            var emailAccounts = query.ToList();
            return emailAccounts;
        }
    }
}
