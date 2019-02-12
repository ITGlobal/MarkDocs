using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Content hash calculation service
    /// </summary>
    [PublicAPI]
    public interface IContentHashProvider
    {
        /// <summary>
        ///     Gets a hash for a specified file
        /// </summary>
        bool TryGetContentHash([NotNull] string path, out string contentHash);
    }
}