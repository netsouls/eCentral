using eCentral.Services.Localization;
using eCentral.Web.Models.Directory;
using FluentValidation;

namespace eCentral.Web.Validators.Directory
{
    public class PortValidator : AbstractValidator<PortModel>
    {
        public PortValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Configuration.Countries.Ports.Fields.Name.Required"));

            RuleFor(x => x.Abbreviation)
                .NotNull()
                .WithMessage(localizationService.GetResource("Configuration.Countries.Ports.Fields.Abbreviation.Required"));
        }
    }
}