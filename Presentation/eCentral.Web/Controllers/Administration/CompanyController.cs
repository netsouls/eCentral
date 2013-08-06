using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Companies;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Companies;
using eCentral.Services.Localization;
using eCentral.Services.Media;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Common;
using eCentral.Web.Models.Companies;

namespace eCentral.Web.Controllers.Administration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class CompanyController : BaseController
    {
        #region Fields

        private readonly ICompanyService companyService;
        private readonly IWorkContext workContext;
        private readonly ICacheManager cacheManager;
        private readonly IFileDataService fileDataService;
        private readonly ILocalizationService localizationService;
        

        #endregion

        #region Ctor

        public CompanyController(ICompanyService companyService, ILocalizationService localizationService,
            IWorkContext workContext, IFileDataService fileDataService, ICacheManager cacheManager )
        {
            this.companyService        = companyService;
            this.localizationService   = localizationService;
            this.workContext           = workContext;
            this.fileDataService       = fileDataService;
            this.cacheManager          = cacheManager;
        }

        #endregion

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageCompanies)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageCompanies)]
        [HttpPost]
        public ActionResult List(JQueryDataTableParamModel command)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            string cacheKey = ModelCacheEventUser.COMPANY_MODEL_KEY.FormatWith(
                    "List");

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var companies = companyService.GetAll(PublishingStatus.All)
                    .Select(company => PrepareCompanyMode(company));
                return companies;
            });

            return Json(new DataTablesParser<CompanyModel>( Request, cacheModel ).Parse() );
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult Create()
        {
            var model = new CompanyModel();

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageCompanies)]
        public ActionResult Create(CompanyModel model)
        {
            if (ModelState.IsValid)
            {
                var company = new Company
                {
                    CompanyName = model.CompanyName,
                    Abbreviation = model.Abbreviation,
                    CreatedOn = DateTime.UtcNow, 
                    LogoId = new Guid(model.LogoId),
                    UpdatedOn = DateTime.UtcNow,
                    CurrentPublishingStatus = PublishingStatus.Active // by default the client status is active
                };

                // we need to add this company
                companyService.Insert(company);

                // return notification message
                SuccessNotification(localizationService.GetResource("Company.Added"));
                return RedirectToAction(SystemRouteNames.Index);
            }

            return View(model);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult CheckNameAvailability(string companyName)
        {
            var nameAvailable = false;
            if (companyName != null)
            {
                companyName = companyName.Trim();

                nameAvailable = companyService.IsUnique(companyName);
            }

            return Json(nameAvailable, JsonRequestBehavior.AllowGet);
        }

        #region Utilities

        [NonAction]
        private CompanyModel PrepareCompanyMode( Company company )
        {
            Guard.IsNotNull(company, "company");

            var model = new CompanyModel()
                {
                    RowId = company.RowId,
                    CompanyName = company.CompanyName,
                    Abbreviation = company.Abbreviation,
                    LogoId = fileDataService.GetFileUrl(company.LogoId.Value),
                };

            PrepareAuditHistoryModel(model, company);

            return model;
        }

        #endregion
    }
}
