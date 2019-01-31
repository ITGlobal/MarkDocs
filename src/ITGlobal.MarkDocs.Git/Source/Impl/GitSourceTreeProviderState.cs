using System;
using System.Collections.Immutable;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class GitSourceTreeProviderState
    {
        private GitSourceTreeProviderState(
            ImmutableArray<GitSourceTree> list,
            ImmutableDictionary<string, GitSourceTree> byId,
            ImmutableDictionary<string, GitSourceTree> byRemoteName)
        {
            List = list;
            ById = byId;
            ByRemoteName = byRemoteName;
        }

        public ImmutableArray< GitSourceTree> List { get; }
        public ImmutableDictionary<string, GitSourceTree> ById { get; }
        public ImmutableDictionary<string, GitSourceTree> ByRemoteName { get; }

        public static GitSourceTreeProviderState Empty { get; }
            = new GitSourceTreeProviderState(
                ImmutableArray<GitSourceTree>.Empty,
                ImmutableDictionary.Create<string, GitSourceTree>(StringComparer.OrdinalIgnoreCase),
                ImmutableDictionary.Create<string, GitSourceTree>(StringComparer.OrdinalIgnoreCase)
            );

        public GitSourceTreeProviderState AddOrUpdate(GitSourceTree tree)
        {
            var list = List;
            if (!ById.ContainsKey(tree.Id))
            {
                list = list.Add(tree);
            }

            var byId = ById;
            byId = byId.SetItem(tree.Id, tree);

            var byRemoteName = ByRemoteName;
            byRemoteName = byRemoteName.SetItem(tree.BranchOrTagName, tree);

            return new GitSourceTreeProviderState(list, byId, byRemoteName);
        }

        public GitSourceTreeProviderState Remove(GitSourceTree tree)
        {
            var list = List;
            if (ById.ContainsKey(tree.Id))
            {
                list = list.Remove(tree);
            }

            var byId = ById;
            byId = byId.Remove(tree.Id);

            var byRemoteName = ByRemoteName;
            byRemoteName = byRemoteName.Remove(tree.BranchOrTagName);

            return new GitSourceTreeProviderState(list, byId, byRemoteName);
        }
    }
}