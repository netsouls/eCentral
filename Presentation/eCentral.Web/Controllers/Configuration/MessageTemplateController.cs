using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Localization;
using eCentral.Services.Messages;
using eCentral.Web.Extensions;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.Messages;

namespace eCentral.Web.Controllers.Configuration
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    [PermissionAuthorization(Permission = SystemPermissionNames.ManageMessageTemplates)]
    public class MessageTemplateController : BaseController
    {
        #region Fields

        private readonly IMessageTemplateService messageTemplateService;
        private readonly IEmailAccountService emailAccountService;
        private readonly ILanguageService languageService;
        private readonly ILocalizedEntityService localizedEntityService;
        private readonly ILocalizationService localizationService;
        private readonly IMessageTokenProvider messageTokenProvider;
        private readonly EmailAccountSettings emailAccountSettings;

        #endregion Fields

        #region Constructors

        public MessageTemplateController(IMessageTemplateService messageTemplateService, 
            IEmailAccountService emailAccountService, ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, IMessageTokenProvider messageTokenProvider, 
            EmailAccountSettings emailAccountSettings)
        {
            this.messageTemplateService = messageTemplateService;
            this.emailAccountService = emailAccountService;
            this.languageService = languageService;
            this.localizedEntityService = localizedEntityService;
            this.localizationService = localizationService;
            this.messageTokenProvider = messageTokenProvider;
            this.emailAccountSettings = emailAccountSettings;
        }

        #endregion
        
        #region Utilities

        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
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

            var messageTemplates = messageTemplateService.GetAll()
                    .Select(template => template.ToModel());

            return Json(new DataTablesParser<MessageTemplateModel>(Request, messageTemplates).Parse());
        }
        
        public ActionResult Edit(Guid rowId)
        {
            var messageTemplate = messageTemplateService.GetById(rowId);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("Index");


            var model = messageTemplate.ToModel();
            model.IsEdit = true;
            model.AllowedTokens = FormatTokens(messageTokenProvider.GetListOfAllowedTokens());
            //available email accounts
            foreach (var ea in emailAccountService.GetAll())
                model.AvailableEmailAccounts.Add(ea.ToModel());
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(MessageTemplateModel model)
        {
            var messageTemplate = messageTemplateService.GetById(model.RowId);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("Index");
            
            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);
                messageTemplateService.Update(messageTemplate);
                
                SuccessNotification(localizationService.GetResource("Configuration.MessageTemplates.Updated"));
                return RedirectToAction("Index");
            }


            //If we got this far, something failed, redisplay form
            model.AllowedTokens = FormatTokens(messageTokenProvider.GetListOfAllowedTokens());
            //available email accounts
            foreach (var ea in emailAccountService.GetAll())
                model.AvailableEmailAccounts.Add(ea.ToModel());
            return View(model);
        }
        
        #endregion
    }
}
