using System.Web.Mvc;
using eCentral.Services.Localization;

namespace eCentral.Web.Controllers
{
    public class StaticController : BaseController
    {
        #region Fields 

        private ILocalizationService localizationService;

        #endregion

        #region Constructors

        public StaticController( ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        #endregion

        public ActionResult SiteClosed()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
