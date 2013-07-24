using System.Web.Mvc;
using eCentral.Services.Localization;
using eCentral.Services.Users;
using eCentral.Web.Framework.Validators;
using eCentral.Web.Models.Users;
using FluentValidation;

namespace eCentral.Web.Validators.Users
{
    public class UserValidator : AbstractValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService, IUserService userService)
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Users.Fields.Username.Required"))
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Common.WrongEmail"));

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Users.Fields.FirstName.Required"));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Users.Fields.LastName.Required"));

            RuleFor(x => x.Mobile)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Users.Fields.Mobile.Required"));

            When(x => !x.IsEdit, () =>
            {
                RuleFor(x => x.Username).IsUnique(userService, localizationService.GetResource("Users.Fields.Username.NotUnique"),
                    "CheckNameAvailability", "User", HttpVerbs.Get, string.Empty);
            });        
        }
    }
}