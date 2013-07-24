using System.Collections.Generic;

namespace eCentral.Services.Messages
{
    /// <summary>
    /// Represents the SMS service
    /// </summary>
    public partial interface ISmsService
    {
        /// <summary>
        /// Load all SMS providers
        /// </summary>
        /// <returns>SMS provider list</returns>
        IList<ISmsProvider> LoadAll();

         /// <summary>
        /// Load SMS provider by system name
        /// </summary>
        /// <param name="systemName">SMS provider system name</param>
        /// <returns>SMS provider</returns>
        ISmsProvider LoadBySystemName(string systemName);

        /// <summary>
        /// Load active SMS providers
        /// </summary>
        /// <returns>Active SMS provider list</returns>
        IList<ISmsProvider> LoadActive();

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        int SendSms(string text); 
    }
}
