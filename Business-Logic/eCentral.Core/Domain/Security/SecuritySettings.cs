using System.Text;
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

        /// <summary>
        /// Gets or sets a minimum password length
        /// </summary>
        public int PasswordMinLength { get; set; }

        #region To String

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // set values 
            builder.AppendFormat("HideMenuItemsBasedOnPermissions: [{0}]", this.HideMenuItemsBasedOnPermissions.ToString());

            return builder.ToString();
        }

        #endregion
    }
}