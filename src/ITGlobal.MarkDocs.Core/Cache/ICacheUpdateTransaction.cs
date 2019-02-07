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
        ///     Writes a <see cref="PhysicalFileAsset"/> into cache if changed,
        ///     copied previous asset content otherwise
        /// </summary>
        void Store([NotNull] PhysicalFileAsset asset, [NotNull] Action<Stream> write);

        /// <summary>
        ///     Writes a <see cref="GeneratedFileAsset"/> into cache if changed,
        ///     copied previous asset content otherwise
        /// </summary>
        void Store([NotNull] GeneratedFileAsset asset, [NotNull] Action<Stream> write);



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
        ///     Reads a previously cached <see cref="PhysicalFileAsset"/>
        /// </summary>
        [NotNull]
        Stream Read([NotNull] PhysicalFileAsset asset);

        /// <summary>
        ///     Reads a previously cached <see cref="GeneratedFileAsset"/>
        /// </summary>
        [NotNull]
        Stream Read([NotNull] GeneratedFileAsset asset);
        
        /// <summary>
        ///     Commits all cached content changes
        ///     and returns a <see cref="ICacheReader" /> that reads this cached content
        /// </summary>
        [NotNull]
        ICacheReader Commit();
    }
}