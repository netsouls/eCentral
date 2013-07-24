﻿using System;
using eCentral.Core.Infrastructure;
using eCentral.Services.Logging;
using eCentral.Services.Tasks;

namespace eCentral.Services.Messages
{
    /// <summary>
    /// Represents a task for sending queued message 
    /// </summary>
    public partial class QueuedMessagesSendTask : ITask
    {

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var queuedEmailService = EngineContext.Current.Resolve<IQueuedEmailService>();
            var emailSender = EngineContext.Current.Resolve<IEmailSender>();

            var maxTries = 3;
            var queuedEmails = queuedEmailService.SearchEmails(null, null, null, null,
                true, maxTries, 0, 10000);
            foreach (var queuedEmail in queuedEmails)
            {
                var bcc = String.IsNullOrWhiteSpace(queuedEmail.Bcc) 
                            ? null 
                            : queuedEmail.Bcc.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var cc = String.IsNullOrWhiteSpace(queuedEmail.CC) 
                            ? null 
                            : queuedEmail.CC.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    emailSender.SendEmail(queuedEmail.EmailAccount, queuedEmail.Subject, queuedEmail.Body,
                       queuedEmail.From, queuedEmail.FromName, queuedEmail.To, queuedEmail.ToName, bcc, cc);

                    queuedEmail.SentOn = DateTime.UtcNow;
                }
                catch (Exception exc)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Error(string.Format("Error sending e-mail. {0}", exc.Message), exc);
                }
                finally
                {
                    queuedEmail.SentTries = queuedEmail.SentTries + 1;
                    queuedEmailService.Update(queuedEmail);
                }
            }
        }
    }
}
