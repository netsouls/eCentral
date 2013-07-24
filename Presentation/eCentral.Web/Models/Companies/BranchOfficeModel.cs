using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Models.Common;
using eCentral.Web.Validators.Companies;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Companies
{
    [Validator(typeof(BranchOfficeValidator))]
    public partial class BranchOfficeModel : BaseAuditHistoryModel
    {
        public BranchOfficeModel()
        {
            this.Address = new AddressModel();
        }

        [SiteResourceDisplayName("BranchOffice.Fields.Name")]
        [AllowHtml]
        public string BranchName { get; set; }

        [SiteResourceDisplayName("BranchOffice.Fields.Abbreviation")]
        [AllowHtml]
        public string Abbreviation { get; set; }

        public AddressModel Address { get; set; }
    }
}