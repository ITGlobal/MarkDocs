using System;
using ITGlobal.MarkDocs;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     Comment static accessor
    /// </summary>
    [PublicAPI]
    public static class PageExtensions
    {
        /// <summary>
        ///     Gets comments for a page
        /// </summary>
        /// <param name="page">
        ///     Page
        /// </param>
        /// <returns>
        ///     Comments for a page
        /// </returns>
        [PublicAPI, NotNull]
        public static IPageComments GetComments([NotNull] this IPage page)
        {
            var extension = page.Documentation.Service.GetExtension<CommentsExtension>();
            if (extension == null)
            {
                throw new InvalidOperationException($"{typeof(CommentsExtension)} is not registered");
            }

            return extension.GetPageComments(page);
        }
    }
}