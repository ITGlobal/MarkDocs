using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A documentation
    /// </summary>
    [PublicAPI]
    public interface IDocumentation
    {
        /// <summary>
        ///     Documentation service
        /// </summary>
        [NotNull]
        IMarkDocService Service { get; }

        /// <summary>
        ///     Documentation ID
        /// </summary>
        [NotNull]
        string Id { get; }

        /// <summary>
        ///     Documentation title
        /// </summary>
        [NotNull]
        string Title { get; }

        /// <summary>
        ///     Documentation version
        /// </summary>
        [NotNull]
        ISourceInfo SourceInfo { get; }

        /// <summary>
        ///     Root page
        /// </summary>
        [NotNull]
        IPage RootPage { get; }

        /// <summary>
        ///     All pages
        /// </summary>
        [NotNull]
        ImmutableDictionary<string, IPage> Pages { get; }

        /// <summary>
        ///     Provides errors and warning for documentation
        /// </summary>
        [NotNull]
        ICompilationReport CompilationReport { get; }

        /// <summary>
        ///     Documentation attached files
        /// </summary>
        [NotNull]
        ImmutableDictionary<string, IFileResource> Files { get; }

        /// <summary>
        ///     Gets a documentation page by its ID
        /// </summary>
        /// <param name="id">
        ///     Page ID
        /// </param>
        /// <returns>
        ///     A documentation page or null if page doesn't exist
        /// </returns>
        [CanBeNull]
        IPage GetPage([NotNull] string id);

        /// <summary>
        ///     Gets an attachment by its ID
        /// </summary>
        /// <param name="id">
        ///     FileResource path
        /// </param>
        /// <returns>
        ///     An attachment or null if it doesn't exist
        /// </returns>
        [CanBeNull]
        IFileResource GetAttachment([NotNull] string id);
    }
}