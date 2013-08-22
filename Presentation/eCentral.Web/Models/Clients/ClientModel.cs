using System.Collections.Generic;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Models.Common;
using eCentral.Web.Validators.Clients;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Clients
{
    [Validator(typeof(ClientValidator))]
    public partial class ClientModel : BaseAuditHistoryModel, IBranchOfficeAssociation
    {
        public ClientModel()
        {
            this.Address = new AddressModel();
            this.AvailableOffices = new List<SelectListItem>();
        }

        [SiteResourceDisplayName("Client.Fields.Name")]
        [AllowHtml]
        public string ClientName { get; set; }

        [SiteResourceDisplayName("Client.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public AddressModel Address { get; set; }

        /// <summary>
        /// Gets or set the associated offices
        /// </summary>
        public IList<string> Offices { get; set; }

        /// <summary>
        /// Gets or sets the selected associated offices identifiers
        /// </summary>
        [SiteResourceDisplayName("Users.Fields.Offices")]
        public string OfficeId { get; set; }

        /// <summary>
        /// Gets or sets the available branch offices
        /// </summary>
        public IList<SelectListItem> AvailableOffices { get; set; }
    }
}