using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog resource (post, slug or an attachment)
    /// </summary>
    public interface IBlogResource
    {
        /// <summary>
        ///     Resource ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     A reference to a blog engine
        /// </summary>
        [PublicAPI, NotNull]
        IBlogEngine Engine { get; }

        /// <summary>
        ///     Name of resource file with extension (only name, not a full path)
        /// </summary>
        [PublicAPI, NotNull]
        string FileName { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        [PublicAPI]
        BlogResourceType Type { get; }
    }
}