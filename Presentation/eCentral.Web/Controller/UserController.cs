using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Common;
using eCentral.Services.Localization;
using eCentral.Services.Messages;
using eCentral.Services.Security.Cryptography;
using eCentral.Services.Users;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Common;
using eCentral.Web.Models.Users;

namespace eCentral.Web.Controllers
{
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class UserController : BaseController
    {
        #region Fields

        private readonly IUserService userService;
        private readonly IWorkflowMessageService messageService;
        private readonly IUserRegistrationService registrationService;
        private readonly IWorkContext workContext;
        private readonly ILocalizationService localizationService;
        private readonly ICacheManager cacheManager;
        private readonly IEncryptionService encryptionService;

        #endregion

        #region Ctor

        public UserController(IUserService userService, ILocalizationService localizationService,
            IEncryptionService encryptionService, IUserRegistrationService registrationService,
            IWorkflowMessageService messageService,
            IWorkContext workContext, ICacheManager cacheManager)
        {
            this.localizationService  = localizationService;
            this.cacheManager         = cacheManager;
            this.encryptionService    = encryptionService;
            this.registrationService  = registrationService;
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

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        [HttpPost]
        public ActionResult List(JQueryDataTableParamModel command)
        {
            if (!Request.IsAjaxRequest())
                return Redirect(Url.RouteUrl(SystemRouteNames.User));

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
        public ActionResult Add()
        {
            var model = new UserModel();
            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageUsers)]
        public ActionResult Add(UserModel model)
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
                    // send the user activation email
                    messageService.SendUserEmailValidationMessage(result.Data, workContext.WorkingLanguage.RowId);

                    //activityService.InsertActivity(workContext.CurrentEnterprise, workContext.CurrentUser, result.Data.RowId,
                    //    SystemActivityLogTypeNames.Enterprise.AddNewUser, result.Data.FormatUserName(), request.Tags);

                    // return notification message
                    SuccessNotification(localizationService.GetResource("Users.Added"));
                    return RedirectToRoute(SystemRouteNames.User);
                }
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        /*[PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult Edit(Guid rowId)
        {
            if (rowId.IsEmpty())
                return RedirectToRoute(SystemRouteNames.Client);

            var client = clientService.GetById(rowId);

            if (client == null)
                return RedirectToRoute(SystemRouteNames.Client);

            var model = client.ToModel();
            PrepareAddEditModel(model);
            model.IsEdit = true;

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult Edit(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                var client = clientService.GetById(model.RowId);

                if (client == null)
                    return RedirectToRoute(SystemRouteNames.Client);

                // update the properties
                client.Email = model.Email;
                model.Address.ToEntity( client.Address );
                client.UpdatedOn = client.Address.UpdatedOn = DateTime.UtcNow;
                
                // we need to update this client 
                clientService.Update(client);

                // return notification message
                SuccessNotification(localizationService.GetResource("Clients.Updated"));
                return RedirectToRoute(SystemRouteNames.Client);
            }

            var errors = ModelState.GetModelErrors();
            //If we got this far, something failed, redisplay form
            PrepareAddEditModel(model);

            return View(model);
        }*/

        [HttpGet]
        [ValidateInput(false)]
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

        /*[HttpPost]
        public ActionResult ChangeStatus ( ChangeStatusModel model )
        {
            var publishingStatus = CommonHelper.To<PublishingStatus>(model.StatusId);

            // update the client status
            model.RowIds.ForEach(rowId =>
            {
                userService.ChangeStatus(rowId, publishingStatus);
            });

            return Json("Selected records status has been changed!");
        }*/

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
