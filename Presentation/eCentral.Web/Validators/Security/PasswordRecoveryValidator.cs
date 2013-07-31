using eCentral.Core.Domain.Users;
using eCentral.Services.Localization;
using eCentral.Web.Models.Security;
using FluentValidation;

namespace eCentral.Web.Validators.Security
{
    public class PasswordRecoveryValidator : AbstractValidator<PasswordRecoveryModel>
        {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
            {
                RuleFor(x => x.UserName).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.Email.Required"))
                    .EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            }
        }
}