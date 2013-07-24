using System;
using System.Text;
using eCentral.Core.Domain.Directory;

namespace eCentral.Core.Domain.Common
{
    public class Address : BaseEntity, ICloneable
    {
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public virtual Guid? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public virtual Guid? StateProvinceId { get; set; }
        
        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public virtual string City { get; set; }

        /// <summary>
        /// Gets or sets the address 1
        /// </summary>
        public virtual string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2
        /// </summary>
        public virtual string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip/postal code
        /// </summary>
        public virtual string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number
        /// </summary>
        public virtual string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime UpdatedOn { get; set; }
        
        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }
        
        public object Clone()
        {
            var addr = new Address()
            {
                Country = this.Country,
                CountryId = this.CountryId,
                StateProvince = this.StateProvince,
                StateProvinceId = this.StateProvinceId,
                City = this.City,
                Address1 = this.Address1,
                Address2 = this.Address2,
                ZipPostalCode = this.ZipPostalCode,
                PhoneNumber = this.PhoneNumber,
                FaxNumber = this.FaxNumber,
                CreatedOn = this.CreatedOn,
                UpdatedOn = this.UpdatedOn
            };
            return addr;
        }

        #region Public Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // set values 
            builder.Append(string.Format("Address 1: [{0}]", this.Address1.HasValue() ? this.Address1.Trim(): string.Empty)).Append
                (string.Format(", Address 2: [{0}]", this.Address2.HasValue() ? this.Address2.Trim() : string.Empty)).Append
             (string.Format(", City: [{0}]", this.City.Trim())).Append
             (string.Format(", State: [{0}]", this.StateProvince.Name.Trim())).Append
             (string.Format(", Country: [{0}]", this.Country.Name.Trim())).Append
             (string.Format(", Zip Code: [{0}]", this.ZipPostalCode.HasValue() ? this.ZipPostalCode.Trim() : string.Empty)).Append
             (string.Format(", Phone: [{0}]", this.PhoneNumber.Trim())).Append
             (string.Format(", Fax: [{0}]", this.FaxNumber.HasValue() ? this.FaxNumber.Trim() : string.Empty));

            return builder.ToString();
        }

        #endregion
    }
}