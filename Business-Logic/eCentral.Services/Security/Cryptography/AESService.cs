using System;

using eCentral.Core;

namespace eCentral.Services.Security.Cryptography
{
    internal partial class AESService : IAESService
    {
        #region Fields

        private IBlockCipherAES cipherAES;
        private IRijndaelEnhancedService rijndaelEnhancedService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public AESService(IRijndaelEnhancedService rijndaelEnhancedService)
        {
            this.rijndaelEnhancedService = rijndaelEnhancedService;
            this.cipherAES = new CipherAES();
        }

        #endregion

        #region Methods

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="encryptedText">Text that needs to be decrypted</param>
        /// <param name="encryptionKey">Encryption security key</param>
        public string Decrypt(string encryptedText, string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            //#if DEBUG
            //return encryptedText;
            //#else
            string aesIV = GetInStringAESIV();
            string aesKey = GetAESKey(encryptionKey, 512, aesIV, 1024);

            return Decrypt(encryptedText, aesKey, aesIV);
            //#endif

        }

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="plainText">Text that needs to be encrypted</param>
        /// <param name="encryptionKey">Encryption security key</param>
        public string Encrypt(string plainText, string encryptionKey)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            //#if DEBUG
            //return plainText;
            //#else
            string aesIV = GetInStringAESIV();
            string aesKey = GetAESKey(encryptionKey, 512, aesIV, 1024);

            return Encrypt(plainText, aesKey, aesIV);
            //#endif
        }


        /// <summary>
        /// Get AES Initial Vector
        /// </summary>
        /// <returns></returns>
        public byte[] GetInbyteAESIV()
        {
            return new byte[this.cipherAES.BlockSizeInBytes()];
        }

        /// <summary>
        /// Get AES Initial Vector
        /// </summary>
        /// <returns></returns>
        public string GetInStringAESIV()
        {
            return
                (new byte[this.cipherAES.BlockSizeInBytes()]).ByteArrayToString().Replace("\\", "");
        }

        /// <summary>
        /// Get AES Key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"></param>
        /// <param name="AESVI"></param>
        /// <param name="iterationCount"></param>
        /// <returns></returns>
        public string GetAESKey(string password, int keySize, string AESIV, int iterationCount)
        {
            byte[] aesIV = AESIV.StringToByteArray();
            for (int i = 0; i < aesIV.Length; ++i) aesIV[i] = 0;
            var salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            return (DeriveKey(password, keySize, salt, iterationCount)).ByteArrayToString();
        }

        /// <summary>
        /// Get AES Key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"></param>
        /// <param name="AESVI"></param>
        /// <param name="iterationCount"></param>
        /// <returns></returns>
        public byte[] GetAESKey(string password, int keySize, byte[] AESIV, int iterationCount)
        {
            for (int i = 0; i < AESIV.Length; ++i) AESIV[i] = 0;
            var salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            return DeriveKey(password, keySize, salt, iterationCount);
        }

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aesKey"></param>
        /// <param name="aesVI"></param>
        /// <returns></returns>
        public string Encrypt(string input, string aesKey, string aesIV)
        {
            var rijndaelKey =
                new RijndaelRequest(aesKey, aesIV); //, 4,8, 512, "SHA512", null, 1024);

            return rijndaelEnhancedService.Encrypt(rijndaelKey, input);
        }

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aesKey"></param>
        /// <param name="aesVI"></param>
        /// <returns></returns>
        public string Decrypt(string input, string aesKey, string aesIV)
        {
            var rijndaelKey =
                new RijndaelRequest(aesKey, aesIV); //, 10, 500, 512, "SHA512", null, 1024);

            return rijndaelEnhancedService.Decrypt(rijndaelKey, input);
        }

        #endregion

        #region Private

        #region AES Key Creator

        /// <summary>
        /// safe method to generate a key
        /// see the next override for more info
        /// </summary>
        private byte[] DeriveKey(string password, int keySize, byte[] salt)
        {
            return DeriveKey(password, keySize, salt, 1024);
        }

        /// <summary>
        /// make a key out of a string password
        /// based on PBKDF1 (PKCS #5 v1.5)
        /// if you do not want a salt set it to null
        /// recomended salt length must be between 8 and 16 bytes
        /// This implementation support keySize up to 32 bytes
        /// use salt = null, iterationCount = 1 for minimal strength
        /// </summary>
        private byte[] DeriveKey(string password, int keySize, byte[] salt, int iterationCount)
        {
            if (keySize > 32) keySize = 32;
            byte[] data = ASCIIEncoder(password);
            if (salt != null)
            {
                var temp = new byte[data.Length + salt.Length];
                Array.Copy(data, 0, temp, 0, data.Length);
                Array.Copy(salt, 0, temp, data.Length, salt.Length);
                data = temp;
            }
            if (iterationCount <= 0) iterationCount = 1;
            for (int i = 0; i < iterationCount; i++)
            {
                data = SHA512AES.MessageSHA(data);
            }
            var key = new byte[keySize];
            Array.Copy(data, 0, key, 0, keySize);
            return key;
        }

        /// <summary>
        /// helper function not to rely on System.Text.ASCIIEncoder
        /// </summary>
        private byte[] ASCIIEncoder(string s)
        {
            var ascii = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                ascii[i] = (byte)s[i];
            }
            return ascii;
        }

        #endregion

        #endregion
    }
}
