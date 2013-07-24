using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Models.Common;
using eCentral.Web.Validators.Clients;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Clients
{
    [Validator(typeof(ClientValidator))]
    public partial class ClientModel : BaseAuditHistoryModel
    {
        public ClientModel()
        {
            this.Address = new AddressModel();
        }

        [SiteResourceDisplayName("Client.Fields.Name")]
        [AllowHtml]
        public string ClientName { get; set; }

        [SiteResourceDisplayName("Client.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        public AddressModel Address { get; set; }
    }
}