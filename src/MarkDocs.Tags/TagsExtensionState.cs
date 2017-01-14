using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tags
{
    internal sealed class TagsExtensionState
    {
        public TagsExtensionState(IMarkDocServiceState state)
        {
            ByDocumentation = state.List.ToDictionary(_ => _, doc => new TagsExtensionStateBranch(doc));
        }

        public Dictionary<IDocumentation, TagsExtensionStateBranch> ByDocumentation { get; }

        public TagsExtensionStateBranch GetBranch(IDocumentation documentation)
        {
            TagsExtensionStateBranch branch;
            if (!ByDocumentation.TryGetValue(documentation, out branch))
            {
                return null;
            }

            return branch;
        }
    }
}