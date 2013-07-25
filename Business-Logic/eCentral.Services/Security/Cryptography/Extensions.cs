using System.Text;

namespace eCentral.Services.Security.Cryptography
{
    /// <summary>
    /// Extension methods for security cryptography
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert a byte array to string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ByteArrayToString(this byte[] input)
        {
            var enc = new ASCIIEncoding();
            return enc.GetString(input);
        }

        /// <summary>
        /// String to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(this string input)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(input);
        }
    }
}
