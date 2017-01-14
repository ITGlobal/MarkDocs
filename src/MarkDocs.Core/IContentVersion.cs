using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Content version information
    /// </summary>
    [PublicAPI]
    public interface IContentVersion
    {
        /// <summary>
        ///     Source URL
        /// </summary>
        [PublicAPI, NotNull]
        string SourceUrl { get; }

        /// <summary>
        ///     Version name (e.g. tag name, branch name or any other identifier)
        /// </summary>
        [PublicAPI, CanBeNull]
        string Version { get; }
        
        /// <summary>
        ///     Last content change time
        /// </summary>
        [PublicAPI]
        DateTime LastChangeTime { get; }

        /// <summary>
        ///     Last content change identifier (e.g. commit hash)
        /// </summary>
        [PublicAPI, CanBeNull]
        string LastChangeId { get; }

        /// <summary>
        ///     Last content change description (e.g. commit message)
        /// </summary>
        [PublicAPI, CanBeNull]
        string LastChangeDescription { get; }

        /// <summary>
        ///     Last content change author (e.g. committer)
        /// </summary>
        [PublicAPI, CanBeNull]
        string LastChangeAuthor { get; }
    }
}