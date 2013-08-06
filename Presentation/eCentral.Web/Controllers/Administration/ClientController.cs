using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Clients;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Clients;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Services.Security.Cryptography;
using eCentral.Web.Extensions;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Clients;
using eCentral.Web.Models.Common;

namespace eCentral.Web.Controllers.Administration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    [RoleAuthorization(Role = SystemUserRoleNames.Users)]
    public partial class ClientController : BaseController
    {
        #region Fields

        private readonly IClientService clientService;
        private readonly IWorkContext workContext;
        private readonly ILocalizationService localizationService;
        private readonly ICacheManager cacheManager; 
        private readonly ICountryService countryService;
        private readonly IStateProvinceService stateprovinceService;
        private readonly IEncryptionService encryptionService;

        #endregion

        #region Ctor

        public ClientController(IClientService clientService, ILocalizationService localizationService,
            ICountryService countryService, IStateProvinceService stateprovinceService,
            IEncryptionService encryptionService, IWorkContext workContext, ICacheManager cacheManager )
        {
            this.clientService        = clientService;
            this.localizationService  = localizationService;
            this.encryptionService    = encryptionService;
            this.cacheManager         = cacheManager;
            this.countryService       = countryService;
            this.stateprovinceService = stateprovinceService;
            this.workContext          = workContext;
        }

        #endregion

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult List(JQueryDataTableParamModel command)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            string cacheKey = ModelCacheEventUser.CLIENT_MODEL_KEY.FormatWith(
                    "List");

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var clients = clientService.GetAll(PublishingStatus.All)
                    .Select(client => PrepareClientModel(client));

                return clients;
            });

            return Json(new DataTablesParser<ClientModel>(Request, cacheModel).Parse());
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult Create()
        {
            var model = new ClientModel();
            PrepareAddEditModel(model);

            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult Create(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                var client = new Client
                {
                    ClientName = model.ClientName,
                    Email = model.Email,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CurrentPublishingStatus = PublishingStatus.Active // by default the client status is active
                };

                if (!string.IsNullOrEmpty(model.Address.Address1) ||
                    !string.IsNullOrEmpty(model.Address.City) || !string.IsNullOrEmpty(model.Address.PhoneNumber) ||
                    !string.IsNullOrEmpty(model.Address.FaxNumber) || model.Address.CountryId.HasValue || model.Address.StateProvinceId.HasValue)
                {
                    client.Address = model.Address.ToEntity();
                    client.Address.CreatedOn = client.Address.UpdatedOn = DateTime.UtcNow;
                }

                // we need to add this client 
                clientService.Insert(client);

                // return notification message
                SuccessNotification(localizationService.GetResource("Clients.Added"));
                return RedirectToAction(SystemRouteNames.Index);
            }

            //If we got this far, something failed, redisplay form
            PrepareAddEditModel(model);

            return View(model);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult Edit(Guid rowId)
        {
            if (rowId.IsEmpty())
                return RedirectToAction(SystemRouteNames.Index);

            var client = clientService.GetById(rowId);

            if (client == null)
                return RedirectToAction(SystemRouteNames.Index);

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
                    return RedirectToAction(SystemRouteNames.Index);

                // update the properties
                client.Email = model.Email;
                model.Address.ToEntity( client.Address );
                client.UpdatedOn = client.Address.UpdatedOn = DateTime.UtcNow;
                
                // we need to update this client 
                clientService.Update(client);

                // return notification message
                SuccessNotification(localizationService.GetResource("Clients.Updated"));
                return RedirectToAction(SystemRouteNames.Index);
            }

            var errors = ModelState.GetModelErrors();
            //If we got this far, something failed, redisplay form
            PrepareAddEditModel(model);

            return View(model);
        }

        [HttpGet]
        [ValidateInput(false)]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult CheckNameAvailability(string clientName)
        {
            var nameAvailable = false;
            if (clientName != null)
            {
                clientName = clientName.Trim();

                nameAvailable = clientService.IsUnique(clientName);
            }

            return Json(nameAvailable, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageClients)]
        public ActionResult ChangeStatus(ChangeStatusModel model)
        {
            var publishingStatus = CommonHelper.To<PublishingStatus>(model.StatusId);

            // update the client status
            model.RowIds.ForEach(rowId =>
            {
                clientService.ChangeStatus(rowId, publishingStatus);
            });

            return Json("Selected records status has been changed!");
        }

        #region Utilities

        [NonAction]
        private ClientModel PrepareClientModel(Client client)
        {
            Guard.IsNotNull(client, "client");

            var model = new ClientModel()
                {
                    RowId = client.RowId,
                    Email =  client.Email,
                    ClientName = client.ClientName,
                    Address = client.Address.ToModel()
                };

            PrepareAuditHistoryModel(model, client);

            return model;
        }

        [NonAction]
        private void PrepareAddEditModel(ClientModel model)
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
