using System.Web.Mvc;
using eCentral.Services.Clients;
using eCentral.Services.Localization;
using eCentral.Web.Framework.Validators;
using eCentral.Web.Models.Clients;
using FluentValidation;

namespace eCentral.Web.Validators.Clients
{
    public class ClientValidator : AbstractValidator<ClientModel>
    {
        public ClientValidator(ILocalizationService localizationService, IClientService clientService)
        {
            RuleFor(x => x.ClientName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Clients.Fields.Name.Required"));

            When(x => !x.IsEdit, () =>
            {
                RuleFor(x => x.ClientName).IsUnique(clientService, localizationService.GetResource("Clients.Fields.Name.NotUnique"),
                    "CheckNameAvailability", "Client", HttpVerbs.Get, string.Empty);
            });

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Clients.Fields.Email.Required"))
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Common.WrongEmail"));

            RuleFor(x => x.OfficeId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Clients.Fields.Offices.Required"));
        }
    }
}