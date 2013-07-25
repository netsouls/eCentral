using System;
using eCentral.Core.Domain.Users;

namespace eCentral.Services.Messages
{
    public partial interface IWorkflowMessageService
    {
        #region User workflow

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">user instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Guid SendUserEmailValidationMessage(User user, Guid languageId);

        #endregion

        #region Misc

        #endregion
    }
}
