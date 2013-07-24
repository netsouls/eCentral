using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace eCentral.Core
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static string FormatWith(this string instance, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, instance, args);
        }
        
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static bool IsCaseInsensitiveEqual(this string instance, string comparing)
        {
            return (string.Compare(instance, comparing, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static bool IsCaseSensitiveEqual(this string instance, string comparing)
        {
            return (string.CompareOrdinal(instance, comparing) == 0);
        }

        public static bool IsEmpty(this Guid instanace)
        {
            return instanace.Equals(Guid.Empty);
        }

        public static string Compress(this string instance)
        {
            byte[] buffer;
            Guard.IsNotNullOrEmpty(instance, "instance");
            byte[] bytes = Encoding.UTF8.GetBytes(instance);
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream zipstream = new GZipStream(stream, CompressionMode.Compress))
                {
                    zipstream.Write(bytes, 0, bytes.Length);
                }
                buffer = stream.ToArray();
            }
            byte[] dst = new byte[buffer.Length + 4];
            Buffer.BlockCopy(buffer, 0, dst, 4, buffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, dst, 0, 4);
            return Convert.ToBase64String(dst);
        }

        public static string Decompress(this string instance)
        {
            string str;
            Guard.IsNotNullOrEmpty(instance, "instance");
            byte[] buffer = Decode(instance);
            if (buffer.Length < 4)
            {
                return string.Empty;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                int num = BitConverter.ToInt32(buffer, 0);
                stream.Write(buffer, 4, buffer.Length - 4);
                byte[] buffer2 = new byte[num];
                stream.Seek(0L, SeekOrigin.Begin);
                GZipStream stream2 = new GZipStream(stream, CompressionMode.Decompress);
                try
                {
                    stream2.Read(buffer2, 0, buffer2.Length);
                    str = Encoding.UTF8.GetString(buffer2);
                }
                catch (InvalidDataException)
                {
                    str = string.Empty;
                }
                finally
                {
                    if (stream2 != null)
                    {
                        stream2.Dispose();
                    }
                }
            }
            return str;
        }

        private static byte[] Decode(string value)
        {
            try
            {
                return Convert.FromBase64String(value);
            }
            catch (FormatException)
            {
                return new byte[0];
            }
        }


    }
}
