using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Common;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Common
{
    [Validator(typeof(AddressValidator))]
    public partial class AddressModel : BaseEntityModel
    {
        public AddressModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
        }

        [SiteResourceDisplayName("Address.Fields.Country")]
        public Guid? CountryId { get; set; }
        [SiteResourceDisplayName("Address.Fields.Country")]
        [AllowHtml]
        public string CountryName { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.StateProvince")]
        public Guid? StateProvinceId { get; set; }
        [SiteResourceDisplayName("Address.Fields.StateProvince")]
        [AllowHtml]
        public string StateProvinceName { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.City")]
        [AllowHtml]
        public string City { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.Address1")]
        [AllowHtml]
        public string Address1 { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.Address2")]
        [AllowHtml]
        public string Address2 { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.ZipPostalCode")]
        [AllowHtml]
        public string ZipPostalCode { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.PhoneNumber")]
        [AllowHtml]
        public string PhoneNumber { get; set; }
        
        [SiteResourceDisplayName("Address.Fields.FaxNumber")]
        [AllowHtml]
        public string FaxNumber { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
    }
}