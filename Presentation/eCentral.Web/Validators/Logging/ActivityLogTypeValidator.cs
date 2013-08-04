using eCentral.Services.Localization;
using eCentral.Web.Models.Logging;
using FluentValidation;

namespace eCentral.Web.Validators.Logging
{
    public class ActivityLogTypeValidator : AbstractValidator<ActivityLogTypeModel>
    {
        public ActivityLogTypeValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.SystemKeyword).NotEmpty();
        }
    }
}