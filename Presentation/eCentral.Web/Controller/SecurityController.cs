using System;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Users;
using eCentral.Services.Authentication;
using eCentral.Services.Users;
using eCentral.Web.Models.Security;

namespace eCentral.Web.Controllers
{
    public class SecurityController : BaseController
    {
        #region Fields

        private readonly IAuthenticationService authenticationService;
        private readonly IWorkContext workContext;
        private readonly IUserService userService;
        private readonly IUserRegistrationService userRegistrationService;

        #endregion

        #region Ctor

        public SecurityController(IAuthenticationService authenticationService,
            IUserRegistrationService userRegistrationService,
            IWorkContext workContext, IUserService userService)
        {
            this.authenticationService   = authenticationService;
            this.workContext             = workContext;
            this.userRegistrationService = userRegistrationService;
            this.userService             = userService;            
        }

        #endregion

        #region Login / Logout

        public ActionResult Login()
        {
            // if the user is coming on the login page again, and session exists, redirect the user to the home page
            if (workContext.IsAuthenticated)
                return RedirectToRoute(SystemRouteNames.HomePage);

            var model = new LoginModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                model.UserName = model.UserName.Trim();

                DataResult<User> validUser = userRegistrationService.ValidateUser(model.UserName, model.Password);

                if (validUser.Success)
                {
                    authenticationService.SignIn(validUser.Data, model.RememberMe);

                    // user is valid, redirect to the default page or requested page
                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToRoute(SystemRouteNames.HomePage);
                }
                else 
                    model.Result = validUser.ErrorMessages;
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Logout()
        {
            //standard logout
            workContext.SignOut();
            return this.RedirectToRoute(SystemRouteNames.Login);
        }

        [HttpPost]
        public JsonResult VerifyUser(string userName, string hashPassword)
        {
            Guard.IsNotNullOrEmpty(userName, "username");
            Guard.IsNotNullOrEmpty(hashPassword, "hashPassword");

            // get this user, if the login credentials match, then its a valid user
            var result = userRegistrationService.ValidateUser(userName, hashPassword);

            return Json(new
            {
                IsValid = result.Success,
                Message = result.Errors
            });
        }

        #endregion

        #region Utilities
        
        #endregion
    }
}
