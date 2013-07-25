namespace eCentral.Services.Security.Cryptography
{
    public interface IAESService
    {
        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="plainText">Text that needs to be encrypted</param>
        /// <param name="encryptionKey">Encryption security key</param>
        string Encrypt(string plainText, string encryptionKey);

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="encryptedText">Text that needs to be decrypted</param>
        /// <param name="encryptionKey">Encryption security key</param>
        string Decrypt(string encryptedText, string encryptionKey);

        /// <summary>
        /// Get AES Initial Vector
        /// </summary>
        /// <returns></returns>
        byte[] GetInbyteAESIV();

        /// <summary>
        /// Get AES Initial Vector
        /// </summary>
        /// <returns></returns>
        string GetInStringAESIV();

        /// <summary>
        /// Get AES Key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"></param>
        /// <param name="AESVI"></param>
        /// <param name="iterationCount"></param>
        /// <returns></returns>
        string GetAESKey(string password, int keySize, string AESIV, int iterationCount);

        /// <summary>
        /// Get AES Key
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"></param>
        /// <param name="AESVI"></param>
        /// <param name="iterationCount"></param>
        /// <returns></returns>
        byte[] GetAESKey(string password, int keySize, byte[] AESIV, int iterationCount);

        /// <summary>
        /// AES Encryption
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aesKey"></param>
        /// <param name="aesVI"></param>
        /// <returns></returns>
        string Encrypt(string input, string aesKey, string aesIV);

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aesKey"></param>
        /// <param name="aesVI"></param>
        /// <returns></returns>
        string Decrypt(string input, string aesKey, string aesIV);
    }
}
