namespace ITGlobal.MarkDocs.Blog.Impl
{
    /// <summary>
    ///     Blog permalink
    /// </summary>
    internal sealed class BlogPermalink : IBlogPermalink
    {
        public BlogPermalink(IBlogPost post, string id)
        {
            Id = id;
            Post = post;
        }

        /// <summary>
        ///     Resource ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     A reference to a blog engine
        /// </summary>
        public IBlogEngine Engine => Post.Engine;

        /// <summary>
        ///     Name of resource file with extension (only name, not a full path)
        /// </summary>
        public string RelativePath => Post.RelativePath;

        /// <summary>
        ///     Resource type
        /// </summary>
        public BlogResourceType Type => BlogResourceType.Permalink;

        /// <summary>
        ///     Blog post reference
        /// </summary>
        public IBlogPost Post { get; }
    }
}