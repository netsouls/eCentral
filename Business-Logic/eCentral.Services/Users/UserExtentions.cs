using System;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Domain.Users;
using eCentral.Core.Infrastructure;
using eCentral.Services.Common;
using eCentral.Services.Security.Cryptography;

namespace eCentral.Services.Users
{
    public static class UserExtentions
    {
        /// <summary>
        /// Gets a value indicating whether user is in a certain customer role
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="customerRoleSystemName">Customer role system name</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>Result</returns>
        public static bool IsInUserRole(this User user,
            string userRoleSystemName, bool onlyActiveUserRoles = true)
        {
            Guard.IsNotNull(user, "User");

            if (String.IsNullOrEmpty(userRoleSystemName))
                throw new ArgumentNullException("userRoleSystemName");

            var result = user.UserRoles
                .Where(cr => !onlyActiveUserRoles || cr.Active)
                .Where(cr => cr.SystemName == userRoleSystemName)
                .FirstOrDefault() != null;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether user is an enterprise administrator
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>Result</returns>
        public static bool IsAdministrator(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Administrators, onlyActiveUserRoles);
        }

        /// <summary>
        /// Gets a value indicating whether customer is registered
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>Result</returns>
        public static bool IsRegistered(this User user, bool onlyActiveUserRoles = true)
        {
            return IsInUserRole(user, SystemUserRoleNames.Users, onlyActiveUserRoles);
        }

        public static string GetFullName(this User user)
        {
            Guard.IsNotNull(user, "User");

            // decrypt the first and last name of the user
            var encryptionService = EngineContext.Current.Resolve<IEncryptionService>();

            var firstName = encryptionService.AESDecrypt( user.GetAttribute<string>(SystemUserAttributeNames.FirstName), user);
            var lastName  = encryptionService.AESDecrypt( user.GetAttribute<string>(SystemUserAttributeNames.LastName), user);
            
            string fullName = "";
            if (!String.IsNullOrWhiteSpace(firstName) && !String.IsNullOrWhiteSpace(lastName))
                fullName = string.Format("{0} {1}", firstName, lastName);
            else
            {
                if (!String.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!String.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }
            return fullName;
        }

        /// <summary>
        /// Formats the customer name
        /// </summary>
        /// <param name="customer">Source</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this User user)
        {
            return FormatUserName(user, false);
        }

        /// <summary>
        /// Formats the customer name
        /// </summary>
        /// <param name="customer">Source</param>
        /// <param name="stripTooLong">Strip too long customer name</param>
        /// <returns>Formatted text</returns>
        public static string FormatUserName(this User user, bool stripTooLong)
        {
            if (user == null)
                return string.Empty;

            string result = string.Empty;
            switch (EngineContext.Current.Resolve<UserSettings>().UserNameFormat)
            {
                case UserNameFormat.ShowFullNames:
                    result = user.GetFullName();
                    break;
                case UserNameFormat.ShowUsernames:
                    result = user.Username;
                    break;
                default:
                    break;
            }

            if (stripTooLong)
            {
                int maxLength = 0; // EngineContext.Current.Resolve<CustomerSettings>().FormatNameMaxLength;
                if (maxLength > 0 && result.Length > maxLength)
                {
                    result = result.Substring(0, maxLength);
                }
            }

            return result;
        }
    }
}
