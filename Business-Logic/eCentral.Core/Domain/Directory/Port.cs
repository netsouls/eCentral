using System;
using eCentral.Core.Domain.Localization;

namespace eCentral.Core.Domain.Directory
{
    /// <summary>
    /// Represents a port
    /// </summary>
    public partial class Port : BaseEntity
    {
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public virtual Guid CountryId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public virtual string Abbreviation { get; set; }

        /// <summary>
        /// Gets or set whether the its a sea port
        /// </summary>
        public bool IsSea { get; set; }

        /// <summary>
        /// Gets or set whether the its a air port
        /// </summary>
        public bool IsAir { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual Country Country { get; set; }
    }

}
