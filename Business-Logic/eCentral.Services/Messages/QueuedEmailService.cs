using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain.Messages;
using eCentral.Services.Events;

namespace eCentral.Services.Messages
{
    public partial class QueuedEmailService:IQueuedEmailService
    {
        private readonly IRepository<QueuedEmail> queuedEmailRepository;
        private readonly IEventPublisher eventPublisher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="queuedEmailRepository">Queued email repository</param>
        /// <param name="eventPublisher">Event published</param>
        public QueuedEmailService(IRepository<QueuedEmail> queuedEmailRepository, IEventPublisher eventPublisher)
        {
            this.queuedEmailRepository = queuedEmailRepository;
            this.eventPublisher        = eventPublisher;
        }

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>        
        public virtual void Insert(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            queuedEmailRepository.Insert(queuedEmail);

            //event notification
            eventPublisher.EntityInserted(queuedEmail);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void Update(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            queuedEmailRepository.Update(queuedEmail);

            //event notification
            eventPublisher.EntityUpdated(queuedEmail);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        public virtual void Delete(QueuedEmail queuedEmail)
        {
            if (queuedEmail == null)
                throw new ArgumentNullException("queuedEmail");

            queuedEmailRepository.Delete(queuedEmail);

            //event notification
            eventPublisher.EntityDeleted(queuedEmail);
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>Queued email</returns>
        public virtual QueuedEmail GetById(Guid queuedEmailId)
        {
            if (queuedEmailId.IsEmpty())
                return null;

            var queuedEmail = queuedEmailRepository.GetById(queuedEmailId);
            return queuedEmail;

        }

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>Queued emails</returns>
        public virtual IList<QueuedEmail> GetByIds(Guid[] queuedEmailIds)
        {
            if (queuedEmailIds == null || queuedEmailIds.Length == 0)
                return new List<QueuedEmail>();

            var query = from qe in queuedEmailRepository.Table
                        where queuedEmailIds.Contains(qe.RowId)
                        select qe;
            var queuedEmails = query.ToList();
            //sort by passed identifiers
            var sortedQueuedEmails = new List<QueuedEmail>();
            foreach (Guid id in queuedEmailIds)
            {
                var queuedEmail = queuedEmails.Find(x => x.RowId == id);
                if (queuedEmail != null)
                    sortedQueuedEmails.Add(queuedEmail);
            }
            return sortedQueuedEmails;
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="startTime">The start time</param>
        /// <param name="endTime">The end time</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Email item list</returns>
        public virtual IPagedList<QueuedEmail> SearchEmails(string fromEmail, string toEmail, DateTime? startTime, DateTime? endTime, 
            bool loadNotSentItemsOnly, int maxSendTries, int pageIndex, int pageSize)
        {
            fromEmail = (fromEmail ?? String.Empty).Trim();
            toEmail = (toEmail ?? String.Empty).Trim();
            
            var query = queuedEmailRepository.Table;
            if (!String.IsNullOrEmpty(fromEmail))
                query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!String.IsNullOrEmpty(toEmail))
                query = query.Where(qe => qe.To.Contains(toEmail));
            if (startTime.HasValue)
                query = query.Where(qe => qe.CreatedOn >= startTime);
            if (endTime.HasValue)
                query = query.Where(qe => qe.CreatedOn <= endTime);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOn.HasValue);
            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = query.OrderByDescending(qe => qe.Priority).ThenByDescending(qe => qe.CreatedOn);

            var queuedEmails = new PagedList<QueuedEmail>(query, pageIndex, pageSize);
            return queuedEmails;
        }
    }
}
