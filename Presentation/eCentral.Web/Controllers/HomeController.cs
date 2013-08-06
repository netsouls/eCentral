using System.Web.Mvc;
using eCentral.Core.Domain.Users;
using eCentral.Web.Framework.Controllers;

namespace eCentral.Web.Controllers
{
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    public class HomeController : BaseController
    {
        #region Fields 

        #endregion

        #region Constructors

        public HomeController( )
        {
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }
    }
}