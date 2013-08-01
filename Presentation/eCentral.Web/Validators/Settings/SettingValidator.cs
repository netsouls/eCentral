using eCentral.Services.Localization;
using eCentral.Web.Models.Settings;
using FluentValidation;

namespace eCentral.Web.Validators.Settings
{
    public class SettingValidator : AbstractValidator<SettingModel>
    {
        public SettingValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotNull().WithMessage(localizationService.GetResource("Settings.Advanced.Fields.Name.Required"));
            RuleFor(x => x.Value).NotNull().WithMessage(localizationService.GetResource("Settings.Advanced.Fields.Value.Required"));
        }
    }
}