using System;
using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Messages
{
    public class EmailAccountSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a store default email account identifier
        /// </summary>
        public Guid DefaultEmailAccountId { get; set; }
    }

}
