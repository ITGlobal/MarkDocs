using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Provides one or more source directory trees
    /// </summary>
    [PublicAPI]
    public interface ISourceTreeProvider
    {
        /// <summary>
        ///     Glob patterns to ignore (e.g. ".git" directory)
        /// </summary>
        [CanBeNull]
        string[] IgnorePatterns { get; }
        
        /// <summary>
        ///     This event is raised when documentation source is changed.
        ///     This event is raised only if storage supports change tracking.
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        ///     Gets a list of all source trees available currently
        /// </summary>
        [NotNull]
        ISourceTree[] GetSourceTrees();

        /// <summary>
        ///     Gets a source tree by its ID
        /// </summary>
        [CanBeNull]
        ISourceTree GetSourceTree([NotNull] string id);

        /// <summary>
        ///     Refreshes all source trees
        /// </summary>
        void Refresh();
    }
}