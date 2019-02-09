using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache.Impl
{
    internal interface IAssetStoreContext
    {
        [NotNull]
        IMarkDocsLog Log { get; }
        [NotNull]
        ISourceTree SourceTree { get; }
        bool DisableCache { get; }

        string RootDirectory { get; }

        [NotNull]
        CompilationEventListener EventListener { get; }

        [CanBeNull]
        string OldDirectory { get; }
        string NewDirectory { get; }

        [CanBeNull]
        DiskCacheIndex OldIndex { get; }
        DiskCacheIndex NewIndex { get; }
    }
}