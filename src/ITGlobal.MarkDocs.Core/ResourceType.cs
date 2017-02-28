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
        ///     Attachment
        /// </summary>
        [PublicAPI]
        Attachment,

        /// <summary>
        ///     Illustration
        /// </summary>
        [PublicAPI]
        Illustration
    }
}