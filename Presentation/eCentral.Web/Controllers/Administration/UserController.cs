using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Common;
using eCentral.Services.Companies;
using eCentral.Services.Localization;
using eCentral.Services.Messages;
using eCentral.Services.Security.Cryptography;
using eCentral.Services.Users;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Common;
using eCentral.Web.Models.Users;

namespace eCentral.Web.Controllers.Administration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class UserController : BaseController
    {
        #region Fields

        private readonly IUserService userService;
        private readonly IWorkflowMessageService messageService;
        private readonly IBranchOfficeService officeService;
        private readonly IUserRegistrationService registrationService;
        private readonly IWorkContext workContext;
        private readonly IGenericAttributeService attributeService;
        private readonly ILocalizationService localizationService;
        private readonly ICacheManager cacheManager;
        private readonly IEncryptionService encryptionService;

        #endregion

        #region Ctor

        public UserController(IUserService userService, ILocalizationService localizationService,
            IEncryptionService encryptionService, IUserRegistrationService registrationService,
            IWorkflowMessageService messageService, IGenericAttributeService attributeService,
            IWorkContext workContext, ICacheManager cacheManager, IBranchOfficeService officeService)
        {
            this.localizationService  = localizationService;
            this.cacheManager         = cacheManager;
            this.encryptionService    = encryptionService;
            this.officeService        = officeService;
            this.registrationService  = registrationService;
            this.attributeService     = attributeService;
            this.messageService       = messageService;
            this.userService          = userService;
            this.workContext          = workContext;
        }

        #endregion

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult List(JQueryDataTableParamModel command)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            string cacheKey = ModelCacheEventUser.USERS_MODEL_KEY.FormatWith(
                    "List");

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var clients = userService.GetAll(PublishingStatus.All)
                    .Select(client => PrepareUserModel(client));

                return clients;
            });

            return Json(new DataTablesParser<UserModel>(Request, cacheModel).Parse());
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult Create()
        {
            var model = new UserModel();
            model.AvailableOffices = base.PrepareSelectList(officeService, cacheManager);

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var request = new UserRegistrationRequest( 
                    model.Username, encryptionService.GetSHAHash(StateKeyManager.TemporaryPassword, true), PublishingStatus.PendingApproval);

                // set the other properties
                request.FirstName = model.FirstName;
                request.LastName = model.LastName;
                request.Mobile = model.Mobile;
                request.IsAdministrator = model.IsAdministrator;

                // register this user
                var result = registrationService.RegisterUser(request);
                if (result.Success)
                {
                    // add the associated branches to this user
                    attributeService.SaveAttribute<List<Guid>>(result.Data, SystemUserAttributeNames.AssociatedBrancOffices,
                        model.OfficeId.ToList<Guid>());

                    // send the user activation email
                    messageService.SendUserEmailValidationMessage(result.Data, workContext.WorkingLanguage.RowId);

                    // return notification message
                    SuccessNotification(localizationService.GetResource("Users.Added"));
                    return RedirectToAction(SystemRouteNames.Index);
                }
            }

            //If we got this far, something failed, redisplay form
            model.AvailableOffices = base.PrepareSelectList(officeService, cacheManager);
            return View(model);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult Edit(Guid rowId)
        {
            if (rowId.IsEmpty())
                return RedirectToAction(SystemRouteNames.Index);

            var user = userService.GetById(rowId);

            if (user == null)
                return RedirectToAction(SystemRouteNames.Index);

            var model = PrepareUserModel(user);
            model.IsEdit = true;

            // retrieve extra properties
            model.Mobile = encryptionService.AESDecrypt(user.GetAttribute<string>(SystemUserAttributeNames.Mobile), user);
            model.IsAdministrator = user.IsInUserRole(SystemUserRoleNames.Administrators);
            var associatedOffices = user.GetAttribute<List<Guid>>(SystemUserAttributeNames.AssociatedBrancOffices);
            if (associatedOffices != null && associatedOffices.Count > 0)
                model.OfficeId = associatedOffices.ToDelimitedString();
            model.AvailableOffices = base.PrepareSelectList(officeService, cacheManager);

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult Edit(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var request = new UserRegistrationRequest(
                    model.Username, string.Empty);

                // set the other properties
                request.FirstName = model.FirstName;
                request.LastName = model.LastName;
                request.Mobile = model.Mobile;
                request.IsAdministrator = model.IsAdministrator;

                // register this user
                var result = registrationService.UpdateRegistration(request);
                if (result.Success)
                {
                    // add the associated branches to this user
                    attributeService.SaveAttribute<List<Guid>>(result.Data, SystemUserAttributeNames.AssociatedBrancOffices,
                        model.OfficeId.ToList<Guid>());

                    // return notification message
                    SuccessNotification(localizationService.GetResource("Users.Updated"));
                    return RedirectToAction(SystemRouteNames.Index);
                }
            }

            //If we got this far, something failed, redisplay form
            model.AvailableOffices = base.PrepareSelectList(officeService, cacheManager);

            var user = userService.GetById(model.RowId);
            if (user == null)
                return RedirectToAction(SystemRouteNames.Index);

            model.Mobile = encryptionService.AESDecrypt(user.GetAttribute<string>(SystemUserAttributeNames.Mobile), user);
            model.IsAdministrator = user.IsInUserRole(SystemUserRoleNames.Administrators);

            return View(model);
        }

        [HttpGet]
        [ValidateInput(false)]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult CheckNameAvailability(string userName)
        {
            var nameAvailable = false;
            if (userName != null)
            {
                userName = userName.Trim();

                nameAvailable = userService.IsUnique(userName);
            }

            return Json(nameAvailable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult ChangeStatus(ChangeStatusModel model)
        {
            var publishingStatus = CommonHelper.To<PublishingStatus>(model.StatusId);

            // update the client status
            model.RowIds.ForEach(rowId =>
            {
                registrationService.ChangeStatus(rowId, publishingStatus);
            });

            return Json("Selected records status has been changed!");
        }

        #region Utilities

        [NonAction]
        private UserModel PrepareUserModel(User user)
        {
            Guard.IsNotNull(user, "user");

            var model = new UserModel()
            {
                RowId = user.RowId, 
                Username = user.Username, 
                FirstName = encryptionService.AESDecrypt( user.GetAttribute<string>(SystemUserAttributeNames.FirstName), user), 
                LastName = encryptionService.AESDecrypt( user.GetAttribute<string>(SystemUserAttributeNames.LastName), user),
                UserRoleNames = user.UserRoles.Select(ur => ur.Name).ToDelimitedString(", "),
                CreatedOn = user.CreatedOn.ToString(StateKeyManager.DateFormat), 
                LastActivityDate = user.LastActivityDate.HasValue ? user.LastActivityDate.Value.ToString(StateKeyManager.DateTimeFormat) : string.Empty
            };

            PrepareAuditHistoryModel(model, user);

            return model;
        }

        #endregion
    }
}
