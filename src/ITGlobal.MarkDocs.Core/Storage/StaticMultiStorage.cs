using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Static subdirectory storage
    /// </summary>
    internal sealed class StaticMultiStorage : IStorage
    {
        #region consts

        private const int WATCH_PERIOD = 1000;
        
        #endregion

        #region fields

        private readonly StaticStorageOptions _options;

        private readonly object _contentDirectoryLock = new object();
        private readonly Dictionary<string, IContentDirectory> _contentDirectories 
            = new Dictionary<string, IContentDirectory>(StringComparer.OrdinalIgnoreCase);

        private readonly Timer _watchTimer;
        private readonly FileSystemWatcher _watcher;
        
        private int _state = 0;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public StaticMultiStorage([NotNull] StaticStorageOptions options)
        {
            _options = options;
            RefreshAll();

            if (options.EnableWatch)
            {
                _watchTimer = new Timer(TimerCallback, null, -1, -1);
                _watcher = new FileSystemWatcher(options.Directory);
                _watcher.Changed += FileSystemChangeHandler;
                _watcher.Created += FileSystemChangeHandler;
                _watcher.Deleted += FileSystemChangeHandler;
                _watcher.Renamed += FileSystemChangeHandler;
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = false;
            }
        }

        #endregion

        #region IStorage

        /// <summary>
        ///     Glob patterns to ignore (e.g. ".git" directory)
        /// </summary>
        public string[] IgnorePatterns => null;

        /// <summary>
        ///     This event is raised when documentation source is changed.
        ///     This event is raised only if storage supports change tracking.
        /// </summary>
        public event EventHandler<StorageChangedEventArgs> Changed;
        private void NotifyChanged(string documentationId = null) => Changed?.Invoke(this, new StorageChangedEventArgs(documentationId));

        /// <summary>
        ///     Initializes storage provider
        /// </summary>
        public void Initialize() { }

        /// <summary>
        ///     Enables <see cref="IStorage.Changed"/> event
        /// </summary>
        public void EnableChangeNotifications()
        {
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///     Gets a list of available documentations
        /// </summary>
        public IContentDirectory[] GetContentDirectories()
        {
            lock (_contentDirectoryLock)
            {
                return _contentDirectories.Values.ToArray();
            }
        }

        /// <summary>
        ///     Gets path to source directory for the specified documentation
        /// </summary>
        public IContentDirectory GetContentDirectory(string documentationId)
        {
            lock (_contentDirectoryLock)
            {
                IContentDirectory contentDirectory;
                if (!_contentDirectories.TryGetValue(documentationId, out contentDirectory))
                {
                    throw new ArgumentOutOfRangeException(nameof(documentationId), $"No such documentation branch - '{documentationId}'");
                }

                return contentDirectory;
            }
        }

        /// <summary>
        ///     Refreshes source files for specified documentation from a remote source if supported.
        /// </summary>
        public void Refresh(string documentationId)
        {
            lock (_contentDirectoryLock)
            {
                if (!_contentDirectories.ContainsKey(documentationId))
                {
                    throw new ArgumentOutOfRangeException(nameof(documentationId), $"No such documentation branch - '{documentationId}'");
                }
                
                RefreshAll();
            }
        }

        /// <summary>
        ///     Refreshes source files for all documentation from a remote source if supported.
        ///     This method might also fetch some new documentations
        /// </summary>
        public void RefreshAll()
        {
            lock (_contentDirectoryLock)
            {
                _contentDirectories.Clear();
                foreach (var directory in Directory.EnumerateDirectories(_options.Directory))
                {
                    if (Directory.EnumerateFileSystemEntries(directory).Any())
                    {
                        var contentDirectory = new StaticContentDirectory(Path.GetFileName(directory), directory);
                        _contentDirectories.Add(contentDirectory.Id, contentDirectory);
                    }
                }
            }
        }

        /// <summary>
        ///     Retreives a value for <see cref="Metadata.ContentId"/> from a file path
        /// </summary>
        public string GetContentId(string rootDirectory, string path) => FileHasher.ComputeFileHash(path);

        /// <summary>
        ///     Retreives a value for <see cref="Metadata.LastChangedBy"/> from a file path
        /// </summary>
        public string GetLastChangeAuthor(string rootDirectory, string path) => null;

        #endregion

        #region methods

        private void TimerCallback(object state)
        {
            _watchTimer.Change(-1, -1);
            NotifyChanged();

            Interlocked.Exchange(ref _state, 0);
        }

        private void FileSystemChangeHandler(object sender, FileSystemEventArgs e)
        {
            // TODO react on different event differently:
            // /branch/... -> NotifyChanged("branch");
            // /new-branch/... -> NotifyChanged();
            // /filename -> ignore

            if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
            {
                _watchTimer.Change(WATCH_PERIOD, WATCH_PERIOD);
            }
        }

        #endregion
    }
}