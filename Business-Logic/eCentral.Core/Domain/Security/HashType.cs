
namespace eCentral.Core.Domain.Security
{
    public enum HashType
    {
        /// <summary>
        /// SHA1 Hashing
        /// </summary>
        SHA1,

        /// <summary>
        /// SHA256 Hashing
        /// </summary>
        SHA256,

        /// <summary>
        /// SHA384 Hashing
        /// </summary>
        SHA384,

        /// <summary>
        /// SHA512 Hashing
        /// </summary>
        SHA512
    }

    public enum CipherAESMode
    {
        Encrypt,
        Decrypt,
        Both
    };

}
