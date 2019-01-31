using System.Collections.Generic;
using ITGlobal.MarkDocs.Tags.Impl;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Tags
{
    /// <summary>
    ///     Helper methods for tags service
    /// </summary>
    [PublicAPI]
    public static class TagsExtensionMethods
    {
        /// <summary>
        ///     Gets an instance of tags extension
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <returns>
        ///     Page tags service
        /// </returns>
        [NotNull]
        public static ITagService GetTagService([NotNull] this IDocumentation documentation)
        {
            var extension = documentation.Service.GetExtension<TagsExtension>();
            return extension;
        }

        /// <summary>
        ///     Gets a list of normalized page tags
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        [NotNull]
        public static IReadOnlyList<string> GetTags([NotNull] this IDocumentation documentation)
        {
            var extension = documentation.GetTagService();
            return extension.GetTags(documentation);
        }

        /// <summary>
        ///     Gets a list of normalized page tags
        /// </summary>
        /// <param name="page">
        ///     Page
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        [NotNull]
        public static IReadOnlyList<string> GetPageTags([NotNull] this IPage page)
        {
            var extension = page.Documentation.GetTagService();
            return extension.GetPageTags(page);
        }

        /// <summary>
        ///     Gets a list of normalized page tags
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <param name="tag">
        ///     Tag
        /// </param>
        /// <returns>
        ///     List of pages
        /// </returns>
        [NotNull]
        public static IReadOnlyList<IPage> GetPagesByTag([NotNull] this IDocumentation documentation, [NotNull] string tag)
        {
            var extension = documentation.GetTagService();
            return extension.GetPagesByTag(documentation, tag);
        }
    }
}