using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Domain.Users;
using eCentral.Core.Infrastructure;
using eCentral.Services.Security;

namespace eCentral.Services.Installation
{
    public partial class InstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<UserRole> userRoleRepository;
        private readonly IRepository<MessageTemplate> messageTemplateRepository;
        private readonly IRepository<ActivityLogType> activityLogTypeRepository;
        private readonly IRepository<EmailAccount> emailAccountRepository;
        private readonly IWebHelper webHelper;

        #endregion

        #region Ctor

        public InstallationService(
            IRepository<UserRole> userRoleRepository,
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
            IRepository<ActivityLogType> activityLogTypeRepository,
            IWebHelper webHelper)
        {
            this.userRoleRepository        = userRoleRepository;
            this.messageTemplateRepository = messageTemplateRepository;
            this.activityLogTypeRepository = activityLogTypeRepository;
            this.emailAccountRepository    = emailAccountRepository;
            this.webHelper                 = webHelper;
        }

        #endregion

        #region Methods

        public virtual void InstallMessageTemplates()
        {
            var eaGeneral = emailAccountRepository.Table.Where(ea => ea.Email.Equals("no-reply@tranzlog.com")).FirstOrDefault();
            var messageTemplates = new List<MessageTemplate>
                               {
                                   new MessageTemplate
                                        {
                                            Name = "Templates.Master",
                                            Subject = "%Site.Name% Master Templace",
                                            Body = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><title>%Site.Name%</title></head><body><table width=\"700\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"background: #384b5c; border-bottom: 2px solid #a4c4e2;\"><img src=\"%Site.ImageURL%email-templates/logo.gif\" width=\"356\" height=\"134\" /></td></tr><tr><td style=\"background: #679cce; padding: 30px;\"><table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td style=\"font-family: Tahoma; font-size: 13px; color: #fff; line-height: 20px;\">%Message.Body%<p>Thanks,<br /><strong>SmartSignin Team</strong><br /><a style=\"color:#fff;text-decoration:underline;\" href=\"%Site.URL%\" target=\"_blank\">www.SmartSignin.com </a><br /><em>Your Smart, Simple and Secure Identity in the Cloud</em></p></td></tr></table></td></tr><tr><td style=\"background: #cfcfcf; border-bottom: 1px solid #989898; padding: 7px;\"><table border=\"0\" cellspacing=\"7\" cellpadding=\"1\" align=\"right\"><tr><td style=\"font-family:Tahoma;font-size:18px;color:#999;\">Stay in Touch&nbsp;</td><td><a href=\"http://www.facebook.com/smartsignin\" target=\"_blank\" title=\"Facebook\"><img src=\"%Site.ImageURL%email-templates/FB.gif\" alt=\"Facebook\" border=\"0\" /></a></td><td><a href=\"http://www.twitter.com/smartsignin\" target=\"_blank\" title=\"Twitter\"><img src=\"%Site.ImageURL%email-templates/TW.gif\" border=\"0\" /></a></td><td><a href=\"http://www.linkedin.com/company/smartsignin-inc-\" target=\"_blank\" title=\"LinkedIn\"><img src=\"%Site.ImageURL%email-templates/IN.gif\" border=\"0\" /></a></td><td><a href=\"http://www.youtube.com/user/SmartSignin?feature=watch\" target=\"_blank\" title=\"You Tube\"><img src=\"%Site.ImageURL%email-templates/YT.gif\" border=\"0\" /></a></td><td><a href=\"https://plus.google.com/b/114507917636567455532/\" target=\"_blank\" title=\"Google Plus\"><img src=\"%Site.ImageURL%email-templates/GP.gif\" border=\"0\" /></a></td></tr></table></td></tr><tr><td style=\"background: #cfcfcf; border-bottom: 1px solid #949494;font-family: Tahoma; font-size: 11px; color: #444; line-height: 20px;padding:10px;\">This is a system generated message, please do not respond to this email. <br /><br />The information transmitted is intended only for the person or entity to which it is addressed and may contain confidential and/or privileged material. Any review, retransmission, dissemination or other use of, or taking of any action in reliance upon, this information by persons or entities other than the intended recipient is prohibited. If you received this in error, please contact the sender and delete the material from any computer.</td></tr></table></body></html>",
                                            IsActive = true,
                                            EmailAccountId = eaGeneral.RowId
                                        },
                                   new MessageTemplate 
                                       {
                                           Name = "User.EmailValidationMessage",
                                           Subject = "%Site.Name% Email validation",
                                           Body = "<p><strong>Dear %User.FullName%,</strong></p><p><strong>Thank you for registering with SmartSignin</strong></p><p><a href=\"%User.AccountActivationURL%\" target=\"_blank\"><img src=\"$Site.ImageURL$email-templates/Activate.png\" border=\"0\" /></a></p><p style=\"font-family: Tahoma; font-size: 12px; font-style: italic;\">Note: If the link does not open, please copy and paste this link <a style=\"color:#fff;\" href=\"%User.AccountActivationURL%\" target=\"_blank\">%User.AccountActivationURL%</a> in your web browser and press enter to activate.</p>",
                                           IsActive = true, EmailAccountId = eaGeneral.RowId
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "User.PasswordRecovery",
                                           Subject = "%Store.Name%. Password recovery",
                                           Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To change your password <a href=\"%user.PasswordRecoveryURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.RowId,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "User.AdminNotification",
                                           Subject = "New user registration",
                                           Body = "<p><a href=\"%Site.URL%\">%Site.Name%</a> <br /><br />A new user registered with your application. Below are the user's details:<br />Full name: %User.FullName%<br />Username: %User.Username%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.RowId,
                                       }
                               };
            messageTemplates.ForEach(mt => messageTemplateRepository.Insert(mt));

        }

        public virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>()
                                      {
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = SystemActivityLogTypeNames.Add,
                                                  Name = "Added Record"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = SystemActivityLogTypeNames.Update,
                                                  Name = "Updated Record"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = SystemActivityLogTypeNames.View,
                                                  Name = "Viewed Record"
                                              },
                                        new ActivityLogType
                                              {
                                                  SystemKeyword = SystemActivityLogTypeNames.ChangePublishingStatus,
                                                  Name = "Publishing Status"
                                              },
                                        new ActivityLogType
                                              {
                                                  SystemKeyword = SystemActivityLogTypeNames.Delete,
                                                  Name = "Deleted Record"
                                              }
                                      };
            activityLogTypes.ForEach(alt => activityLogTypeRepository.Insert(alt));
        }

        public virtual void InstallPermissions()
        {
            var permissionProviders = new List<Type>();
            permissionProviders.Add(typeof(StandardPermissionProvider));

            foreach (var providerType in permissionProviders)
            {
                dynamic provider = Activator.CreateInstance(providerType);
                EngineContext.Current.Resolve<IPermissionService>().Install(provider);
            }
        }
                
        #endregion
    }
}