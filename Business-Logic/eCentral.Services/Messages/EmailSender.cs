using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using eCentral.Core.Domain.Messages;
using eCentral.Services.Security.Cryptography;

namespace eCentral.Services.Messages
{
    public partial class EmailSender:IEmailSender
    {
        #region Fields

        private readonly IEncryptionService encryptionService;
        
        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public EmailSender(IEncryptionService encryptionService)
        {
            this.encryptionService   = encryptionService;
        }

        #endregion

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        public void SendEmail(EmailAccount emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null)
        {
            SendEmail(emailAccount, subject, body, 
                new MailAddress(fromAddress, fromName), new MailAddress(toAddress, toName), 
                bcc, cc);
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="from">From address</param>
        /// <param name="to">To address</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses ist</param>
        public virtual void SendEmail(EmailAccount emailAccount, string subject, string body,
            MailAddress from, MailAddress to,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null)
        {
            var message = new MailMessage();
            message.From = from;
            
#if DEBUG
            message.To.Add(new MailAddress("deepankar@netsouls.net","eCetral Debug"));
#else
            message.To.Add(to);
#endif
            if (null != bcc)
            {
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }
            if (null != cc)
            {
                foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }
            message.Subject = subject;
            message.Body = body; // get to set templating
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host                  = emailAccount.Host;
                smtpClient.Port                  = emailAccount.Port;
                smtpClient.EnableSsl             = emailAccount.EnableSsl;

                if (emailAccount.UseDefaultCredentials)
                    smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                else
                {
                    // decrypt the password
                    var emailPassword = encryptionService.AESDecrypt(emailAccount.Password);
                    smtpClient.Credentials = new NetworkCredential(emailAccount.Username, emailPassword);
                }
                smtpClient.Send(message);
            }
        }
    }
}
