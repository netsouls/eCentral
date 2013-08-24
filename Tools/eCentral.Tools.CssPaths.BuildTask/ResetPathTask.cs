using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace eCentral.Tools.CssPaths.BuildTask
{
    /// <summary>
    /// MSBuild task to reset all the path
    /// </summary>
    public class ResetPathTask : Task
    {
        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        public override bool Execute()
        {
            var relativePath = "/library/images";

            // we need to read each individual file and reset the paths in that file
            foreach (var sourceFile in SourceFiles)
            {
                var cssContent = File.ReadAllText(sourceFile.ItemSpec);
                // replace image paths
                var relativePaths = FindDistinctRelativePathsIn(cssContent);

                foreach ( var data in relativePaths)
                {
                    cssContent = ReplaceRelativePathsIn(cssContent, data, relativePath);
                }

                // save the file again
                File.WriteAllText(sourceFile.ItemSpec, cssContent);
            }

            return true;
        }

        private string ReplaceRelativePathsIn(string css, string oldPath, string newPath)
        {
            var regex = new Regex(@"url\([""']{0,1}" + Regex.Escape(oldPath) + @"[""']{0,1}\)", RegexOptions.IgnoreCase);

            return regex.Replace(css, delegate(Match match)
            {
                var path = match.Value.Replace(oldPath, string.Format("{0}/{1}", newPath, oldPath));
                return path;
            });
        }

        private IEnumerable<string> FindDistinctRelativePathsIn(string css)
        {
            var matches = Regex.Matches(css, @"url\([""']{0,1}(.+?)[""']{0,1}\)", RegexOptions.IgnoreCase);
            var matchesHash = new HashSet<string>();
            foreach (Match match in matches)
            {
                var path = match.Groups[1].Captures[0].Value;
                if (!path.StartsWith("/") && !path.StartsWith("http://") && !path.StartsWith("https://") && !path.StartsWith("data:") && !path.StartsWith("squishit://"))
                {
                    if (matchesHash.Add(path))
                    {
                        yield return path;
                    }
                }
            }
        }
    }
}
