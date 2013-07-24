using System.IO;
using System.Web;

namespace eCentral.Services.Media
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the file binary array
        /// </summary>
        /// <param name="postedFile">Posted file</param>
        /// <returns>binary array</returns>
        public static byte[] GetBits(this HttpPostedFileBase postedFile)
        {
            Stream fs = postedFile.InputStream;
            int size = postedFile.ContentLength;
            byte[] data = new byte[size];
            fs.Read(data, 0, size);
            return data;
        }
    }
}
