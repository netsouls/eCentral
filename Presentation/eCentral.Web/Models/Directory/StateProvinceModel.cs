using System;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Directory;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Directory
{
    [Validator(typeof(StateProvinceValidator))]
    public partial class StateProvinceModel : BaseEntityModel
    {
        public Guid CountryId { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.States.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.States.Fields.Abbreviation")]
        [AllowHtml]
        public string Abbreviation { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}