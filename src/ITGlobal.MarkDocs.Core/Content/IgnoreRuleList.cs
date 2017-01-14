namespace ITGlobal.MarkDocs.Content
{
    internal sealed class IgnoreRuleList : IIgnoreRule
    {
        private readonly IIgnoreRule[] _ignoreRules;

        public IgnoreRuleList(IIgnoreRule[] ignoreRules)
        {
            _ignoreRules = ignoreRules;
        }

        public bool ShouldIgnore(string path)
        {
            foreach (var rule in _ignoreRules)
            {
                if (rule.ShouldIgnore(path))
                {
                    return true;
                }
            }

            return false;
        }
    }
}