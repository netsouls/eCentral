using System.Collections.Generic;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Domain;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Domain.Users;
using eCentral.Services.Directory;
using eCentral.Services.Helpers;
using eCentral.Services.Localization;
using eCentral.Services.Users;
using eCentral.Services.Common;

namespace eCentral.Services.Messages
{
    public partial class MessageTokenProvider : IMessageTokenProvider
    {
        #region Fields

        private readonly ILanguageService languageService;
        private readonly ILocalizationService localizationService;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly IEmailAccountService emailAccountService;
        private readonly ICurrencyService currencyService;
        private readonly IWebHelper webHelper;
        private readonly IWorkContext workContext;
        private readonly SiteInformationSettings siteSettings;
        private readonly MessageTemplatesSettings templatesSettings;
        private readonly EmailAccountSettings emailAccountSettings;

        #endregion

        #region Ctor

        public MessageTokenProvider(ILanguageService languageService,
            ILocalizationService localizationService, IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ICurrencyService currencyService,IWebHelper webHelper,
            IWorkContext workContext,
            SiteInformationSettings siteSettings, MessageTemplatesSettings templatesSettings,
            EmailAccountSettings emailAccountSettings)
        {
            this.languageService      = languageService;
            this.localizationService  = localizationService;
            this.dateTimeHelper       = dateTimeHelper;
            this.emailAccountService  = emailAccountService;
            this.currencyService      = currencyService;
            this.webHelper            = webHelper;
            this.workContext          = workContext;
            this.siteSettings         = siteSettings;
            this.templatesSettings    = templatesSettings;
            this.emailAccountSettings = emailAccountSettings;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        public virtual void AddDefaultTokens(IList<Token> tokens)
        {
            tokens.Add(new Token("Site.Name", siteSettings.SiteName));
            tokens.Add(new Token("Site.URL", siteSettings.SiteUrl, true));
            tokens.Add(new Token("Site.ImageURL", siteSettings.SiteUrl + "library/images/", true));
            var defaultEmailAccount = emailAccountService.GetById(emailAccountSettings.DefaultEmailAccountId);
            if (defaultEmailAccount == null)
                defaultEmailAccount = emailAccountService.GetAll().FirstOrDefault();
            tokens.Add(new Token("Site.Email", defaultEmailAccount.Email));
        }

        public virtual void AddUserTokens(IList<Token> tokens, User user)
        {
            tokens.Add(new Token("User.Username", user.Username));
            tokens.Add(new Token("User.FullName", user.FormatUserName()));
            
            //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
            string passwordRecoveryUrl = string.Format("{0}passwordrecovery/confirm?token={1}&email={2}", webHelper.AbsoluteWebRoot.ToString(), 
                user.GetAttribute<string>(SystemUserAttributeNames.PasswordRecoveryToken), user.Username);
            
            /*string accountActivationUrl = string.Format("{0}user/activation?token={1}-{2}", webHelper.AbsoluteWebRoot.ToString(), user.GetAttribute<string>(SystemUserAttributeNames.AccountActivationToken), user.UserGuid.ToString());*/
            tokens.Add(new Token("User.PasswordRecoveryURL", passwordRecoveryUrl, true));
            
        }

        public virtual void AddMasterTokens(IList<Token> tokens, string messageBody)
        {
            tokens.Add(new Token("Message.Body", messageBody, true));
        }

        public virtual string[] GetListOfAllowedTokens()
        {
            var allowedTokens = new List<string>()
            {
                "%Message.Body%", // main message template
                "%Site.Name%",
                "%Site.URL%",
                "%Site.Email%",
                "%User.Email%", 
                "%User.Username%", 
                "%User.FullName%", 
                "%User.PasswordRecoveryURL%", 
                "%User.AccountActivationURL%"
            };
            return allowedTokens.ToArray();
        }
        
        #endregion
    }
}
