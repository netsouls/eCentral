using System.Web.Mvc;
using eCentral.Services.Companies;
using eCentral.Services.Localization;
using eCentral.Web.Framework.Validators;
using eCentral.Web.Models.Companies;
using FluentValidation;

namespace eCentral.Web.Validators.Companies
{
    public class BranchOfficeValidator : AbstractValidator<BranchOfficeModel>
    {
        public BranchOfficeValidator(ILocalizationService localizationService, IBranchOfficeService dataService)
        {
            RuleFor(x => x.BranchName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("BranchOffice.Fields.Name.Required"));

            RuleFor(x => x.Abbreviation)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("BranchOffice.Fields.Abbreviation.Required"));

            When(x => !x.IsEdit, () =>
            {
                RuleFor(x => x.BranchName).IsUnique(dataService, localizationService.GetResource("BranchOffice.Fields.Name.NotUnique"),
                    "CheckNameAvailability", "Client", HttpVerbs.Get, string.Empty);
            });
        }
    }
}