using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Documentation source tree root 
    /// </summary>
    [PublicAPI]
    public interface ISourceTreeRoot
    {
        /// <summary>
        ///     Reference to parent tree object
        /// </summary>
        [NotNull]
        ISourceTree SourceTree { get; }

        /// <summary>
        ///     Path to root directory
        /// </summary>
        [NotNull]
        string RootDirectory { get; }

        /// <summary>
        ///     Source version information
        /// </summary>
        [NotNull]
        ISourceInfo SourceInfo { get; }

        /// <summary>
        ///     Retrieves a value for <see cref="PageMetadata.ContentId"/> from a file path
        /// </summary>
        [CanBeNull]
        string GetContentId([NotNull] string path);

        /// <summary>
        ///     Retrieves a value for <see cref="PageMetadata.LastChangedBy"/> from a file path
        /// </summary>
        [CanBeNull]
        string GetLastChangeAuthor([NotNull] string path);
    }
}