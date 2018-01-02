using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog resource type
    /// </summary>
    [PublicAPI]
    public enum BlogResourceType
    {
        /// <summary>
        ///     Post
        /// </summary>
        [PublicAPI]
        Post,

        /// <summary>
        ///     Permalink
        /// </summary>
        [PublicAPI]
        Permalink,

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