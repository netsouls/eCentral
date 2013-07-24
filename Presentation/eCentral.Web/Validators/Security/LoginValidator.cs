using eCentral.Core.Domain.Users;
using eCentral.Services.Localization;
using eCentral.Web.Models.Security;
using FluentValidation;

namespace eCentral.Web.Validators.Security
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator(ILocalizationService localizationService, UserSettings userSettings)
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.Email.Required"))
                .EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));

            // on login page just do validation for not empty
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Security.Fields.Password.Required"));
        }
    }
}