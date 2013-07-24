using eCentral.Core.Domain.Security;

namespace eCentral.Services.Security.Cryptography
{
    public interface IHashService
    {
        /// <summary>
        /// Get SHA Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        string GetHash(string plainText, HashType hashType, bool returnHex);

        /// <summary>
        /// Checks a text with a hash.</summary>
        /// </summary>
        /// <param name="plainText">The text to compare the hash against.</param>
        /// <param name="compareHash">The hash to compare against</param>
        /// <param name="hashType">Type of Hash </param>
        /// <returns></returns>
        bool CheckHash( string plainText, string compareHash, HashType hashType );

        /// <summary>
        /// Convert a Binary value to Hex
        /// </summary>
        /// <param name="binaryValue"></param>
        /// <returns></returns>
        string BinaryToHex(string binaryValue);

        /// <summary>
        /// Converts a Hex value to Binary
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        string HexToBinary(string hexValue);
    }
}
