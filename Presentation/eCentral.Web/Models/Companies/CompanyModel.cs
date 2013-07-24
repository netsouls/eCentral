using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Companies;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Companies
{
    [Validator(typeof(CompanyValidator))]
    public partial class CompanyModel : BaseAuditHistoryModel
    {
        public CompanyModel()
        {
        }

        [SiteResourceDisplayName("Company.Fields.Name")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [SiteResourceDisplayName("Company.Fields.Abbreviation")]
        [AllowHtml]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or set the valid picture id
        /// This is a hack to validate on the client side that the image has been uploaded
        /// the value is set dynamically in the js equivalent to PictureId
        /// </summary>
        [SiteResourceDisplayName("Company.Fields.Logo")]
        public string LogoId { get; set; }
    }
}