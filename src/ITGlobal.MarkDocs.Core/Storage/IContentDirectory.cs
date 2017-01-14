using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Describes content directory
    /// </summary>
    [PublicAPI]
    public interface IContentDirectory
    {
        /// <summary>
        ///     Documentation ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     Path to a content directory
        /// </summary>
        [PublicAPI, NotNull]
        string Path { get; }

        /// <summary>
        ///     Content version information
        /// </summary>
        [PublicAPI, NotNull]
        IContentVersion ContentVersion { get; }
    }
}