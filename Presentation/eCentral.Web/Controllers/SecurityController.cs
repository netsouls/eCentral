using System;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Users;
using eCentral.Services.Authentication;
using eCentral.Services.Common;
using eCentral.Services.Messages;
using eCentral.Services.Users;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.Security;

namespace eCentral.Web.Controllers
{
    public class SecurityController : BaseController
    {
        #region Fields

        private readonly IAuthenticationService authenticationService;
        private readonly IWorkContext workContext;
        private readonly IUserService userService;
        private readonly IWorkflowMessageService messageService;
        private readonly IGenericAttributeService attributeService;
        private readonly IUserRegistrationService userRegistrationService;

        #endregion

        #region Ctor

        public SecurityController(IAuthenticationService authenticationService,
            IUserRegistrationService userRegistrationService, IGenericAttributeService attributeService,
            IWorkContext workContext, IUserService userService, IWorkflowMessageService messageService)
        {
            this.authenticationService   = authenticationService;
            this.workContext             = workContext;
            this.messageService          = messageService;
            this.attributeService        = attributeService;
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
                    ErrorNotification(validUser.ErrorMessages);
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

        #region Activation 

        public ActionResult AccountActivation(Guid userId, Guid token)
        {
            // check for the user
            var user = ValidateUserActivation(userId, token, SystemUserAttributeNames.AccountActivationToken);

            if (user == null)
                return RedirectToRoute(SystemRouteNames.Login);

            // valid user with a valid token, we need him to set the security settings
            var model = new AccountActivationModel
            {
                UserId = user.RowId,
                Token = token
            };

            PrepareAccountActivationModel(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult AccountActivation(AccountActivationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = ValidateUserActivation(model.UserId, model.Token, SystemUserAttributeNames.AccountActivationToken);

                // set the question and answers and the security verification image for this user in the database
                if (user != null)
                {
                    // set this user unique password
                    ChangePasswordRequest changeRequest = new ChangePasswordRequest(user.Username,
                        false, model.Password);
                    var changeResult = userRegistrationService.ChangePassword(changeRequest);

                    if (changeResult.Success)
                    {
                        // activate this user account now
                        user.CurrentPublishingStatus = PublishingStatus.Active;
                        user.LastActivityDate = DateTime.UtcNow;
                        userService.Update(user);

                        // need to delete the activation token
                        attributeService.SaveAttribute(user, SystemUserAttributeNames.AccountActivationToken, string.Empty);

                        SuccessNotification("Your account has been activated");
                        return RedirectToRoute(SystemRouteNames.Login);
                    }

                    ErrorNotification(changeResult.ErrorMessages);
                }                
            }

            // reached here, means error, show the form again 
            PrepareAccountActivationModel(model);
            return View(model);
        }

        #endregion

        #region Password Recovery

        public ActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            return View(model);
        }

        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        public ActionResult PasswordRecoverySend(PasswordRecoveryModel model)
        {
            if (ModelState.IsValid)
            {
                var user = userService.GetByUsername(model.UserName);
                if (user != null && user.CurrentPublishingStatus == PublishingStatus.Active)
                {
                    var passwordRecoveryToken = Guid.NewGuid();
                    attributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken, passwordRecoveryToken.ToString());
                    messageService.SendUserPasswordRecoveryMessage(user, workContext.WorkingLanguage.RowId);

                    SuccessNotification("Email with instructions to reset your password has been sent to you.");
                    return RedirectToRoute(SystemRouteNames.Login);
                }
                else
                    ErrorNotification("The specified user does not exists");

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult PasswordRecoveryConfirm(Guid userId, Guid token)
        {
            // check for the user
            var user = ValidateUserActivation(userId, token, SystemUserAttributeNames.PasswordRecoveryToken);

            if (user == null)
                return RedirectToRoute(SystemRouteNames.Login);

            // valid user with a valid token, we need him to set the security settings
            var model = new AccountActivationModel
            {
                UserId = user.RowId,
                Token = token
            };

            PrepareAccountActivationModel(model);
            return View(model);
        }

        [HttpPost]
        [FormValueRequired("set-password")]
        public ActionResult PasswordRecoveryConfirm(AccountActivationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = ValidateUserActivation(model.UserId, model.Token, SystemUserAttributeNames.PasswordRecoveryToken);

                // set the question and answers and the security verification image for this user in the database
                if (user != null)
                {
                    // set this user unique password
                    ChangePasswordRequest changeRequest = new ChangePasswordRequest(user.Username,
                        false, model.Password);
                    var changeResult = userRegistrationService.ChangePassword(changeRequest);

                    if (changeResult.Success)
                    {
                        // need to delete the password token
                        attributeService.SaveAttribute(user, SystemUserAttributeNames.PasswordRecoveryToken, string.Empty);

                        SuccessNotification("Your password has been changed");
                        return RedirectToRoute(SystemRouteNames.Login);
                    }

                    ErrorNotification(changeResult.ErrorMessages);
                }
            }

            // reached here, means error, show the form again 
            PrepareAccountActivationModel(model);
            return View(model);
        }

        #endregion

        #region Utilities

        [NonAction]
        private void PrepareAccountActivationModel(AccountActivationModel model)
        {
            Guard.IsNotNull(model, "model");

            // retrieve the user
            var user = userService.GetById(model.UserId);
            if (user != null)
            {
                model.ContactName = user.GetFullName();
                model.UserName = user.Username;
            }
        }

        [NonAction]
        private User ValidateUserActivation(Guid userId, Guid token, string attributeName)
        {
            // need to get the user details 
            var user = userService.GetByGuid(userId);

            if (user == null)
                return null; // in-correct user

            // check whether the user is active or not when account activation
            if ( attributeName.IsCaseInsensitiveEqual(SystemUserAttributeNames.AccountActivationToken))
                if (user.CurrentPublishingStatus != PublishingStatus.PendingApproval)
                    return null; // user has already activated his/her account
            else if ( attributeName.IsCaseInsensitiveEqual(SystemUserAttributeNames.PasswordRecoveryToken))
                if (user.CurrentPublishingStatus != PublishingStatus.Active)
                    return null; // user has already activated his/her account

            var uToken = user.GetAttribute<string>(attributeName);
            if (string.IsNullOrEmpty(uToken))
                return null; // user has already activated his/her account

            if (!uToken.IsCaseInsensitiveEqual(token.ToString()))
                return null; // in-correct token

            return user;
        }

        #endregion
    }
}
