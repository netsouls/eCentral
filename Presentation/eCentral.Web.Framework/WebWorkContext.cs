using System;
using System.Linq;
using System.Web;
using eCentral.Core;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Domain.Users;
using eCentral.Core.Session;
using eCentral.Services.Authentication;
using eCentral.Services.Common;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Services.Users;
using eCentral.Web.Framework.Localization;

namespace eCentral.Web.Framework
{
    /// <summary>
    /// Working context for web application
    /// </summary>
    public partial class WebWorkContext : IWorkContext
    {
        private readonly HttpContextBase httpContext;
        private readonly IUserService userservice;
        private readonly IAuthenticationService authenticationService;
        private readonly ILanguageService languageService;
        private readonly ISessionManager sessionManager;
        private readonly ICurrencyService currencyService;
        private readonly CurrencySettings currencySettings;
        private readonly LocalizationSettings localizationSettings;
        private readonly IWebHelper webHelper;

        private User _cachedUser;
        private User _originalUserIfImpersonated;
        
        public WebWorkContext(HttpContextBase httpContext,
            IUserService userservice,
            IAuthenticationService authenticationService,
            ILanguageService languageService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            ISessionManager sessionManager,
            LocalizationSettings localizationSettings,
            IWebHelper webHelper)
        {
            this.httpContext = httpContext;
            this.userservice = userservice;
            this.authenticationService = authenticationService;
            this.languageService = languageService;
            this.currencyService = currencyService;
            this.currencySettings = currencySettings;
            this.sessionManager = sessionManager;
            this.localizationSettings = localizationSettings;
            this.webHelper = webHelper;
        }

        protected User GetCurrentUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            User user = null;
            if (httpContext != null)
            {
                //registered user
                if (user == null)
                {
                    user = authenticationService.GetAuthenticatedUser();
                }

                //impersonate user if required (currently used for 'phone order' support)
                if (user != null && user.CurrentPublishingStatus == PublishingStatus.Active)
                {
                    Guid? impersonatedUserId = user.GetAttribute<Guid?>(SystemUserAttributeNames.ImpersonatedUserId);
                    if (impersonatedUserId.HasValue && !impersonatedUserId.Value.IsEmpty())
                    {
                        var impersonatedUser = userservice.GetById(impersonatedUserId.Value);
                        if (impersonatedUser != null && impersonatedUser.CurrentPublishingStatus == PublishingStatus.Active)
                        {
                            //set impersonated user
                            _originalUserIfImpersonated = user;
                            user = impersonatedUser;
                        }
                    }
                }
            }

            //validation
            if (user != null && user.CurrentPublishingStatus == PublishingStatus.Active)
            {
                //update last activity date
                bool updateLastActivity = false;
                if (user.LastActivityDate.HasValue)
                {
                    if (user.LastActivityDate.Value.AddMinutes(1.0) < DateTime.UtcNow)
                    {
                        updateLastActivity = true;
                    }
                }
                else
                    updateLastActivity = true;

                if (updateLastActivity)
                {
                    user.LastActivityDate = DateTime.UtcNow;
                    userservice.Update(user);
                }

                _cachedUser = user;
            }

            return _cachedUser;
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return GetCurrentUser();
            }
        }

        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        public User OriginalUserIfImpersonated
        {
            get
            {
                return _originalUserIfImpersonated;
            }
        }

        /// <summary>
        /// Gets whether the user is authenticated or not
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                if (this.CurrentUser == null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets whether the user is an administrator or not
        /// </summary>
        public bool IsAdministrator
        {
            get
            {
                return _cachedUser.IsAdministrator();
            }
        }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        public Language WorkingLanguage
        {
            get
            {
                //get language from URL (if possible)
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    if (httpContext != null)
                    {
                        string virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath;
                        string applicationPath = httpContext.Request.ApplicationPath;
                        if (virtualPath.IsLocalizedUrl(applicationPath, false))
                        {
                            var seoCode = virtualPath.GetLanguageSeoCodeFromUrl(applicationPath, false);
                            if (!String.IsNullOrEmpty(seoCode))
                            {
                                var langByCulture = languageService.GetAll()
                                    .Where(l => seoCode.Equals(l.UniqueSeoCode, StringComparison.InvariantCultureIgnoreCase))
                                    .FirstOrDefault();
                                if (langByCulture != null && langByCulture.Published)
                                {
                                    //the language is found. now we need to save it
                                    if (this.CurrentUser != null &&
                                        !langByCulture.Equals(this.CurrentUser.Language))
                                    {
                                        this.CurrentUser.Language = langByCulture;
                                        userservice.Update(this.CurrentUser);
                                    }
                                }
                            }
                        }
                    }
                }

                if (this.CurrentUser != null &&
                    this.CurrentUser.Language != null &&
                    this.CurrentUser.Language.Published)
                    return this.CurrentUser.Language;

                var lang = languageService.GetAll().FirstOrDefault();
                return lang;
            }
            set
            {
                if (this.CurrentUser == null)
                    return;

                this.CurrentUser.Language = value;
                userservice.Update(this.CurrentUser);
            }
        }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        public Currency WorkingCurrency
        {
            get
            {
                if (this.CurrentUser != null &&
                    this.CurrentUser.Currency != null &&
                    this.CurrentUser.Currency.Published)
                    return this.CurrentUser.Currency;

                var currency = currencyService.GetById(currencySettings.PrimaryCurrencyId);
                return currency;
            }
            set
            {
                if (this.CurrentUser == null)
                    return;

                this.CurrentUser.Currency = value;
                userservice.Update(this.CurrentUser);
            }
        }

        public HttpContextBase Context
        {
            get
            {
                return httpContext;
            }
        }

        /// <summary>
        /// Sign out a user
        /// </summary>
        public void SignOut()
        {
            // call the authentication service to logout the user
            authenticationService.SignOut();
            _cachedUser = null;
        }
    }
}
