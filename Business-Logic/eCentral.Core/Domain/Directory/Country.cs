using System.Collections.Generic;
using eCentral.Core.Domain.Localization;

namespace eCentral.Core.Domain.Directory
{
    /// <summary>
    /// Represents a country
    /// </summary>
    public partial class Country : BaseEntity, ILocalizedEntity
    {
        private ICollection<StateProvince> _stateProvinces;
        private ICollection<Port> _ports;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the two letter ISO code
        /// </summary>
        public virtual string TwoLetterIsoCode { get; set; }

        /// <summary>
        /// Gets or sets the three letter ISO code
        /// </summary>
        public virtual string ThreeLetterIsoCode { get; set; }

        /// <summary>
        /// Gets or sets the numeric ISO code
        /// </summary>
        public virtual int NumericIsoCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public virtual bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public virtual int DisplayOrder { get; set; }
       
        /// <summary>
        /// Gets or sets the state/provinces
        /// </summary>
        public virtual ICollection<StateProvince> StateProvinces
        {
            get { return _stateProvinces ?? (_stateProvinces = new List<StateProvince>()); }
            protected set { _stateProvinces = value; }
        }

        /// <summary>
        /// Gets or sets the ports
        /// </summary>
        public virtual ICollection<Port> Ports
        {
            get { return _ports ?? (_ports = new List<Port>()); }
            protected set { _ports = value; }
        }
    }

}
