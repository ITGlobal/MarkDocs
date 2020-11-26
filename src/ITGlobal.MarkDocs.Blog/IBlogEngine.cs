using System;
using System.Collections.Generic;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     MarkDocs-powered blog engine
    /// </summary>
    [PublicAPI]
    public interface IBlogEngine : IDisposable
    {
        /// <summary>
        ///     Blog version
        /// </summary>
        [NotNull]
        ISourceInfo SourceInfo { get; }

        /// <summary>
        ///     Internal state version
        /// </summary>
        long StateVersion { get; }

        /// <summary>
        ///     Provides errors and warning for blog
        /// </summary>
        [NotNull]
        ICompilationReport CompilationReport { get; }

        /// <summary>
        ///     Blog post index
        /// </summary>
        [NotNull]
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
        [CanBeNull]
        IBlogResource GetResource([NotNull] string id);

        /// <summary>
        ///     Search blog posts by text
        /// </summary>
        [NotNull]
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
        [NotNull]
        IReadOnlyList<string> Suggest([NotNull] string query);

        /// <summary>
        ///     Updates blog contents
        /// </summary>
        void Synchronize();
    }
}
