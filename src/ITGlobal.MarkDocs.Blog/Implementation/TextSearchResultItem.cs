namespace ITGlobal.MarkDocs.Blog.Implementation
{
    /// <summary>
    ///     A text search result item
    /// </summary>
    internal sealed class TextSearchResultItem : ITextSearchResultItem
    {
        public TextSearchResultItem(IBlogPost post, string preview)
        {
            Post = post;
            Preview = preview;
        }

        /// <summary>
        ///     Blog post
        /// </summary>
        public IBlogPost Post { get; }

        /// <summary>
        ///     Preview (HTML)
        /// </summary>
        public string Preview { get; }
    }
}