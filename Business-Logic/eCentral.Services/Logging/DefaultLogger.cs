using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Users;

namespace eCentral.Services.Logging
{
    /// <summary>
    /// Default logger
    /// </summary>
    public partial class DefaultLogger : ILogger
    {
        #region Fields

        private readonly IRepository<Log> logRepository;
        private readonly IWebHelper webHelper;

        #endregion
        
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="logRepository">Log repository</param>
        /// <param name="webHelper">Web helper</param>
        public DefaultLogger(IRepository<Log> logRepository, IWebHelper webHelper)
        {
            this.logRepository   = logRepository;
            this.webHelper       = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        public virtual bool IsEnabled(LogLevel level)
        {
            switch(level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        public virtual void Delete(Log log)
        {
            Guard.IsNotNull(log, "log");

            logRepository.Delete(log);
        }

        /// <summary>
        /// Clears a log
        /// </summary>
        public virtual void ClearLog()
        {
            var log = logRepository.Table.ToList();
            foreach (var logItem in log)
                logRepository.Delete(logItem);
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <returns>Log item collection</returns>
        public virtual IList<Log> GetAll(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel)
        {
            var query = logRepository.Table;
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOn);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOn);
            if (logLevel.HasValue)
            {
                int logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }
             if (!String.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOn);

            var log = query.ToList();
            return log;
        }

        /// <summary>
        /// Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Log item collection</returns>
        public virtual IPagedList<Log> GetAll(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel, int pageIndex, int pageSize)
        {
            var query = logRepository.Table;
            if (fromUtc.HasValue)
                query = query.Where(l => fromUtc.Value <= l.CreatedOn);
            if (toUtc.HasValue)
                query = query.Where(l => toUtc.Value >= l.CreatedOn);
            if (logLevel.HasValue)
            {
                int logLevelId = (int)logLevel.Value;
                query = query.Where(l => logLevelId == l.LogLevelId);
            }
            if (!String.IsNullOrEmpty(message))
                query = query.Where(l => l.ShortMessage.Contains(message) || l.FullMessage.Contains(message));
            query = query.OrderByDescending(l => l.CreatedOn);

            var log = new PagedList<Log>(query, pageIndex, pageSize);
            return log;
        }

        /// <summary>
        /// Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual Log GetById(Guid logId)
        {
            if (logId.IsEmpty())
                return null;

            var log = logRepository.GetById(logId);
            return log;
        }

        /// <summary>
        /// Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        public virtual IList<Log> GetByIds(Guid[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            var query = from l in logRepository.Table
                        where logIds.Contains(l.RowId)
                        select l;
            var logItems = query.ToList();
            //sort by passed identifiers
            var sortedLogItems = new List<Log>();
            foreach (Guid id in logIds)
            {
                var log = logItems.Find(x => x.RowId == id);
                if (log != null)
                    sortedLogItems.Add(log);
            }
            return sortedLogItems;
        }

        /// <summary>
        /// Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="customer">The customer to associate log record with</param>
        /// <returns>A log item</returns>
        public virtual Log Insert(LogLevel logLevel, string shortMessage, string fullMessage = "", User user = null)
        {
            var log = new Log()
            {
                LogLevel     = logLevel,
                ShortMessage = shortMessage,
                FullMessage  = fullMessage,
                IpAddress    = webHelper.GetCurrentIpAddress(),
                User         = user,
                PageUrl      = webHelper.GetThisPageUrl(true),
                ReferrerUrl  = webHelper.GetUrlReferrer(),
                CreatedOn    = DateTime.UtcNow
            };

            logRepository.Insert(log);

            return log;
        }

        #endregion
    }
}
