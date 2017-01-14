using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Git
{
    /// <summary>
    ///     Defines rules to include refs (branches or tags) into documentation
    /// </summary>
    [PublicAPI]
    public sealed class RefInclusionRules
    {
        private sealed class RegexpList
        {
            private readonly List<Regex> _regexes = new List<Regex>();
            private string[] _rules = new string[0];
            private bool _upToDate;

            public RegexpList(params string[] rules)
            {
                Rules = rules;
            }

            public string[] Rules
            {
                get { return _rules; }
                set
                {
                    _rules = value;
                    _upToDate = false;
                }
            }

            public bool IsMatch(string str)
            {
                if (!_upToDate)
                {
                    _regexes.Clear();
                    foreach (var rule in _rules)
                    {
                        _regexes.Add(new Regex(ConvertRuleToPattern(rule), RegexOptions.IgnoreCase));
                    }

                    _upToDate = true;
                }

                return _regexes.Any(regex => regex.IsMatch(str));
            }

            private static string ConvertRuleToPattern(string rule)
            {
                var pattern = new StringBuilder();
                foreach (var c in rule)
                {
                    if (char.IsLetterOrDigit(c))
                    {
                        pattern.Append(c);
                    }
                    else if (c == '*')
                    {
                        pattern.Append('.');
                        pattern.Append('*');
                    }
                    else
                    {
                        pattern.Append('\\');
                        pattern.Append(c);
                    }
                }

                return pattern.ToString();
            }
        }

        private readonly RegexpList _include = new RegexpList("*");
        private readonly RegexpList _exclude = new RegexpList();

        /// <summary>
        ///     Indicates whether git storage should use refs as a documentation version source
        /// </summary>
        [PublicAPI]
        public bool Use { get; set; }

        /// <summary>
        ///     Sets ref name patterns to include. A "*" wildcard is allowed.
        ///     A ref will produce a documentation version if its name matches any of <see cref="Include"/> patterns
        ///     or if not patterns were supplied (so empty array means that any branch should pass).
        ///     Note that <see cref="Exclude"/> parameter takes priority.
        /// </summary>
        [PublicAPI, NotNull]
        public string[] Include
        {
            get { return _include.Rules; }
            set { _include.Rules = value; }
        }

        /// <summary>
        ///     Sets ref name patterns to exclude. A "*" wildcard is allowed.
        ///     A ref won't produce a documentation version if its name matches any of <see cref="Exclude"/> patterns.
        /// </summary>
        [PublicAPI, NotNull]
        public string[] Exclude
        {
            get { return _exclude.Rules; }
            set { _exclude.Rules = value; }
        }

        internal IEnumerable<RemoteInfo> Filter(IEnumerable<RemoteInfo> remotes)
        {
            if (!Use)
            {
                yield break;
            }

            foreach (var remote in remotes)
            {
                if (_include.IsMatch(remote.Name) && !_exclude.IsMatch(remote.Name))
                {
                    yield return remote;
                }
            }
        }
    }
}