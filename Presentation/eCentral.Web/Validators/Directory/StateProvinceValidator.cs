using eCentral.Services.Localization;
using eCentral.Web.Models.Directory;
using FluentValidation;

namespace eCentral.Web.Validators.Directory
{
    public class StateProvinceValidator : AbstractValidator<StateProvinceModel>
    {
        public StateProvinceValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage(localizationService.GetResource("Configuration.Countries.States.Fields.Name.Required"));
        }
    }
}