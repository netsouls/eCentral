using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Data;
using eCentral.Core.Domain.Messages;
using eCentral.Services.Events;

namespace eCentral.Services.Messages
{
    public partial class MessageTemplateService: IMessageTemplateService
    {
        #region Constants

        private const string MESSAGETEMPLATES_ALL_KEY     = "eCentral.messagetemplate.all";
        private const string MESSAGETEMPLATES_BY_ID_KEY   = "eCentral.messagetemplate.id-{0}";
        private const string MESSAGETEMPLATES_BY_NAME_KEY = "eCentral.messagetemplate.name-{0}";
        private const string MESSAGETEMPLATES_PATTERN_KEY = "eCentral.messagetemplate.";

        #endregion

        #region Fields

        private readonly IRepository<MessageTemplate> messageTemplateRepository;
        private readonly IEventPublisher eventPublisher;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="messageTemplateRepository">Message template repository</param>
        /// <param name="eventPublisher">Event published</param>
        public MessageTemplateService(ICacheManager cacheManager,
            IRepository<MessageTemplate> messageTemplateRepository,
            IEventPublisher eventPublisher)
        {
            this.cacheManager              = cacheManager;
            this.messageTemplateRepository = messageTemplateRepository;
            this.eventPublisher            = eventPublisher;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void Insert(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            messageTemplateRepository.Insert(messageTemplate);

            cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityInserted(messageTemplate);
        }

        /// <summary>
        /// Updates a message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        public virtual void Update(MessageTemplate messageTemplate)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException("messageTemplate");

            messageTemplateRepository.Update(messageTemplate);

            cacheManager.RemoveByPattern(MESSAGETEMPLATES_PATTERN_KEY);

            //event notification
            eventPublisher.EntityUpdated(messageTemplate);
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetById(Guid messageTemplateId)
        {
            if (messageTemplateId.IsEmpty())
                return null;

            string key = string.Format(MESSAGETEMPLATES_BY_ID_KEY, messageTemplateId);
            return cacheManager.Get(key, () =>
            {
                var manufacturer = messageTemplateRepository.GetById(messageTemplateId);
                return manufacturer;
            });
        }

        /// <summary>
        /// Gets a message template
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <returns>Message template</returns>
        public virtual MessageTemplate GetByName(string messageTemplateName)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                throw new ArgumentException("messageTemplateName");

            string key = string.Format(MESSAGETEMPLATES_BY_NAME_KEY, messageTemplateName);
            return cacheManager.Get(key, () =>
            {
                var query = from mt in messageTemplateRepository.Table
                                   where mt.Name == messageTemplateName
                                   select mt;
                return query.FirstOrDefault();
            });

        }

        /// <summary>
        /// Gets all message templates
        /// </summary>
        /// <returns>Message template list</returns>
        public virtual IList<MessageTemplate> GetAll()
        {
            return cacheManager.Get(MESSAGETEMPLATES_ALL_KEY, () =>
            {
                var query = from mt in messageTemplateRepository.Table
                            orderby mt.Name                            
                            select mt;
                var messageTemplates = query.ToList();
                return messageTemplates;
            });
        }

        #endregion
    }
}
