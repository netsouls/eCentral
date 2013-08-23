using System;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Directory;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Directory
{
    [Validator(typeof(PortValidator))]
    public partial class PortModel : BaseEntityModel
    {
        public Guid CountryId { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Ports.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SiteResourceDisplayName("Configuration.Countries.Ports.Fields.Abbreviation")]
        [AllowHtml]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets whether the port is sea
        /// </summary>
        [SiteResourceDisplayName("Configuration.Countries.Ports.Fields.Sea")]
        public bool IsSea { get; set; }

        /// <summary>
        /// Gets or sets whether the port is air
        /// </summary>
        [SiteResourceDisplayName("Configuration.Countries.Ports.Fields.Air")]
        public bool IsAir { get; set; }
    }
}