using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Directory;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Directory
{
    [Validator(typeof(CountryValidator))]
    public class CountryModel : BaseEntityModel
    {
        [SiteResourceDisplayName("Configuration.Countries.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Fields.TwoLetterIsoCode")]
        [AllowHtml]
        public string TwoLetterIsoCode { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Fields.ThreeLetterIsoCode")]
        [AllowHtml]
        public string ThreeLetterIsoCode { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Fields.NumericIsoCode")]
        public int NumericIsoCode { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Fields.Published")]
        public bool Published { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Fields.NumberOfStates")]
        public int NumberOfStates { get; set; }
    }
}