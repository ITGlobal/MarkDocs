using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Reads asset content from cache.
    ///     Each <see cref="ICacheReader"/> is linked to specific documentation branch.
    /// </summary>
    [PublicAPI]
    public interface ICacheReader
    {
        /// <summary>
        ///     Reads a cached content
        /// </summary>
        /// <exception cref="CachedAssetNotFoundException">
        ///     Thrown if a cached content is not found
        /// </exception>
        [NotNull]
        Stream Read([NotNull] IResource resource);
    }
}