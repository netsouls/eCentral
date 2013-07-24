using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Internal;
using FluentValidation.Mvc;
using FluentValidation.Validators;
using eCentral.Services;

namespace eCentral.Web.Framework.Validators
{
    public class IsUniquePropertyValidator : PropertyValidator
    {
        private readonly IPropertyValidatorService _service;
        
        public string Url { get; private set; }
        public string HttpMethod { get; private set; }
        public string AdditionalFields { get; private set; }

        public IsUniquePropertyValidator(IPropertyValidatorService service, string errorMessage,
                         string action,
                         string controller,
                         HttpVerbs HttpVerb = HttpVerbs.Get,
                         string additionalFields = "")
            : base(errorMessage) //"{PropertyName} must be unique."
        {
            this._service = service;
            var httpContext = HttpContext.Current;

            if (httpContext != null)
            {
                var httpContextBase = new HttpContextWrapper(httpContext);
                var routeData       = new RouteData();
                var requestContext  = new RequestContext(httpContextBase, routeData);

                var helper       = new UrlHelper(requestContext);
                Url              = helper.Action(action, controller);
                HttpMethod       = HttpVerb.ToString();
                AdditionalFields = additionalFields;
            }
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var uniqueValue = context.PropertyValue as string;
            if (String.IsNullOrWhiteSpace(uniqueValue))
                return false;

            return this._service.IsUnique(context.PropertyValue);
        }
    }

    public class IsUniqueFluentValidationPropertyValidator : FluentValidationPropertyValidator
    {
        public IsUniqueFluentValidationPropertyValidator(ModelMetadata metaData, ControllerContext controllerContext, PropertyRule rule, IPropertyValidator validator)
            : base(metaData, controllerContext, rule, validator)
        {

        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            if (!this.ShouldGenerateClientSideRules())
            {
                yield break;
            }

            var validator = Validator as IsUniquePropertyValidator;

            var errorMessage = new MessageFormatter()
                .AppendPropertyName(Rule.GetDisplayName())
                //.AppendArgument("ValueToCompare", validator.ValueToCompare)
                .BuildMessage(validator.ErrorMessageSource.GetString());

            //This is the rule that asp.net mvc 3, uses for Remote attribute.
            yield return new ModelClientValidationRemoteRule(errorMessage, validator.Url, validator.HttpMethod, validator.AdditionalFields);
        }
    }
}
