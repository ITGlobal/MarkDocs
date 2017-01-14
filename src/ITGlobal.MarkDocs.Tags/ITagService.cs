using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Tags
{
    /// <summary>
    ///     Page tags service
    /// </summary>
    [PublicAPI]
    public interface ITagService
    {
        /// <summary>
        ///     Gets a list of all known tags
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        [PublicAPI, NotNull]
        IReadOnlyList<string> GetTags([NotNull] IDocumentation documentation);

        /// <summary>
        ///     Gets a list of normalized page tags
        /// </summary>
        /// <param name="page">
        ///     Page
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        [PublicAPI, NotNull]
        IReadOnlyList<string> GetPageTags([NotNull] IPage page);

        /// <summary>
        ///     Gets a list of pages with specified tag
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
        [PublicAPI, NotNull]
        IReadOnlyList<IPage> GetPagesByTag([NotNull] IDocumentation documentation, [NotNull] string tag);
    }
}