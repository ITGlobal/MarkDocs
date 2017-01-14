using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class SingleIgnoreRule : IIgnoreRule
    {
        private readonly string _directory;
        private readonly Matcher _matcher = new Matcher();

        public SingleIgnoreRule(string directory, IEnumerable<string> ignorePatters)
        {
            _matcher.AddIncludePatterns(ignorePatters);
            _directory = directory;
        }

        public bool ShouldIgnore(string path)
        {
            var matcherResult = _matcher.Match(_directory, path);
            return matcherResult.HasMatches;
        }
    }
}