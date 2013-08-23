using eCentral.Core;
using eCentral.Core.Domain.Security;
using eCentral.Services.Localization;
using eCentral.Web.Models.Security;
using FluentValidation;

namespace eCentral.Web.Validators.Security
{
    public class AccountActivationValidator : AbstractValidator<AccountActivationModel>
    {
        public AccountActivationValidator(ILocalizationService localizationService, SecuritySettings securitySettings)
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.Password.Required"))
                .Length(securitySettings.PasswordMinLength, 999).
                    WithMessage(string.Format(localizationService.GetResource("Security.Fields.Password.LengthValidation"), securitySettings.PasswordMinLength))
                .Matches(StateKeyManager.PasswordValidationRegEx).WithMessage(localizationService.GetResource("Security.Fields.Password.IncorrectFormat"));

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.ConfirmPassword.Required"))
                .Equal(x => x.Password).WithMessage(localizationService.GetResource("Security.Fields.Password.EnteredPasswordsDoNotMatch"));
        }
    }
}