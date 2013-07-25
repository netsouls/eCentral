using eCentral.Core;
using eCentral.Core.Domain.Security;
using eCentral.Core.Infrastructure;

namespace eCentral.Services.Security.Cryptography
{
    public class EncryptionService : IEncryptionService
    {
        #region Fields

        private readonly SecuritySettings securitySettings;
        private readonly IHashService hashService;
        private readonly IAESService aesService;
        
        #endregion

        #region Ctor
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="securitySettings">Security Settings</param>
        /// <param name="criptoService">Cripto Service</param>
        public EncryptionService(SecuritySettings securitySettings)
        {
            this.securitySettings = securitySettings;
            this.hashService      = EngineContext.Current.Resolve<IHashService>();
            this.aesService = EngineContext.Current.Resolve<IAESService>();
        }

        #endregion

        #region Methods

        #region Hash 

        /// <summary>
        /// Get SHA Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public virtual string GetHash(string plainText, HashType hashType, bool returnHex)
        {
            return hashService.GetHash(plainText, hashType, returnHex);
        }

        /// <summary>
        /// Convert a Binary value to Hex
        /// </summary>
        /// <param name="binaryValue"></param>
        /// <returns></returns>
        public virtual string BinaryToHex(string binaryValue)
        {
            return hashService.BinaryToHex(binaryValue);
        }

        /// <summary>
        /// Converts a Hex value to Binary
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        public virtual string HexToBinary(string hexValue)
        {
            return hashService.HexToBinary(hexValue);
        }

        /// <summary>
        /// Get SHA Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public virtual string GetSHAHash(string plainText, bool returnHex)
        {
            return hashService.GetHash(plainText, HashType.SHA512, returnHex);
        }

        /// <summary>
        /// Checks a text with a hash.</summary>
        /// </summary>
        /// <param name="plainText">The text to compare the hash against.</param>
        /// <param name="compareHash">The hash to compare against</param>
        /// <param name="hashType">Type of Hash </param>
        /// <returns></returns>
        public virtual bool CheckHash(string plainText, string compareHash)
        {
            return hashService.CheckHash(plainText, compareHash, HashType.SHA512);
        }

        /// <summary>
        /// Checks a text with a hash.</summary>
        /// </summary>
        /// <param name="plainText">The text to compare the hash against.</param>
        /// <param name="compareHash">The hash to compare against</param>
        /// <param name="hashType">Type of Hash </param>
        /// <returns></returns>
        public virtual bool CheckHash(string plainText, string compareHash, HashType hashType)
        {
            return hashService.CheckHash(plainText, compareHash, hashType);
        }

        #endregion

        #region AES

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="plainText">Text that needs to be encrypted</param>
        /// <param name="currentUser">Current logged in user</param>
        public string AESEncrypt<T>(string plainText, T entity)
            where T : BaseEntity
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            string encryptKey = AESEncryptionKey<T>(entity);

            return AESEncrypt(plainText, encryptKey);
        }

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="encryptedText">Text that needs to be decrypted</param>
        /// <param name="currentUser">Current logged in user</param>
        /// <returns></returns>
        public string AESDecrypt<T>(string encryptedText, T entity)
            where T: BaseEntity
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            string encryptKey = AESEncryptionKey<T>(entity);

            return AESDecrypt(encryptedText, encryptKey);
        }

        /// <summary>
        /// AES Encryption with server private key
        /// </summary>
        /// <param name="plainText">Text that needs to be encrypted</param>
        public string AESEncrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            return AESEncrypt(plainText, securitySettings.EncryptionKey + StateKeyManager.EncryptionKey);
        }

        /// <summary>
        /// AES Decryption with server private key
        /// </summary>
        /// <param name="encryptedText">Text that needs to be decrypted</param>
        /// <returns></returns>
        public string AESDecrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            return AESDecrypt(encryptedText, securitySettings.EncryptionKey + StateKeyManager.EncryptionKey);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="plainText">Text that needs to be decrypted</param>
        /// <param name="encryptionKey">Encryption security key</param>
        private string AESDecrypt(string encryptedText, string encryptionKey)
        {
            return aesService.Decrypt(encryptedText, encryptionKey);
        }

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="plainText">Text that needs to be encrypted</param>
        /// <param name="encryptionKey">Encryption security key</param>
        private string AESEncrypt(string plainText, string encryptionKey)
        {
            return aesService.Encrypt(plainText, encryptionKey);
        }

        private string AESEncryptionKey<T> ( T entity) 
            where T : BaseEntity
        {
            return "{0}{1}{2}{3}{4}{5}{6}{7}".FormatWith
            (
                entity.RowId.ToString().Substring(9, 2),
                entity.RowId.ToString().Substring(19, 2),
                entity.RowId.ToString().Substring(4, 2),
                entity.RowId.ToString().Substring(32, 2),
                entity.RowId.ToString().Substring(34, 2),
                entity.RowId.ToString().Substring(21, 2),
                entity.RowId.ToString().Substring(0, 2),
                entity.RowId.ToString().Substring(16, 2)
            );
        }

        #endregion
    }
}
