using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Web.Infrastructure.Cache;

namespace eCentral.Web.Controllers
{
    public partial class CountryController : BaseController
    {
        #region Fields

        private readonly ICountryService countryService;
        private readonly IStateProvinceService stateProvinceService;
        private readonly ILocalizationService localizationService;
        private readonly IWorkContext workContext;
        private readonly ICacheManager cacheManager;

        #endregion

        #region Constructors

        public CountryController(ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this.countryService       = countryService;
            this.stateProvinceService = stateProvinceService;
            this.localizationService  = localizationService;
            this.workContext          = workContext;
            this.cacheManager         = cacheManager;
        }

        #endregion

        #region States / provinces

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetStatesByCountryId(string countryId, bool addEmptyStateIfRequired)
        {
            //this action method gets called via an ajax request
            if (String.IsNullOrEmpty(countryId))
                return Json(new {}, JsonRequestBehavior.AllowGet); // do not throw an exception return null.

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
