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
        Page,

        /// <summary>
        ///     Page preview
        /// </summary>
        PagePreview,

        /// <summary>
        ///     File
        /// </summary>
        File,

        /// <summary>
        ///     Generated file
        /// </summary>
        GeneratedFile,
    }
}