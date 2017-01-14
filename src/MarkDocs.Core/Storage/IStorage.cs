﻿using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///    Provides access to documentation source files
    /// </summary>
    [PublicAPI]
    public interface IStorage
    {
        /// <summary>
        ///     Glob patterns to ignore (e.g. ".git" directory)
        /// </summary>
        [PublicAPI, CanBeNull]
        string[] IgnorePatterns { get; }

        /// <summary>
        ///     This event is raised when documentation source is changed.
        ///     This event is raised only if storage supports change tracking.
        /// </summary>
        [PublicAPI]
        event EventHandler<StorageChangedEventArgs> Changed;

        /// <summary>
        ///     Initializes storage provider
        /// </summary>
        [PublicAPI]
        void Initialize();

        /// <summary>
        ///     Gets a list of available documentations
        /// </summary>
        [PublicAPI, NotNull]
        IContentDirectory[] GetContentDirectories();

        /// <summary>
        ///     Gets path to source directory for the specified documentation
        /// </summary>
        [NotNull, PublicAPI]
        IContentDirectory GetContentDirectory([NotNull] string documentationId);

        /// <summary>
        ///     Refreshes source files for specified documentation from a remote source if supported.
        /// </summary>
        [PublicAPI]
        void Refresh([NotNull] string documentationId);

        /// <summary>
        ///     Refreshes source files for all documentation from a remote source if supported.
        ///     This method might also fetch some new documentations
        /// </summary>
        [PublicAPI]
        void RefreshAll();
    }
}
