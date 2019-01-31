using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Options for static directory source tree
    /// </summary>
    [PublicAPI]
    public sealed class StaticDirectorySourceTreeOptions
    {
        /// <summary>
        ///     Pathes to source directories
        /// </summary>
        [PublicAPI, NotNull]
        public string[] Directories { get; set; }

        /// <summary>
        ///     Watch for source changes
        /// </summary>
        [PublicAPI]
        public bool WatchForChanges { get; set; }
    }
}