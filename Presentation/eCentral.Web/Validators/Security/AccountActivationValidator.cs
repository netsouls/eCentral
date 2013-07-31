using FluentValidation;
using eCentral.Core;
using eCentral.Core.Domain.Users;
using eCentral.Services.Localization;
using eCentral.Services.Users;
using eCentral.Web.Models.Security;

namespace eCentral.Web.Validators.Security
{
    public class AccountActivationValidator : AbstractValidator<AccountActivationModel>
    {
        public AccountActivationValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.Password.Required"))
                .Length(userSettings.PasswordMinLength, 999).
                    WithMessage(string.Format(localizationService.GetResource("Security.Fields.Password.LengthValidation"), userSettings.PasswordMinLength))
                .Matches(StateKeyManager.PasswordValidationRegEx).WithMessage(localizationService.GetResource("Security.Fields.Password.IncorrectFormat"));

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.ConfirmPassword.Required"))
                .Equal(x => x.Password).WithMessage(localizationService.GetResource("Security.Fields.Password.EnteredPasswordsDoNotMatch"));
        }
    }
}