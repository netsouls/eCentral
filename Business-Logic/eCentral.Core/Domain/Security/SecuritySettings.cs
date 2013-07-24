using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Security
{
    public class SecuritySettings : ISettings
    {
        /// <summary>
        /// Gets or sets an encryption key
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets a vaule indicating whether to hide web admin menu items based on ACL
        /// </summary>
        public bool HideMenuItemsBasedOnPermissions { get; set; }
    }
}