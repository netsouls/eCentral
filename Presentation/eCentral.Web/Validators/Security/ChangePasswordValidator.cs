using eCentral.Core;
using eCentral.Core.Domain.Security;
using eCentral.Services.Localization;
using eCentral.Web.Models.Security;
using FluentValidation;

namespace eCentral.Web.Validators.Security
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator(ILocalizationService localizationService, SecuritySettings securitySettings)
        {
            RuleFor(x => x.OldPassword).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.OldPassword.Required"));
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.NewPassword.Required"));
            RuleFor(x => x.NewPassword).Length(securitySettings.PasswordMinLength, 999).WithMessage(string.Format(localizationService.GetResource("Security.Fields.Password.LengthValidation"), securitySettings.PasswordMinLength))
                .Matches(StateKeyManager.PasswordValidationRegEx).WithMessage(localizationService.GetResource("Security.Fields.Password.IncorrectFormat"));
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.ConfirmPassword.Required"));
            RuleFor(x => x.ConfirmNewPassword).Equal(x => x.NewPassword).WithMessage(localizationService.GetResource("Security.Fields.Password.EnteredPasswordsDoNotMatch"));
        }
    }
}