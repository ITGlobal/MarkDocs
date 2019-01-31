using System.Collections.Immutable;

namespace ITGlobal.MarkDocs.Tags.Impl
{
    internal sealed class TagsExtensionState
    {
        private TagsExtensionState(ImmutableDictionary<string, TagsExtensionStateBranch> byDocumentation)
        {
            ByDocumentation = byDocumentation;
        }

        public ImmutableDictionary<string, TagsExtensionStateBranch> ByDocumentation { get; }

        public static TagsExtensionState Empty { get; }
            = new TagsExtensionState(ImmutableDictionary<string, TagsExtensionStateBranch>.Empty);

        public TagsExtensionStateBranch GetBranch(IDocumentation documentation)
        {
            if (!ByDocumentation.TryGetValue(documentation.Id, out var branch))
            {
                return null;
            }

            return branch;
        }

        public TagsExtensionState AddOrUpdate(IDocumentation documentation)
        {
            var dict = ByDocumentation;
            dict = dict.SetItem(documentation.Id, new TagsExtensionStateBranch(documentation));
            return new TagsExtensionState(dict);
        }

        public TagsExtensionState Remove(IDocumentation documentation)
        {
            var dict = ByDocumentation;
            dict = dict.Remove(documentation.Id);
            return new TagsExtensionState(dict);
        }
    }
}