using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Web.Extensions;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.Directory;

namespace eCentral.Web.Controllers.Configuration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    [PermissionAuthorization(Permission = SystemPermissionNames.ManageCountries)]        
    public class CountryController : BaseController
    {
        #region Fields

        private readonly ICountryService countryService;
        private readonly IStateProvinceService stateProvinceService;
        private readonly ILocalizationService localizationService;

        #endregion Fields

        #region Constructors

        public CountryController(ICountryService countryService, 
            IStateProvinceService stateProvinceService, ILocalizationService localizationService)
        {
            this.countryService       = countryService;
            this.stateProvinceService = stateProvinceService;
            this.localizationService  = localizationService;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult List()
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            var countries = countryService.GetAll(true)
                    .Select(c => c.ToModel());

            return Json(new DataTablesParser<CountryModel>(Request, countries).Parse());
        }

        public ActionResult Create()
        {
            var model = new CountryModel();
            
            //default values
            model.Published = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(CountryModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var country = model.ToEntity();
                countryService.Insert(country);
                
                SuccessNotification(localizationService.GetResource("Configuration.Countries.Added"));
                return continueEditing ? RedirectToAction(SystemRouteNames.Edit, new { rowId = country.RowId }) : RedirectToAction(SystemRouteNames.Index);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(Guid rowId)
        {
            var country = countryService.GetById(rowId);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction(SystemRouteNames.Index);

            var model = country.ToModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(CountryModel model, bool continueEditing)
        {
            var country = countryService.GetById(model.RowId);
            if (country == null)
                //No country found with the specified id
                return RedirectToAction(SystemRouteNames.Index);

            if (ModelState.IsValid)
            {
                country = model.ToEntity(country);
                countryService.Update(country);

                SuccessNotification(localizationService.GetResource("Configuration.Countries.Updated"));
                return continueEditing ? RedirectToAction(SystemRouteNames.Edit, new { rowId = country.RowId }) : RedirectToAction(SystemRouteNames.Index);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #region States / provinces

        [HttpPost]
        public ActionResult States(Guid countryId)
        {
            var states = stateProvinceService.GetByCountryId(countryId, true)
                .Select(s => s.ToModel());

            return Json(new DataTablesParser<StateProvinceModel>(Request, states).Parse());
        }

        //create or update
        public ActionResult CreateOrUpdateState(Guid rowId, Guid countryId)
        {
            var model = new StateProvinceModel();

            if (rowId.IsEmpty())
                model.CountryId = countryId;
            else
            {
                var stateProvince = stateProvinceService.GetById(rowId);
                if ( stateProvince != null)
                {
                    model = stateProvince.ToModel();
                    model.IsEdit = true;
                }
            }

            return PartialView("_CreateOrUpdateState", model);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateState(StateProvinceModel model)
        {
            var country = countryService.GetById(model.CountryId);
            if (country == null)
                //No country found with the specified id
                return Json(new { IsValid = false, errorMessage = "Country does not exists" });

            if (ModelState.IsValid)
            {
                if (model.RowId.IsEmpty())
                    stateProvinceService.Insert(model.ToEntity());
                else
                {
                    var stateProvince = stateProvinceService.GetById(model.RowId);
                    if ( stateProvince == null )
                        //No state found with the specified id
                        return Json(new { IsValid = false, errorMessage = "State does not exists" });

                    stateProvince = model.ToEntity(stateProvince);
                    stateProvinceService.Update(stateProvince);
                }

                return Json( new {IsValid = true});
            }

            //If we got this far, something failed, redisplay form
            return Json(new { IsValid = false, htmlData = RenderPartialViewToString("_CreateOrUpdateState", model) });
        }

        #endregion

        #endregion
    }
}
