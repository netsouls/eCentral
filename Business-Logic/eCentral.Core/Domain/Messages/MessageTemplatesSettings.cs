﻿using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Messages
{
    public class MessageTemplatesSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether to replace message tokens according to case invariant rules
        /// </summary>
        public bool CaseInvariantReplacement { get; set; }

    }

}
