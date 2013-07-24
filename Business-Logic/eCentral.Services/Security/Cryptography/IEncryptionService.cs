using eCentral.Core;

namespace eCentral.Services.Security.Cryptography 
{
    public interface IEncryptionService : IAESService, IHashService
    {
        #region Hash Encryption 

        /// <summary>
        /// Get SHA Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string GetSHAHash(string plainText, bool returnHex);

        /// <summary>
        /// Checks a text with a hash.</summary>
        /// </summary>
        /// <param name="plainText">The text to compare the hash against.</param>
        /// <param name="compareHash">The hash to compare against</param>
        /// <param name="hashType">Type of Hash </param>
        /// <returns></returns>
        bool CheckHash(string plainText, string compareHash);
        
        #endregion

        #region AES Encryption

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="plainText">Text that needs to be encrypted</param>
        /// <param name="currentUser">Current logged in user</param>
        string AESEncrypt<T>(string plainText, T entity)
            where T : BaseEntity;

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="encryptedText">Text that needs to be decrypted</param>
        /// <param name="currentUser">Current logged in user</param>
        /// <returns></returns>
        string AESDecrypt<T>(string encryptedText, T entity)
            where T : BaseEntity;

        #endregion
    }
}
