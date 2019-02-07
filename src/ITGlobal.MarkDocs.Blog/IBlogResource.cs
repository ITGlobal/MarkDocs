using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog resource (post, slug or an attachment)
    /// </summary>
    [PublicAPI]
    public interface IBlogResource
    {
        /// <summary>
        ///     Resource ID
        /// </summary>
        [NotNull]
        string Id { get; }

        /// <summary>
        ///     A reference to a blog engine
        /// </summary>
        [NotNull]
        IBlogEngine Engine { get; }

        /// <summary>
        ///     Name of resource file with extension
        /// </summary>
        [NotNull]
        string RelativePath { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        BlogResourceType Type { get; }
    }
}