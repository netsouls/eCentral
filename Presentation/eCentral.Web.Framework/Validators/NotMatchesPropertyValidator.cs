using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using FluentValidation.Internal;
using FluentValidation.Mvc;
using FluentValidation.Validators;

namespace eCentral.Web.Framework.Validators
{
    public class NotMatchesPropertyValidator : PropertyValidator
    {
        readonly string expression;
        readonly Regex regex;

        public NotMatchesPropertyValidator(string expression)
            : base(() => "{PropertyName} must be checked.")
        {
            this.expression = expression;
            regex = new Regex(expression);
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue != null &&
                regex.IsMatch((string)context.PropertyValue))
                return false;

            return true;
        }

        public string Expression
        {
            get { return expression; }
        }
    }

    public class NotMatchesFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public NotMatchesFluentValidationPropertyValidator(ModelMetadata metadata, ControllerContext controllerContext,
            PropertyRule rule, IPropertyValidator validator)
            : base(metadata, controllerContext, rule, validator)
        {

        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            if (!ShouldGenerateClientSideRules()) yield break;

            var validator = Validator as NotMatchesPropertyValidator;

            var formatter = new MessageFormatter().AppendPropertyName(Rule.GetDisplayName());
            string errorMessage = formatter.BuildMessage(validator.ErrorMessageSource.GetString());

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = "nomatchregex"
            };
            rule.ValidationParameters.Add("nomatchpattern", validator.Expression);
            yield return rule;
        }
    }
}
