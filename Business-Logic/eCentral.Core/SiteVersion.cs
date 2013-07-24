using System;
using System.Diagnostics;
using System.Reflection;

namespace eCentral.Core
{
    public static class SiteVersion
    {
        private static string currentVersion;
        private static string currentVersionHashValue;

        /// <summary>
        /// Gets or sets the store version
        /// </summary>
        public static string CurrentVersion 
        {
            get
            {
                if (!currentVersion.HasValue())
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

                    currentVersion = string.Format("{0}", fvi.ProductVersion);
                }

                return currentVersion;
            }
        }

        public static string CurrentVersionHashValue
        {
            get
            {
                if (!currentVersionHashValue.HasValue())
                {
                    // get the current version first and parse that as datetime 
                    object[] versionValues = CurrentVersion.Split(new char[] {'.'});

                    DateTime releaseDate = new DateTime(
                        Convert.ToInt32(versionValues[0]), Convert.ToInt32(versionValues[1]), Convert.ToInt32(versionValues[2]), 0, 0, 0, DateTimeKind.Utc);

                    currentVersionHashValue = releaseDate.ToBinary().ToString();
                }

                return currentVersionHashValue;
            }
        }
    }
}
