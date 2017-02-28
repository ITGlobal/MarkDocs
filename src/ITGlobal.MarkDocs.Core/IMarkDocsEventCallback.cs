using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A callback handler for MarkDocs lifetime events
    /// </summary>
    [PublicAPI]
    public interface IMarkDocsEventCallback
    {
        /// <summary>
        ///     This method is called when MarkDocs starts to refresh sources of all documentations
        /// </summary>
        [PublicAPI]
        void StorageRefreshAllStarted();

        /// <summary>
        ///     This method is called when MarkDocs completes to refresh sources of all documentations
        /// </summary>
        [PublicAPI]
        void StorageRefreshAllCompleted();

        /// <summary>
        ///     This method is called when MarkDocs starts to refresh sources of documentation <paramref name="id"/>
        /// </summary>
        [PublicAPI]
        void StorageRefreshStarted([NotNull] string id);

        /// <summary>
        ///     This method is called when MarkDocs completes to refresh sources of documentation <paramref name="id"/>
        /// </summary>
        [PublicAPI]
        void StorageRefreshCompleted([NotNull] string id);

        /// <summary>
        ///     This method is called when MarkDocs starts to compile all documentations
        /// </summary>
        [PublicAPI]
        void CompilationStarted();

        /// <summary>
        ///     This method is called when MarkDocs completes to compile all documentations
        /// </summary>
        [PublicAPI]
        void CompilationCompleted();

        /// <summary>
        ///     This method is called when MarkDocs starts to compile documentation <paramref name="id"/>
        /// </summary>
        [PublicAPI]
        void CompilationStarted([NotNull] string id);

        /// <summary>
        ///     This method is called when MarkDocs starts to compile documentation <paramref name="id"/>
        /// </summary>
        [PublicAPI]
        void CompilationCompleted([NotNull] string id);

        /// <summary>
        ///     This method is called when MarkDocs detects source changes
        /// </summary>
        [PublicAPI]
        void StorageChanged([CanBeNull] string id = null);

        /// <summary>
        ///     This method is called when MarkDocs is compiling specified page
        /// </summary>
        [PublicAPI]
        void CompilingPage([NotNull] string documentationId, [NotNull] string id, int i, int count);

        /// <summary>
        ///     This method is called when MarkDocs is caching specified attachment
        /// </summary>
        [PublicAPI]
        void CachingAttachment([NotNull] string documentationId, [NotNull] string id, int i, int count);

        /// <summary>
        ///     This method is called when MarkDocs fails to render a page
        /// </summary>
        [PublicAPI]
        void Warning([NotNull] string documentationId, [NotNull]  string pageId, [NotNull] string message, [CanBeNull] string location = null);

        /// <summary>
        ///     This method is called when MarkDocs detects non-fatal issues when rendering a page
        /// </summary>
        [PublicAPI]
        void Error([NotNull] string documentationId, [NotNull] string pageId, [NotNull] Exception e);
    }
}