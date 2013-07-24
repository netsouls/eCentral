using System;
using System.Collections.Generic;
using eCentral.Core;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Users;
using eCentral.Services.Common;
using eCentral.Services.Localization;
using eCentral.Services.Logging;
using eCentral.Services.Security.Cryptography;

namespace eCentral.Services.Users
{
    /// <summary>
    /// User registration service
    /// </summary>
    public partial class UserRegistrationService : IUserRegistrationService
    {
        #region Fields

        private readonly IUserService userService;
        private readonly IGenericAttributeService attributeService;
        private readonly IUserActivityService userActivityService;
        private readonly ILocalizationService localizationService;
        private readonly IEncryptionService encryptionService;
        private readonly IWebHelper webHelper;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="UserService">User service</param>
        /// <param name="localizationService">Localization service</param>
        /// <param name="rewardPointsSettings">Reward points settings</param>
        public UserRegistrationService(IUserService userService, IUserActivityService userActivityService,
            IEncryptionService encryptionService, IGenericAttributeService attributeService,
            ILocalizationService localizationService, IWebHelper webHelper)
        {
            this.userService         = userService;
            this.attributeService    = attributeService;
            this.userActivityService = userActivityService;
            this.encryptionService   = encryptionService;
            this.webHelper           = webHelper;
            this.localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual DataResult ChangePassword(ChangePasswordRequest request)
        {
            Guard.IsNotNull(request, "Request");

            var result = new DataResult();
            if (String.IsNullOrWhiteSpace(request.UserName))
            {
                result.AddError(this.localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(this.localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var User = this.userService.GetByUsername(request.UserName);
            if (User == null)
            {
                result.AddError(this.localizationService.GetResource("Account.ChangePassword.Errors.EmailNotFound"));
                return result;
            }

            var requestIsValid = false;
            if (request.ValidateRequest)
            {
                //password
                string oldPwd = request.OldPassword;

                bool oldPasswordIsValid = oldPwd.IsCaseInsensitiveEqual(User.Password);

                if (!oldPasswordIsValid)
                    result.AddError(localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));

                if (oldPasswordIsValid)
                    requestIsValid = true;
            }
            else
                requestIsValid = true;


            //at this point request is valid
            if (requestIsValid)
            {
                User.Password = request.NewPassword;
                User.LastPasswordChangeDate = DateTime.UtcNow; // set the date time when the password has been changed. 

                this.userService.Update(User);
            }

            return result;
        }

        /// <summary>
        /// Recover the user lost password
        /// </summary>
        public virtual DataResult<IList<string>> RecoverPassword(ResetPasswordRequest request)
        {
            // we need to validate the user on each request and then validate the 
            // user step messages and return the results
            Guard.IsNotNull(request, "request");

            var result = new DataResult<IList<string>>();
            
            var user = this.userService.GetByUsername(request.UserName);
            
            if (user == null) // user does not exists
            {
                result.AddError(this.localizationService.GetResource("Account.PasswordRecovery.UserNotFound"));
                return result;
            }

            if (((PublishingStatus)user.CurrentPublishingStatus) != PublishingStatus.Active) // user has not activated their account
            {
                result.AddError(this.localizationService.GetResource("Account.PasswordRecovery.UserNotActive"));
                return result;
            }

            // now take action depending upon the reset password step
            switch (request.CurrentStep)
            {
                case PasswordResetSteps.ValidateEmailAddress:
                    
                case PasswordResetSteps.ValidateSecurityQuestions:
                case PasswordResetSteps.ResetPassword: // re-check for question answer validation
                    if (request.CurrentStep == PasswordResetSteps.ResetPassword)
                    {
                        var changePasswordRequest = new ChangePasswordRequest(request.UserName, false, request.NewPassword);
                        var changeResult = ChangePassword(changePasswordRequest);

                        // reset the variables to our result
                        result.Errors = changeResult.Errors;
                        return result;
                    }
                    break;
            }

            return result;
        }

        /// <summary>
        /// Validate user
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <param name="hashPassword">Hash Password</param>
        /// <returns>Result</returns>
        public virtual DataResult<User> ValidateUser(string userName, string hashPassword)
        {
            var result = new DataResult<User>();

            User user = this.userService.GetByUsername(userName);

            // need to set appropriate messages
            if (user == null) 
            {
                result.AddError(localizationService.GetResource("Security.Login.UserDoesNotExists"));
                return result;
            }

            // check whether the user isapproved or not 
            if (user.CurrentPublishingStatus != PublishingStatus.Active)
            {
                result.AddError(localizationService.GetResource("Security.Login.UserNotActive"));
                return result;
            }

            // only registered can login
            if (!user.IsRegistered())
                return result;

            // validate the password
            var isValid = encryptionService.GetSHAHash( hashPassword, true).IsCaseInsensitiveEqual(user.Password); //client is sending hashPassword

            if (!isValid)
            {
                user.FailedPasswordAttemptCount++;
                this.userService.Update(user);
                result.AddError(localizationService.GetResource("Security.Login.WrongCredentials"));
                return result;
            }

            // valid credentials and user
            result.Data = user;

            return result;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual DataResult<User> RegisterUser(UserRegistrationRequest request)
        {
            Guard.IsNotNull(request, "Request");

            var result = new DataResult<User>();

            if (!CommonHelper.IsValidEmail(request.Username))
            {
                result.AddError(this.localizationService.GetResource("Common.WrongEmail"));
                return result;
            }

            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError(this.localizationService.GetResource("Users.Fields.Password"));
                return result;
            }

            //validate unique user
            if (!this.userService.IsUnique(request.Username))
            {
                result.AddError(this.localizationService.GetResource("Users.Fields.Username.NotUnique"));
                return result;
            }

            var user = new User()
            {
                CreatedOn = DateTime.UtcNow,
                Username = request.Username,
                Password = request.Password, // we have already encrypted the password on client side
                CurrentPublishingStatus = request.CurrentPublishingStatus, // whether its an active user
            };

            //add to 'Registered' role
            var registeredRole = this.userService.GetUserRoleBySystemName(SystemUserRoleNames.Users);
            if ( registeredRole == null )
                throw new SiteException("'" + SystemUserRoleNames.Users + "' role could not be loaded");
            user.UserRoles.Add(registeredRole);

            // if administrator
            if (request.IsAdministrator)
            {
                // set the enterprise administrator role
                var administratorRole = this.userService.GetUserRoleBySystemName(SystemUserRoleNames.Administrators);
                if (administratorRole == null)
                    throw new SiteException("'" + SystemUserRoleNames.Administrators + "' role could not be loaded");
                user.UserRoles.Add(administratorRole);
            }

            // add audit history
            user.AuditHistory.Add
             (
                userActivityService.InsertActivity(SystemActivityLogTypeNames.Add,
                    user.ToString(), string.Empty)
             );

            // insert the user
            this.userService.Insert(user);

            // upadate attributes
            attributeService.SaveAttribute(user, SystemUserAttributeNames.FirstName, encryptionService.AESEncrypt(request.FirstName, user));
            attributeService.SaveAttribute(user, SystemUserAttributeNames.LastName, encryptionService.AESEncrypt(request.LastName, user));
            
            if ( !string.IsNullOrEmpty(request.Mobile))
                attributeService.SaveAttribute(user, SystemUserAttributeNames.Mobile, encryptionService.AESEncrypt( request.Mobile,user));

            // set the data result 
            result.Data = user;
            return result;
        }

        #endregion
    }
}