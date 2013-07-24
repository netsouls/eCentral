using System.Web;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Domain.Users;

namespace eCentral.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets whether the user is authenticated or not
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        User CurrentUser { get; }

        /// <summary>
        /// Gets or sets the original user (in case the current one is impersonated)
        /// </summary>
        User OriginalUserIfImpersonated { get; }

        /// <summary>
        /// Gets or sets whether the user is administrator or not
        /// </summary>
        bool IsAdministrator { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        Currency WorkingCurrency { get; set; }

        /// <summary>
        /// Get or set the current context 
        /// </summary>
        HttpContextBase Context { get; }

        /// <summary>
        /// Sign out a user
        /// </summary>
        void SignOut();
    }
}
