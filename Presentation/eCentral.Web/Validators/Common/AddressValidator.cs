using eCentral.Services.Localization;
using eCentral.Web.Models.Common;
using FluentValidation;

namespace eCentral.Web.Validators.Common
{
    public class AddressValidator : AbstractValidator<AddressModel>
    {
        public AddressValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.CountryId)
                .NotNull()
                .WithMessage(localizationService.GetResource("Address.Fields.Country.Required"));
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.City.Required"));
            /*RuleFor(x => x.Address1)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.Address1.Required"));
            RuleFor(x => x.ZipPostalCode)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));*/
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Address.Fields.PhoneNumber.Required"));
        }
    }
}