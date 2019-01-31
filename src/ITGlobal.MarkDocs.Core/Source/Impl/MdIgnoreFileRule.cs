using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class MdIgnoreFileRule : IIgnoreRule
    {
        public const string FileName = ".mdignore";

        private readonly string _directory;
        private readonly string _filename;
        private readonly Matcher _matcher = new Matcher();

        public MdIgnoreFileRule(string directory, string filename)
        {
            _directory = directory;
            _filename = filename;

            foreach (var line in File.ReadAllLines(filename, Encoding.UTF8)
                .Select(l => l.Trim())
                .Where(l=>!string.IsNullOrWhiteSpace(l)))
            {
                if (line.StartsWith("#"))
                {
                    continue;
                }

                if (line.StartsWith("!"))
                {
                    var pattern = line.Substring(1);
                    if (!string.IsNullOrEmpty(pattern))
                    {
                        _matcher.AddExclude(pattern);
                    }
                }
                else
                {
                    _matcher.AddInclude(line);
                }
            }
        }

        public bool ShouldIgnore(string path)
        {
            if (_filename == path)
            {
                return true;
            }
            
            var matcherResult = _matcher.Match(_directory, path);
            return matcherResult.HasMatches;
        }
    }
}