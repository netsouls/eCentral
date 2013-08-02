using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Users;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;

namespace eCentral.Web.Controllers
{
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    public class APIController : BaseController
    {
        #region Fields

        private readonly IWorkContext workContext;
        private readonly ILocalizationService localizationService;
        private readonly ICountryService countryService;
        private readonly IStateProvinceService stateProvinceService;
        private readonly ICacheManager cacheManager;
        private readonly IWebHelper webHelper;
        
        #endregion

        #region Ctor

        public APIController(IWorkContext workContext, ILocalizationService localizationService,
            ICountryService countryService, IStateProvinceService stateProvinceService, 
            IWebHelper webHelper, ICacheManager cacheManager)
        {
            this.workContext = workContext;
            this.countryService = countryService;
            this.localizationService = localizationService;
            this.stateProvinceService = stateProvinceService;
            this.webHelper = webHelper;
            this.cacheManager = cacheManager;
        }

        #endregion

        #region States / provinces

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult StatesByCountry(string countryId, bool addEmptyStateIfRequired)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                return Json(new { }, JsonRequestBehavior.AllowGet); // do not throw an exception return null.

            var country = countryService.GetById(new Guid(countryId));
            if (country == null)
                //no country found
                return Json(new { }, JsonRequestBehavior.AllowGet);

            string cacheKey = string.Format(ModelCacheEventUser.STATEPROVINCES_BY_COUNTRY_MODEL_KEY, countryId, addEmptyStateIfRequired, workContext.WorkingLanguage.RowId);
            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var states = stateProvinceService.GetByCountryId(country.RowId).ToList();
                var result = (from s in states
                              select new { id = s.RowId, name = s.GetLocalized(x => x.Name) })
                              .ToList();

                if (addEmptyStateIfRequired && result.Count == 0)
                    result.Insert(0, new { id = Guid.Empty, name = localizationService.GetResource("Address.OtherNonUS") });
                return result;

            });

            return Json(cacheModel, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
