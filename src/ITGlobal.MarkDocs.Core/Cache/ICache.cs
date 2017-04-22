using System.Collections.Generic;
using System.IO;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Provides content caching services
    /// </summary>
    [PublicAPI]
    public interface ICache
    {
        /// <summary>
        ///     Checks whether cache is up to date
        /// </summary>
        [PublicAPI]
        CacheVerifyResult Verify([NotNull] IReadOnlyList<IContentDirectory> contentDirectories);

        /// <summary>
        ///     Starts update operation
        /// </summary>
        /// <returns>
        ///     Cache update operation
        /// </returns>
        [PublicAPI, NotNull]
        ICacheUpdateOperation BeginUpdate();

        /// <summary>
        ///     Gets a cached content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <returns>
        ///     Cached content or null
        /// </returns>
        [PublicAPI, CanBeNull]
        Stream Read([NotNull] IResource item);
    }
}