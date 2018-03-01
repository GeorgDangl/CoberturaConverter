using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoberturaConverter.Core.DotCover
{
    public static class DotCoverBaseDirectoryFinder
    {
        public static string GetBaseDirectory(IEnumerable<string> filePaths)
        {
            if (filePaths == null)
            {
                throw new ArgumentNullException(nameof(filePaths));
            }

            // TODO Check Google, there's for sure a much more efficient way to do this

            var filePathList = filePaths.ToList();

            var initialFilePath = filePathList[0];

            string commonStart = string.Empty;
            for (var i = 1; i < initialFilePath.Length; i++)
            {
                var substring = initialFilePath.Substring(0, i);
                if (filePathList.All(p => p.StartsWith(substring)))
                {
                    commonStart = substring;
                }
                else
                {
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(commonStart))
            {
                return string.Empty;
            }

            commonStart = Path.GetDirectoryName(commonStart);

            return commonStart
                .TrimEnd('/')
                .TrimEnd('\\');
        }
    }
}
