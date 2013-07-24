using eCentral.Core.Domain.Users;

namespace eCentral.Services.Authentication
{
    /// <summary>
    /// Authentication service interface
    /// </summary>
    public partial interface IAuthenticationService 
    {
        /// <summary>
        /// SignIn a valid user
        /// </summary>
        /// <param name="userName">Username</param>
        void SignIn(User user, bool createPersistentCookie);

        /// <summary>
        /// Sign out a user
        /// </summary>
        void SignOut();

        /// <summary>
        /// Get an authenticated user details
        /// </summary>
        /// <returns></returns>
        User GetAuthenticatedUser();
    }
}