using eCentral.Services.Localization;
using eCentral.Web.Models.Directory;
using FluentValidation;

namespace eCentral.Web.Validators.Directory
{
    public class CountryValidator : AbstractValidator<CountryModel>
    {
        public CountryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Configuration.Countries.Fields.Name.Required"));

            RuleFor(x => x.TwoLetterIsoCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Configuration.Countries.Fields.TwoLetterIsoCode.Required"));
            RuleFor(x => x.TwoLetterIsoCode)
                .Length(2)
                .WithMessage(localizationService.GetResource("Configuration.Countries.Fields.TwoLetterIsoCode.Length"));

            RuleFor(x => x.ThreeLetterIsoCode)
                .NotNull()
                .WithMessage(localizationService.GetResource("Configuration.Countries.Fields.ThreeLetterIsoCode.Required"));
            RuleFor(x => x.ThreeLetterIsoCode)
                .Length(3)
                .WithMessage(localizationService.GetResource("Configuration.Countries.Fields.ThreeLetterIsoCode.Length"));
        }
    }
}