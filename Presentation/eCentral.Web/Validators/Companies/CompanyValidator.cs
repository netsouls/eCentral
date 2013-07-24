using System.Web.Mvc;
using eCentral.Core;
using eCentral.Services.Companies;
using eCentral.Services.Localization;
using eCentral.Web.Framework.Validators;
using eCentral.Web.Models.Companies;
using FluentValidation;

namespace eCentral.Web.Validators.Companies
{
    public class CompanyValidator : AbstractValidator<CompanyModel>
    {
        public CompanyValidator(ILocalizationService localizationService, ICompanyService companyService)
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Company.Fields.Name.Required"));

            RuleFor(x => x.Abbreviation)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Company.Fields.Abbreviation.Required"));

            RuleFor(x => x.LogoId).NotEmpty().WithMessage(localizationService.GetResource("Company.Fields.Logo.Required"))
                .Matches(StateKeyManager.GuidValidationRegEx).WithMessage(localizationService.GetResource("Company.Fields.Logo.Required"))
                .NotMatches(StateKeyManager.GuidEmptyValidationRegEx).WithMessage(localizationService.GetResource("Company.Fields.Logo.Required"));

            When(x => !x.IsEdit, () =>
            {
                RuleFor(x => x.CompanyName).IsUnique(companyService, localizationService.GetResource("Company.Fields.Name.NotUnique"),
                    "CheckNameAvailability", "Company", HttpVerbs.Get, string.Empty);
            });
        }
    }
}