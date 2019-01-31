using System;
using System.IO;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Cache update operation
    /// </summary>
    [PublicAPI]
    public interface ICacheUpdateTransaction : IDisposable
    {
        /// <summary>
        ///     Writes a <see cref="DocumentationModel"/> into cache
        /// </summary>
        void Store([NotNull] DocumentationModel model);

        /// <summary>
        ///     Writes a <see cref="PageAsset"/> into cache if changed,
        ///     copied previous asset content otherwise
        /// </summary>
        void Store([NotNull] PageAsset asset, [NotNull] Action<Stream> write);

        /// <summary>
        ///     Writes a <see cref="PagePreviewAsset"/> into cache if changed,
        ///     copied previous asset content otherwise
        /// </summary>
        void Store([NotNull] PagePreviewAsset asset, [NotNull] Action<Stream> write);

        /// <summary>
        ///     Writes a <see cref="AttachmentAsset"/> into cache if changed,
        ///     copied previous asset content otherwise
        /// </summary>
        void Store([NotNull] AttachmentAsset asset, [NotNull] Action<Stream> write);

        /// <summary>
        ///     Writes a <see cref="GeneratedAsset"/> into cache if changed,
        ///     copied previous asset content otherwise
        /// </summary>
        void Store([NotNull] GeneratedAsset asset, [NotNull] Action<Stream> write);



        /// <summary>
        ///     Reads a previously cached <see cref="PageAsset"/>
        /// </summary>
        [NotNull]
        Stream Read([NotNull] PageAsset asset);

        /// <summary>
        ///     Reads a previously cached <see cref="PagePreviewAsset"/>
        /// </summary>
        [NotNull]
        Stream Read([NotNull] PagePreviewAsset asset);

        /// <summary>
        ///     Reads a previously cached <see cref="AttachmentAsset"/>
        /// </summary>
        [NotNull]
        Stream Read([NotNull] AttachmentAsset asset);

        /// <summary>
        ///     Reads a previously cached <see cref="GeneratedAsset"/>
        /// </summary>
        [NotNull]
        Stream Read([NotNull] GeneratedAsset asset);
        
        /// <summary>
        ///     Commits all cached content changes
        ///     and returns a <see cref="ICacheReader" /> that reads this cached content
        /// </summary>
        [NotNull]
        ICacheReader Commit();
    }
}