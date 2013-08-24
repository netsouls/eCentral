using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;

namespace eCentral.Tools.AssemblyVersioning.BuildTask
{
    /// <summary>
    /// Task to auto increment the build date 
    /// </summary>
    public class AutoIncrementTask : Task
    {
        #region Fields

        private const string versionPattern = @"\[assembly: Assembly(.*)?Version\(\""([0-9][0-9][0-9][0-9]).([0-1][0-9]).([0-3][0-9])(.\d{0,3})?""\)\]";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the assembly info file path
        /// </summary>
        public string AssemblyInfoPath { get; set; }

        #endregion

        public override bool Execute()
        {
            try
            {
                if ( string.IsNullOrEmpty(AssemblyInfoPath))
                    throw new ArgumentException("AssemblyInfoPath must have a value");

                Console.Out.WriteLine("Change Assembly Version for {0}", AssemblyInfoPath);

                // read the assembly info path
                string[] fileContent = File.ReadAllLines(AssemblyInfoPath, Encoding.Default);
                var regEx = new Regex(versionPattern);

                // iterate and find all the versioning numbers
                var newContent = new List<string>();
                fileContent.ToList().ForEach(line =>
                {
                    if (regEx.IsMatch(line))
                    {
                        line = VersionChanger(regEx.Match(line));
                        Console.Out.WriteLine("Incremented to: {0}", line);
                    }
                    newContent.Add(line);
                });

                // save the file again
                File.WriteAllLines(AssemblyInfoPath, newContent);
            }
            catch ( Exception exc)
            {
                Console.Out.WriteLine(exc.Message);
                return false;
            }

            return true;
        }

        private string VersionChanger(Match match)
        {
            // assembly version would be in the format yyyy.dd.mm and assembly file version would be in the format yyyy.dd.mm.revsion
            // first find the type of assembly version 
            string versionType = match.Groups[1].Value;

            // get the yyyy.mm.dd
            int yyyy = int.Parse(match.Groups[2].Value);
            int mm = int.Parse(match.Groups[3].Value);
            int dd = int.Parse(match.Groups[4].Value);

            var currentDate = DateTime.UtcNow;

            bool hasRevision = false; // we have the revision number only when change the assembly file version
            int revisionNumber = 1;

            if (!string.IsNullOrEmpty(match.Groups[5].Value))
            {
                hasRevision = true;
                revisionNumber = int.Parse(match.Groups[5].Value.Replace(".", string.Empty));

                // increment revision if same date
                if (yyyy == currentDate.Year && mm == currentDate.Month && dd == currentDate.Day)
                    ++revisionNumber;
                else
                    revisionNumber = 1;
            }

            var dateVersion = currentDate.ToString("yyyy.MM.dd");

            return string.Format(match.Result("[assembly: Assembly{0}Version(\"{1}{2}\")]"), 
                versionType, dateVersion, ( hasRevision ? string.Format(".{0}",revisionNumber) : string.Empty));
        }
    }
}
