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
        [PublicAPI, NotNull]
        IDisposable StorageRefresh(string documentationId = null);

        /// <summary>
        ///     This method is called when MarkDocs starts to compile all documentations
        /// </summary>
        [PublicAPI, NotNull]
        IDisposable CompilationStarted();

        /// <summary>
        ///     This method is called when MarkDocs starts to compile documentation <paramref name="id"/>
        /// </summary>
        [PublicAPI, NotNull]
        IDisposable CompilationStarted([NotNull] string id);
        
        /// <summary>
        ///     This method is called when MarkDocs detects source changes
        /// </summary>
        [PublicAPI]
        void StorageChanged([CanBeNull] string id = null);

        /// <summary>
        ///     This method is called when MarkDocs is parsing specified page
        /// </summary>
        [PublicAPI]
        void ParsedPage([NotNull] string documentationId, [NotNull] string id, int i, int count);

        /// <summary>
        ///     This method is called when MarkDocs is rendering specified page
        /// </summary>
        [PublicAPI]
        void RenderedPage([NotNull] string documentationId, [NotNull] string id, int i, int count);

        /// <summary>
        ///     This method is called when MarkDocs is caching specified attachment
        /// </summary>
        [PublicAPI]
        void CachedAttachment([NotNull] string documentationId, [NotNull] string id, int i, int count);

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

        /// <summary>
        ///     This method is called when MarkDocs detects non-fatal issues when extracting file metadata
        /// </summary>
        [PublicAPI]
        void MetadataError([NotNull] string filename, [NotNull] Exception e);
    }
}