using System;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A listener for MarkDocs compilation events
    /// </summary>
    [PublicAPI]
    public abstract class CompilationEventListener : IDisposable
    {
        /// <summary>
        ///     This method is called when MarkDocs starts reading asset tree
        /// </summary>
        public virtual void ReadingAssetTree() { }

        /// <summary>
        ///     This method is called when MarkDocs starts processing asset tree
        /// </summary>
        public virtual void ProcessingAssets([NotNull] AssetTree tree) { }

        /// <summary>
        ///     This method is called when MarkDocs starts flushing processed assets
        /// </summary>
        public virtual void Committing() { }
        
        /// <summary>
        ///     This method is called when MarkDocs completed compilation
        /// </summary>
        public virtual void Completed(TimeSpan elapsed) { }

        /// <summary>
        ///     Handles a warning
        /// </summary>
        public virtual void Warning(string filename, string message, string location = null) { }

        /// <summary>
        ///     Handles an error
        /// </summary>
        public virtual void Error(string message) { }

        /// <summary>
        ///     Handles an error
        /// </summary>
        public virtual void Error(string filename, string message, string location = null) { }

        /// <summary>
        ///     This method is called when an asset is written
        /// </summary>
        public virtual void Written(Asset asset) { }

        /// <summary>
        ///     This method is called when a cached asset is reused
        /// </summary>
        public virtual void Cached(Asset asset) { }

        /// <summary>
        ///     This method is called when a cached asset is reused
        /// </summary>
        public virtual void Cached(string assetId) { }

        /// <inheritdoc />
        public virtual void Dispose() { }
    }
}