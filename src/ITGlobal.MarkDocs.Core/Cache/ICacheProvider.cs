using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Provides content caching services
    /// </summary>
    [PublicAPI]
    public interface ICacheProvider
    {
        /// <summary>
        ///     Loads all documentation models stored in cache
        /// </summary>
        [NotNull]
        CacheDocumentationModel[] Load();

        /// <summary>
        ///     Starts update operation
        /// </summary>
        /// <returns>
        ///     Cache update operation
        /// </returns>
        [NotNull]
        ICacheUpdateTransaction BeginTransaction(
            [NotNull] ISourceTree sourceTree,
            [NotNull] ISourceInfo sourceInfo,
            bool forceCacheClear = false
        );

        /// <summary>
        ///     Drops all cached content for a no-longer-needed documentation branch
        /// </summary>
        void Drop([NotNull] string documentationId);
    }
}