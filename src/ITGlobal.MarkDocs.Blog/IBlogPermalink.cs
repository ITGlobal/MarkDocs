using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Blog permalink
    /// </summary>
    public interface IBlogPermalink : IBlogResource
    {
        /// <summary>
        ///     Blog post reference
        /// </summary>
        [PublicAPI, NotNull]
        IBlogPost Post { get; }
    }
}