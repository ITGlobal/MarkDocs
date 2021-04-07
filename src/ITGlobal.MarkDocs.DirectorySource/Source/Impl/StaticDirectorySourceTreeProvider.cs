using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class StaticDirectorySourceTreeProvider : ISourceTreeProvider, IDisposable
    {

        private readonly StaticDirectorySourceTreeOptions _options;
        private readonly IAssetTreeReader _reader;

        private readonly object _sourceTreesLock = new object();

        private ImmutableDictionary<string, StaticDirectorySourceTree> _sourceTrees
            = ImmutableDictionary<string, StaticDirectorySourceTree>.Empty;

        public StaticDirectorySourceTreeProvider(StaticDirectorySourceTreeOptions options, IAssetTreeReader reader)
        {
            _options = options;
            _reader = reader;

            Refresh();
        }

        public string[] IgnorePatterns => null;

        public event EventHandler Changed;
        private void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public ISourceTree[] GetSourceTrees()
        {
            lock (_sourceTreesLock)
            {
                return _sourceTrees.Values.OrderBy(_ => _.Id).Cast<ISourceTree>().ToArray();
            }
        }

        public ISourceTree GetSourceTree(string id)
        {
            lock (_sourceTreesLock)
            {
                _sourceTrees.TryGetValue(id, out var sourceTree);
                return sourceTree;
            }
        }

        public void Refresh()
        {
            var hasAnyChanges = false;
            ImmutableDictionary<string, StaticDirectorySourceTree> sourceTrees;
            lock (_sourceTreesLock)
            {
                sourceTrees = _sourceTrees;
            }

            var toBeDeleted = new HashSet<string>(sourceTrees.Keys);
            var toBeRefreshed = new List<StaticDirectorySourceTree>();
            foreach (var directory in _options.Directories)
            {
                if (Directory.Exists(directory))
                {
                    var id = Path.GetFileName(directory).ToLowerInvariant();
                    if (string.IsNullOrEmpty(id))
                    {
                        id = Path.GetFileName(Path.GetDirectoryName(directory))!.ToLowerInvariant();
                    }

                    toBeDeleted.Remove(id);

                    if (sourceTrees.TryGetValue(id, out var tree))
                    {
                        toBeRefreshed.Add(tree);
                        continue;
                    }

                    tree = new StaticDirectorySourceTree(this, _reader, id, directory, _options.WatchForChanges);
                    sourceTrees = sourceTrees.SetItem(id, tree);

                    hasAnyChanges = true;
                }
            }

            foreach (var id in toBeDeleted)
            {
                sourceTrees = sourceTrees.Remove(id);
                hasAnyChanges = true;
            }

            if (hasAnyChanges)
            {
                lock (_sourceTreesLock)
                {
                    _sourceTrees = sourceTrees;
                }

                NotifyChanged();
            }

            foreach (var sourceTree in toBeRefreshed)
            {
                sourceTree.Refresh();
            }
        }

        public void Dispose()
        {
            lock (_sourceTreesLock)
            {
                foreach (var sourceTree in _sourceTrees.Values)
                {
                    sourceTree.Dispose();
                }

                _sourceTrees = _sourceTrees.Clear();
            }
        }

    }
}