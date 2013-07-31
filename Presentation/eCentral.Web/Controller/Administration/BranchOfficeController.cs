using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Companies;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Companies;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Web.Extensions;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Common;
using eCentral.Web.Models.Companies;

namespace eCentral.Web.Controllers.Administration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class BranchOfficeController : BaseController
    {
        #region Fields

        private readonly IBranchOfficeService dataService;
        private readonly IWorkContext workContext;
        private readonly ILocalizationService localizationService;
        private readonly ICacheManager cacheManager; 
        private readonly ICountryService countryService;
        private readonly IStateProvinceService stateprovinceService;

        #endregion

        #region Ctor

        public BranchOfficeController(IBranchOfficeService dataService, ILocalizationService localizationService,
            ICountryService countryService, IStateProvinceService stateprovinceService,
            IWorkContext workContext, ICacheManager cacheManager )
        {
            this.dataService          = dataService;
            this.localizationService  = localizationService;
            this.cacheManager         = cacheManager;
            this.countryService       = countryService;
            this.stateprovinceService = stateprovinceService;
            this.workContext          = workContext;
        }

        #endregion

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageBranchOffices)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageBranchOffices)]
        [HttpPost]
        public ActionResult List(JQueryDataTableParamModel command)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            string cacheKey = ModelCacheEventUser.OFFICE_MODEL_KEY.FormatWith(
                    "List");

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var offices = dataService.GetAll(PublishingStatus.All)
                    .Select(office => PrepareBranchOfficeModel(office));

                return offices;
            });

            return Json(new DataTablesParser<BranchOfficeModel>(Request, cacheModel).Parse());
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageBranchOffices)]
        public ActionResult Create()
        {
            var model = new BranchOfficeModel();
            PrepareAddEditModel(model);

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageBranchOffices)]
        public ActionResult Create(BranchOfficeModel model)
        {
            if (ModelState.IsValid)
            {
                var office = new BranchOffice
                {
                    BranchName = model.BranchName, 
                    Abbreviation = model.Abbreviation,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CurrentPublishingStatus = PublishingStatus.Active // by default the client status is active
                };

                if (!string.IsNullOrEmpty(model.Address.Address1) ||
                    !string.IsNullOrEmpty(model.Address.City) || !string.IsNullOrEmpty(model.Address.PhoneNumber) ||
                    !string.IsNullOrEmpty(model.Address.FaxNumber) || model.Address.CountryId.HasValue || model.Address.StateProvinceId.HasValue)
                {
                    office.Address = model.Address.ToEntity();
                    office.Address.CreatedOn = office.Address.UpdatedOn = DateTime.UtcNow;
                }

                // we need to add this client 
                dataService.Insert(office);

                // return notification message
                SuccessNotification(localizationService.GetResource("BranchOffice.Added"));
                return RedirectToAction(SystemRouteNames.Index);
            }

            //If we got this far, something failed, redisplay form
            PrepareAddEditModel(model);

            return View(model);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageBranchOffices)]
        public ActionResult Edit(Guid rowId)
        {
            if (rowId.IsEmpty())
                return RedirectToAction(SystemRouteNames.Index);

            var office = dataService.GetById(rowId);

            if (office == null)
                return RedirectToAction(SystemRouteNames.Index);

            var model = office.ToModel();
            PrepareAddEditModel(model);
            model.IsEdit = true;

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageBranchOffices)]
        public ActionResult Edit(BranchOfficeModel model)
        {
            if (ModelState.IsValid)
            {
                var office = dataService.GetById(model.RowId);

                if (office == null)
                    return RedirectToAction(SystemRouteNames.Index);

                // update the properties
                model.Address.ToEntity( office.Address );
                office.UpdatedOn = office.Address.UpdatedOn = DateTime.UtcNow;
                
                // we need to update this client 
                dataService.Update(office);

                // return notification message
                SuccessNotification(localizationService.GetResource("BranchOffice.Updated"));
                return RedirectToAction(SystemRouteNames.Index);
            }

            var errors = ModelState.GetModelErrors();
            //If we got this far, something failed, redisplay form
            PrepareAddEditModel(model);

            return View(model);
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult CheckNameAvailability(string branchName)
        {
            var nameAvailable = false;
            if (branchName != null)
            {
                branchName = branchName.Trim();

                nameAvailable = dataService.IsUnique(branchName);
            }

            return Json(nameAvailable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeStatus ( ChangeStatusModel model )
        {
            var publishingStatus = CommonHelper.To<PublishingStatus>(model.StatusId);

            // update the client status
            model.RowIds.ForEach(rowId =>
            {
                dataService.ChangeStatus(rowId, publishingStatus);
            });

            return Json("Selected records status has been changed!");
        }

        #region Utilities

        [NonAction]
        private BranchOfficeModel PrepareBranchOfficeModel(BranchOffice office)
        {
            Guard.IsNotNull(office, "office");

            var model = new BranchOfficeModel()
                {
                    RowId = office.RowId,
                    Abbreviation = office.Abbreviation,
                    BranchName = office.BranchName,
                    Address = office.Address.ToModel()
                };

            PrepareAuditHistoryModel(model, office);

            return model;
        }

        [NonAction]
        private void PrepareAddEditModel(BranchOfficeModel model)
        {
            Guard.IsNotNull(model, "model");

            // add countries
            var countryId = model.Address.CountryId.HasValue ? model.Address.CountryId.Value : Guid.Empty;

            countryService.GetAll()
            .ForEach(c =>
            {
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.RowId.ToString(), Selected = (c.RowId == countryId) });
            });

            if (!countryId.IsEmpty())
            {
                var states = stateprovinceService.GetByCountryId(countryId);
                if (states.Count > 0)
                {
                    states.ForEach(s =>
                    {
                        model.Address.AvailableStates.Add(new SelectListItem()
                        {
                            Text = s.GetLocalized(x => x.Name),
                            Value = s.RowId.ToString(),
                            Selected = (model.Address.StateProvinceId.HasValue ? s.RowId == model.Address.StateProvinceId : false)
                        });
                    });
                }
            }           
        }

        #endregion
    }
}
