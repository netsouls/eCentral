namespace eCentral.Services.Security.Cryptography
{
    public interface IAESService
    {
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
        string AESEncrypt(string input, string aesKey, string aesIV);

        /// <summary>
        /// AES Decryption
        /// </summary>
        /// <param name="input"></param>
        /// <param name="aesKey"></param>
        /// <param name="aesVI"></param>
        /// <returns></returns>
        string AESDecrypt(string input, string aesKey, string aesIV);
    }
}
