using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Content version information
    /// </summary>
    [PublicAPI]
    public interface ISourceInfo
    {
        /// <summary>
        ///     Source URL
        /// </summary>
        [NotNull]
        string SourceUrl { get; }

        /// <summary>
        ///     Version name (e.g. tag name, branch name or any other identifier)
        /// </summary>
        [CanBeNull]
        string Version { get; }
        
        /// <summary>
        ///     Last content change time
        /// </summary>
        [CanBeNull]
        DateTime? LastChangeTime { get; }

        /// <summary>
        ///     Last content change identifier (e.g. commit hash)
        /// </summary>
        [CanBeNull]
        string LastChangeId { get; }

        /// <summary>
        ///     Last content change description (e.g. commit message)
        /// </summary>
        [CanBeNull]
        string LastChangeDescription { get; }

        /// <summary>
        ///     Last content change author (e.g. committer)
        /// </summary>
        [CanBeNull]
        string LastChangeAuthor { get; }
    }
}