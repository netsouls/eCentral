using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Internal;
using FluentValidation.Mvc;
using FluentValidation.Validators;

namespace eCentral.Web.Framework.Validators
{
    public class IsCheckedPropertyValidator : PropertyValidator
    {
        public IsCheckedPropertyValidator()
            : base("{PropertyName} must be checked.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue != null)
                return (bool)context.PropertyValue;

            return false;   
        }
    }

    public class IsCheckedFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public IsCheckedFluentValidationPropertyValidator(ModelMetadata metaData, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
            : base(metaData, controllerContext, rule, validator)
        {

        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            if (!this.ShouldGenerateClientSideRules())
            {
                yield break;
            }

            var validator = Validator as IsCheckedPropertyValidator;

            var errorMessage = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                //.AppendArgument("ValueToCompare", validator.ValueToCompare)
                .BuildMessage(validator.ErrorMessageSource.GetString());

            var rule = new ModelClientValidationRule
            {
                ErrorMessage   = errorMessage,
                ValidationType = "equalischecked"
            };

            rule.ValidationParameters["other"] = true;
            yield return rule;
        }
    }
}
