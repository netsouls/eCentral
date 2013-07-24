using System;
using System.Web;
using System.Web.Security;
using eCentral.Core;
using eCentral.Core.Domain.Users;
using eCentral.Core.Infrastructure;
using eCentral.Core.Session;
using eCentral.Services.Logging;
using eCentral.Services.Users;

namespace eCentral.Services.Authentication
{
    /// <summary>
    /// Authentication service
    /// </summary>
    public partial class FormsAuthenticationService : IAuthenticationService
    {
        private readonly HttpContextBase httpContext;
        private readonly IUserService userService;
        private readonly UserSettings userSettings;
        private readonly IWebHelper webHelper;
        private readonly ISessionManager sessionManager;
        private readonly TimeSpan expirationTimeSpan;

        private User _cachedUser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="userService">User service</param>
        /// <param name="userSettings">User settings</param>
        public FormsAuthenticationService(HttpContextBase httpContext,
            IUserService userService, 
            ISessionManager sessionManager, 
            UserSettings userSettings, IWebHelper webHelper)
        {
            this.httpContext        = httpContext;
            this.userService        = userService;
            this.sessionManager     = sessionManager;
            this.userSettings       = userSettings;
            this.webHelper          = webHelper;
            this.expirationTimeSpan = FormsAuthentication.Timeout;
        }

        /// <summary>
        /// Validate a user login
        /// </summary>
        /// <param name="user"></param>
        public virtual void SignIn(User user, bool createPersistentCookie)
        {
            var now = DateTime.UtcNow.ToLocalTime();

            var ticket = new FormsAuthenticationTicket(
                1 /*version*/,
                user.Username,
                now,
                now.Add(expirationTimeSpan),
                createPersistentCookie,
                user.Username,
                FormsAuthentication.FormsCookiePath);

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (ticket.IsPersistent)
            {
                cookie.Expires = ticket.Expiration;
            }
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (FormsAuthentication.CookieDomain != null)
            {
                cookie.Domain = FormsAuthentication.CookieDomain;
            }

            httpContext.Response.Cookies.Add(cookie);
            _cachedUser = user;

            // user valid login credentials
            user.FailedPasswordAttemptCount    = 0;
            userService.Update(user); // reset invalid credentials count

            EngineContext.Current.Resolve<IUserActivityService>().LoginHistory();
        }

        /// <summary>
        /// Sign out a user
        /// </summary>
        public virtual void SignOut()
        {
            _cachedUser = null;
            // sign out of form authentication also
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Get an authenticated user details
        /// </summary>
        /// <returns></returns>
        public virtual User GetAuthenticatedUser()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (httpContext == null ||
                httpContext.Request == null ||
                !httpContext.Request.IsAuthenticated || 
                !(httpContext.User.Identity is FormsIdentity))
            {
                return null;
            }

            var formsIdentity = (FormsIdentity)httpContext.User.Identity;
            var user = GetAuthenticatedUserFromTicket(formsIdentity.Ticket);
            if (user != null && user.CurrentPublishingStatus == PublishingStatus.Active && user.IsRegistered())
                _cachedUser = user;
            
            return _cachedUser;
        }

        public virtual User GetAuthenticatedUserFromTicket ( FormsAuthenticationTicket ticket )
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");

            var username = ticket.UserData;

            if (String.IsNullOrWhiteSpace(username))
                return null;
            var user = userService.GetByUsername(username);
            return user;
        }
    }
}