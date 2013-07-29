using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Domain.Security;
using eCentral.Core.Infrastructure;
using eCentral.Services.Configuration;
using eCentral.Services.Localization;
using eCentral.Services.Messages;
using eCentral.Services.Security;
using eCentral.Services.Security.Cryptography;
using eCentral.Web.Extensions;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Models.Messages;

namespace eCentral.Web.Controllers.Configuration
{
	public class EmailAccountController : BaseController
	{
        private readonly IEmailAccountService emailAccountService;
        private readonly ILocalizationService localizationService;
        private readonly ISettingService settingService;
        private readonly IEmailSender emailSender;
        private readonly EmailAccountSettings emailAccountSettings;
        private readonly SiteInformationSettings siteSettings;
        private readonly IPermissionService permissionService;

        #region Ctor

        public EmailAccountController(IEmailAccountService emailAccountService,
            ILocalizationService localizationService, ISettingService settingService, 
            IEmailSender emailSender, 
            EmailAccountSettings emailAccountSettings, SiteInformationSettings siteSettings,
            IPermissionService permissionService)
		{
            this.emailAccountService = emailAccountService;
            this.localizationService = localizationService;
            this.emailAccountSettings = emailAccountSettings;
            this.emailSender = emailSender;
            this.settingService = settingService;
            this.siteSettings = siteSettings;
            this.permissionService = permissionService;
		}

        #endregion

        #region Methods 

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        public ActionResult Index()
        {
            //mark as default email account (if selected)
            if (!String.IsNullOrEmpty(Request["rowId"]))
            {
                Guid defaultEmailAccountId = CommonHelper.To<Guid>(Request["rowId"]);
                var defaultEmailAccount = emailAccountService.GetById(defaultEmailAccountId);
                if (defaultEmailAccount != null)
                {
                    emailAccountSettings.DefaultEmailAccountId = defaultEmailAccountId;
                    settingService.Save(emailAccountSettings);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        [HttpPost]
        public ActionResult List()
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            var emailAccounts = emailAccountService.GetAll()
                    .Select(email => PrepareEmailModel(email));

            return Json(new DataTablesParser<EmailAccountModel>(Request, emailAccounts).Parse());
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        public ActionResult Create()
        {
            var model = new EmailAccountModel();
            //default values
            model.Port = 25;
            return View(model);
        }

        [HttpPost]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        public ActionResult Create(EmailAccountModel model)
        {
            if (ModelState.IsValid)
            {
                var emailAccount = model.ToEntity();

                // encrypt the password 
                var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
                emailAccount.Password = encryptionService.AESEncrypt(model.Password);

                emailAccountService.Insert(emailAccount);

                SuccessNotification(localizationService.GetResource("Configuration.EmailAccounts.Added"));
                return RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        public ActionResult Edit(Guid rowId)
        {
            var emailAccount = emailAccountService.GetById(rowId);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("Index");

            var model = emailAccount.ToModel();
            model.IsEdit = true;

            // decrypt the password 
            var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
            model.Password = encryptionService.AESDecrypt(emailAccount.Password);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        public ActionResult Edit(EmailAccountModel model)
        {
            var emailAccount = emailAccountService.GetById(model.RowId);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                emailAccount = model.ToEntity(emailAccount);

                // encrypt the password 
                var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
                emailAccount.Password = encryptionService.AESEncrypt(model.Password);

                emailAccountService.Update(emailAccount);

                SuccessNotification(localizationService.GetResource("Configuration.EmailAccounts.Updated"));
                return RedirectToAction("Index");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("sendtestemail")]
        [PermissionAuthorization(Permission = SystemPermissionNames.ManageEmailAccounts)]
        public ActionResult SendTestEmail(EmailAccountModel model)
        {
            var emailAccount = emailAccountService.GetById(model.RowId);
            if (emailAccount == null)
                //No email account found with the specified id
                return RedirectToAction("Index");

            try
            {
                if (String.IsNullOrWhiteSpace(model.SendTestEmailTo))
                    throw new SiteException("Enter test email address");


                var from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                var to = new MailAddress(model.SendTestEmailTo);
                string subject = siteSettings.SiteName + ". Testing email functionality.";
                string body = "Email works fine.";
                emailSender.SendEmail(emailAccount, subject, body, from, to);
                SuccessNotification(localizationService.GetResource("Configuration.EmailAccounts.SendTestEmail.Success"), false);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message, false);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Utilities

        [NonAction]
        private EmailAccountModel PrepareEmailModel(EmailAccount email)
        {
            var model = email.ToModel();

            model.IsDefaultEmailAccount = email.RowId == emailAccountSettings.DefaultEmailAccountId;
            return model;
        }

        #endregion
	}
}
