using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using eCentral.Core.Domain.Security;

namespace eCentral.Services.Security.Cryptography
{
    internal partial class HashService : IHashService
    {
        #region Fields

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public HashService()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get SHA Hash
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public virtual string GetHash(string plainText, HashType hashType, bool returnHex)
        {
            string hashText = string.Empty;

            switch (hashType)
            {
                case HashType.SHA1:
                    hashText = GetSHA1(plainText, returnHex);
                    break;

                case HashType.SHA256:
                    hashText = GetSHA256(plainText, returnHex);
                    break;

                case HashType.SHA384:
                    hashText = GetSHA384(plainText, returnHex);
                    break;

                case HashType.SHA512:
                    hashText = GetSHA512(plainText, returnHex);
                    break;
            }

            return hashText;
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
            string orignalHash = GetHash(plainText, hashType, true);

            return (orignalHash == compareHash);
        }

        /// <summary>
        /// Convert a Binary value to Hex
        /// </summary>
        /// <param name="binaryValue"></param>
        /// <returns></returns>
        public virtual string BinaryToHex(string binaryValue)
        {
            string hexValue = "";
            short iCount;
            for (iCount = 0; iCount < binaryValue.Length / 32; iCount++)
            {
                string tempstr =
                    Convert.ToString(Convert.ToUInt32(binaryValue.Substring(binaryValue.Length - 32 - 32 * iCount, 32), 2), 16);
                while (tempstr.Length < 8)
                {
                    tempstr = "0" + tempstr;
                }
                hexValue = tempstr + hexValue;
            }
            if (32 * iCount < binaryValue.Length)
            {
                hexValue =
                    Convert.ToString(Convert.ToUInt32(binaryValue.Substring(0, binaryValue.Length - 32 * iCount), 2), 16) +
                    hexValue;
            }
            while (hexValue.Length * 4 < binaryValue.Length)
            {
                hexValue = "0" + hexValue;
            }
            return hexValue;
        }

        /// <summary>
        /// Converts a Hex value to Binary
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        public virtual string HexToBinary(string hexValue)
        {
            string binaryValue = string.Empty;
            short iCount;

            for (iCount = 0; iCount < hexValue.Length / 8; iCount++)
            {
                string tempstr = Convert.ToString(
                    Convert.ToUInt32(hexValue.Substring(hexValue.Length - 8 - 8 * iCount, 8), 16), 2);
                while (tempstr.Length < 32)
                {
                    tempstr = "0" + tempstr;
                }
                binaryValue = tempstr + binaryValue;
            }

            if (8 * iCount < hexValue.Length)
            {
                binaryValue = Convert.ToString(Convert.ToUInt32(hexValue.Substring(0, hexValue.Length - 8 * iCount), 16), 2) +
                            binaryValue;
            }

            while (binaryValue.Length < hexValue.Length * 4)
            {
                binaryValue = "0" + binaryValue;
            }

            return binaryValue;
        }

        #endregion

        #region Hash Utilities

        private string GetSHA1(string plainText, bool returnHex)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] hashValue, messageBytes = asciiEncoding.GetBytes(plainText);

            var shHash = new SHA1Managed();
            string hexValue = string.Empty;

            hashValue = shHash.ComputeHash(messageBytes);
            hexValue = GetHashFromBytesArray(hashValue);

            if (returnHex)
                return BinaryToHex(hexValue);

            return hexValue;
        }

        private string GetSHA256(string plainText, bool returnHex)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] hashValue, messageBytes = asciiEncoding.GetBytes(plainText);

            var shHash = new SHA256Managed();
            string hexValue = string.Empty;

            hashValue = shHash.ComputeHash(messageBytes);
            hexValue = GetHashFromBytesArray(hashValue);

            if (returnHex)
                return BinaryToHex(hexValue);

            return hexValue;
        }

        private string GetSHA384(string plainText, bool returnHex)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] hashValue, messageBytes = asciiEncoding.GetBytes(plainText);

            var shHash = new SHA384Managed();
            string hexValue = string.Empty;

            hashValue = shHash.ComputeHash(messageBytes);
            hexValue = GetHashFromBytesArray(hashValue);

            if (returnHex)
                return BinaryToHex(hexValue);

            return hexValue;
        }

        private string GetSHA512(string plainText, bool returnHex)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] hashValue, messageBytes = asciiEncoding.GetBytes(plainText);

            var shHash = new SHA512Managed();
            string hexValue = string.Empty;

            hashValue = shHash.ComputeHash(messageBytes);
            hexValue = GetHashFromBytesArray(hashValue);

            if (returnHex)
                return BinaryToHex(hexValue);

            return hexValue;
        }

        private static string GetHashFromBytesArray(IEnumerable<byte> hashValue)
        {
            String hexValue = string.Empty;
            foreach (byte byteValue in hashValue)
            {
                string tempstr = Convert.ToString(byteValue, 2);
                while (tempstr.Length < 8)
                {
                    tempstr = "0" + tempstr;
                }

                hexValue += tempstr;
            }

            return hexValue;
        }

        #endregion
    }
}
