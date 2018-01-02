using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Extension methods for <see cref="IBlogPost"/>
    /// </summary>
    [PublicAPI]
    public static class BlogPostExtensions
    {
        /// <summary>
        ///     Reads blog post markup as a string
        /// </summary>
        /// <param name="post">
        ///     Blog post
        /// </param>
        /// <returns>
        ///     Blog post markup
        /// </returns>
        [PublicAPI, NotNull]
        public static string ReadMarkupString([NotNull] this IBlogPost post)
        {
            using (var stream = post.ReadMarkup())
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }

        /// <summary>
        ///     Reads blog post markup as a string (asynchronously)
        /// </summary>
        /// <param name="post">
        ///     Blog post 
        /// </param>
        /// <param name="cancellationToken">
        ///     Cancellation token
        /// </param>
        /// <returns>
        ///     Blog post markup
        /// </returns>
        [PublicAPI, NotNull]
        public static async Task<string> ReadMarkupStringAsync([NotNull] this IBlogPost post, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = post.ReadMarkup())
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();
                return str;
            }
        }

        /// <summary>
        ///     Reads blog post HTML as a string
        /// </summary>
        /// <param name="post">
        ///     Blog post
        /// </param>
        /// <returns>
        ///     Blog post  HTML
        /// </returns>
        [PublicAPI, NotNull]
        public static string ReadHtmlString([NotNull] this IBlogPost post)
        {
            using (var stream = post.ReadHtml())
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }

        /// <summary>
        ///     Reads blog post HTML as a string (asynchronously)
        /// </summary>
        /// <param name="post">
        ///     Blog post 
        /// </param>
        /// <param name="cancellationToken">
        ///     Cancellation token
        /// </param>
        /// <returns>
        ///     Blog post HTML
        /// </returns>
        [PublicAPI, NotNull]
        public static async Task<string> ReadHtmlStringAsync([NotNull] this IBlogPost post, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = post.ReadHtml())
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();
                return str;
            }
        }
    }
}