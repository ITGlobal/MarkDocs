using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Extension methods for <see cref="IPage"/>
    /// </summary>
    [PublicAPI]
    public static class PageExtensions
    {
        /// <summary>
        ///     Reads page markup as a string
        /// </summary>
        /// <param name="page">
        ///     Documentation page
        /// </param>
        /// <returns>
        ///     Page markup
        /// </returns>
        [PublicAPI, NotNull]
        public static string ReadMarkupString([NotNull] this IPage page)
        {
            using (var stream = page.ReadMarkup())
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }

        /// <summary>
        ///     Reads page markup as a string (asynchronously)
        /// </summary>
        /// <param name="page">
        ///     Documentation page
        /// </param>
        /// <param name="cancellationToken">
        ///     Cancellation token
        /// </param>
        /// <returns>
        ///     Page markup
        /// </returns>
        [PublicAPI, NotNull]
        public static async Task<string> ReadMarkupStringAsync([NotNull] this IPage page, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = page.ReadMarkup())
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();
                return str;
            }
        }

        /// <summary>
        ///     Reads page HTML as a string
        /// </summary>
        /// <param name="page">
        ///     Documentation page
        /// </param>
        /// <returns>
        ///     Page HTML
        /// </returns>
        [PublicAPI, NotNull]
        public static string ReadHtmlString([NotNull] this IPage page)
        {
            using (var stream = page.ReadHtml())
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }

        /// <summary>
        ///     Reads page HTML as a string (asynchronously)
        /// </summary>
        /// <param name="page">
        ///     Documentation page
        /// </param>
        /// <param name="cancellationToken">
        ///     Cancellation token
        /// </param>
        /// <returns>
        ///     Page HTML
        /// </returns>
        [PublicAPI, NotNull]
        public static async Task<string> ReadHtmlStringAsync([NotNull] this IPage page, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = page.ReadHtml())
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();
                return str;
            }
        }

        /// <summary>
        ///     Reads page preview as a string
        /// </summary>
        /// <param name="page">
        ///     Documentation page
        /// </param>
        /// <returns>
        ///     Page HTML
        /// </returns>
        [PublicAPI, NotNull]
        public static string ReadPreviewHtmlString([NotNull] this IPage page)
        {
            using (var stream = page.ReadPreviewHtml())
            using (var reader = new StreamReader(stream))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }

        /// <summary>
        ///     Reads page preview as a string (asynchronously)
        /// </summary>
        /// <param name="page">
        ///     Documentation page
        /// </param>
        /// <param name="cancellationToken">
        ///     Cancellation token
        /// </param>
        /// <returns>
        ///     Page HTML
        /// </returns>
        [PublicAPI, NotNull]
        public static async Task<string> ReadPreviewHtmlStringAsync([NotNull] this IPage page, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var stream = page.ReadPreviewHtml())
            using (var reader = new StreamReader(stream))
            {
                var str = await reader.ReadToEndAsync();
                return str;
            }
        }
    }
}