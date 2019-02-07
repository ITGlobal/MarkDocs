using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Cache item type
    /// </summary>
    [PublicAPI]
    public enum ResourceType
    {
        /// <summary>
        ///     Page
        /// </summary>
        [PublicAPI]
        Page,

        /// <summary>
        ///     Page preview
        /// </summary>
        [PublicAPI]
        PagePreview,

        /// <summary>
        ///     File
        /// </summary>
        [PublicAPI]
        File,

        /// <summary>
        ///     Generated file
        /// </summary>
        [PublicAPI]
        GeneratedFile
    }
}