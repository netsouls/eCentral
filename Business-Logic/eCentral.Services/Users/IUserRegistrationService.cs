using System;
using eCentral.Core;
using eCentral.Core.Domain.Users;

namespace eCentral.Services.Users
{
    /// <summary>
    /// User registration interface
    /// </summary>
    public partial interface IUserRegistrationService
    {
        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <param name="hashPassword">Hash Password</param>
        /// <returns>UserValidationResult</returns>
        DataResult<User> ValidateUser(string userName, string hashPassword);

        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        DataResult<User> RegisterUser(UserRegistrationRequest request);

        /// <summary>
        /// Update registration
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        DataResult<User> UpdateRegistration(UserRegistrationRequest request);

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        DataResult ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Change the status
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <param name="publishingStatus">publishing status</param>
        /// <returns></returns>
        bool ChangeStatus(Guid userId, PublishingStatus publishingStatus);
    }
}