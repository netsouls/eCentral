using eCentral.Services.Localization;
using eCentral.Web.Models.Messages;
using FluentValidation;

namespace eCentral.Web.Validators.Messages
{
    public class MessageTemplateValidator : AbstractValidator<MessageTemplateModel>
    {
        public MessageTemplateValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Subject).NotNull().WithMessage(localizationService.GetResource("Configuration.MessageTemplates.Fields.Subject.Required"));
            RuleFor(x => x.Body).NotNull().WithMessage(localizationService.GetResource("Configuration.MessageTemplates.Fields.Body.Required"));
        }
    }
}