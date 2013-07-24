using System.Web.Mvc;
using FluentValidation;
using eCentral.Services;

namespace eCentral.Web.Framework.Validators
{
    public static class MyValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> IsCreditCard<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new FluentValidation.Validators.CreditCardValidator());
        }

        public static IRuleBuilderOptions<T, TElement> IsUnique<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder, IPropertyValidatorService service,
            string errorMessage, string action, string controller, HttpVerbs httpVerb, string additionalFields)
        {
            return ruleBuilder.SetValidator(new IsUniquePropertyValidator(service,
                errorMessage, action, controller, httpVerb, additionalFields));
        }

        public static IRuleBuilderOptions<T, TElement> IsChecked<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new IsCheckedPropertyValidator());
        }

        /// <summary>
        /// Defines a regular expression validator on the current rule builder, but only for string properties.
        /// Validation will fail if the value returned by the lambda matches the regular expression.
        /// </summary>
        /// <typeparam name="T">Type of object being validated</typeparam>
        /// <param name="ruleBuilder">The rule builder on which the validator should be defined</param>
        /// <param name="expression">The regular expression to check the value against.</param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, string> NotMatches<T>(this IRuleBuilder<T, string> ruleBuilder, string expression)
        {
            return ruleBuilder.SetValidator(new NotMatchesPropertyValidator(expression));
        }
    }
}
