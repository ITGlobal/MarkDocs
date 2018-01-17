using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Markdocs blog engine
    /// </summary>
    public interface IBlogEngine : IDisposable
    {
        /// <summary>
        ///     Gets an underlying MarkDocs service
        /// </summary>
        [PublicAPI]
        IMarkDocService MarkDocs { get; }

        /// <summary>
        ///     Initializes blog data (foreground)
        /// </summary>
        [PublicAPI]
        void Initialize();

        /// <summary>
        ///     Blog version
        /// </summary>
        [PublicAPI, NotNull]
        IContentVersion ContentVersion { get; }

        /// <summary>
        ///     Provides errors and warning for blog
        /// </summary>
        [PublicAPI]
        ICompilationReport CompilationReport { get; }

        /// <summary>
        ///     Blog post index
        /// </summary>
        [PublicAPI, NotNull]
        IBlogIndex Index { get; }

        /// <summary>
        ///     Gets a blog resource by its ID
        /// </summary>
        /// <param name="id">
        ///     Blog resource URL or permalink
        /// </param>
        /// <returns>
        ///     A blog resource or null if resource doesn't exist
        /// </returns>
        [PublicAPI, CanBeNull]
        IBlogResource GetResource([NotNull] string id);

        /// <summary>
        ///     Search blog posts by text
        /// </summary>
        [PublicAPI, NotNull]
        ITextSearchResult Search([NotNull] string query);
        
        /// <summary>
        ///     Gets a list of search query suggestions
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     List of search suggestions
        /// </returns>
        [PublicAPI, NotNull]
        IReadOnlyList<string> Suggest([NotNull] string query);
    }
}
