using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Cache update operation
    /// </summary>
    [PublicAPI]
    public interface ICacheUpdateOperation : IDisposable
    {
        /// <summary>
        ///     Clears cached content for specified documentation
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        [PublicAPI]
        void Clear([NotNull] IDocumentation documentation);
        
        /// <summary>
        ///     Caches content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <param name="content">
        ///     Item content
        /// </param>
        [PublicAPI]
        void Write([NotNull] IResource item, [NotNull] IResourceContent content);

        /// <summary>
        ///     Flushes all cached content changes
        /// </summary>
        [PublicAPI]
        void Flush();

        /// <summary>
        ///     Commits all cached content changes
        /// </summary>
        [PublicAPI]
        void Commit();
    }
}