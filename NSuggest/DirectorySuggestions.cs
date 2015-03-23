using System.Collections.Generic;
using System.IO;

namespace NSuggest
{
    public sealed class DirectorySuggestions : IProvideSuggestions
    {
        public IEnumerable<string> For(string prefix)
        {
            if (prefix.Length < 2 || prefix[1] != ':')
                return null;

            var lastSlashPos = prefix.LastIndexOf('\\');
            var baseFolder = prefix;
            string partialMatch = null;
            if (lastSlashPos != -1)
            {
                baseFolder = prefix.Substring(0, lastSlashPos);
                partialMatch = prefix.Substring(lastSlashPos + 1);
            }
            try
            {
                return Directory.GetDirectories(baseFolder + '\\', partialMatch + "*");
            }
            catch
            {
                return null;
            }
        }
    }
}