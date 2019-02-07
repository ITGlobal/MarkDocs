using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog permalink
    /// </summary>
    [PublicAPI]
    public interface IBlogPermalink : IBlogResource
    {
        /// <summary>
        ///     Blog post reference
        /// </summary>
        [NotNull]
        IBlogPost Post { get; }
    }
}