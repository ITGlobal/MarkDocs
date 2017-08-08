using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Default implementation of <see cref="IMarkDocsEventCallback"/>
    /// </summary>
    [PublicAPI]
    public class MarkDocsEventCallback : IMarkDocsEventCallback
    {
        private sealed class EmptyToken: IDisposable
        {
            public void Dispose() { }
        }

        /// <summary>
        ///     This method is called when MarkDocs starts to refresh sources of all documentations
        /// </summary>
        public virtual IDisposable StorageRefresh(string documentationId = null) => new EmptyToken();

        /// <summary>
        ///     This method is called when MarkDocs starts to compile all documentations
        /// </summary>
        public virtual IDisposable CompilationStarted() => new EmptyToken();

        /// <summary>
        ///     This method is called when MarkDocs starts to compile documentation <paramref name="id"/>
        /// </summary>
        public virtual IDisposable CompilationStarted(string id) => new EmptyToken();

        /// <summary>
        ///     This method is called when MarkDocs detects source changes
        /// </summary>
        public virtual void StorageChanged(string id = null) { }

        /// <summary>
        ///     This method is called when MarkDocs is compiling specified page
        /// </summary>
        public virtual void CompilingPage(string documentationId, string id, int i, int count) { }

        /// <summary>
        ///     This method is called when MarkDocs is caching specified attachment
        /// </summary>
        public virtual void CachingAttachment(string documentationId, string id, int i, int count) { }

        /// <summary>
        ///     This method is called when MarkDocs fails to render a page
        /// </summary>
        public virtual void Warning(string documentationId, string pageId, string message, string location = null) {}

        /// <summary>
        ///     This method is called when MarkDocs detects non-fatal issues when rendering a page
        /// </summary>
        public virtual void Error(string documentationId, string pageId, Exception e) { }

        /// <summary>
        ///     This method is called when MarkDocs detects non-fatal issues when extracting file metadata
        /// </summary>
        public virtual void MetadataError(string filename, Exception e) { }
    }
}