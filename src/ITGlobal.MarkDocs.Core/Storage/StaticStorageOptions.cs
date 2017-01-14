using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Options for static directory storage
    /// </summary>
    [PublicAPI]
    public sealed class StaticStorageOptions
    {
        /// <summary>
        ///     Path to source directory
        /// </summary>
        [PublicAPI, NotNull]
        public string Directory { get; set; }

        /// <summary>
        ///     Watch for source changes
        /// </summary>
        [PublicAPI]
        public bool EnableWatch { get; set; }

        /// <summary>
        ///     Map each subdirectory to a separate documentation
        /// </summary>
        [PublicAPI]
        public bool UseSubdirectories { get; set; }
    }
}