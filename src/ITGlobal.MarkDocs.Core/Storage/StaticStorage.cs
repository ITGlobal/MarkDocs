using System;
using System.IO;
using System.Threading;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Static directory storage
    /// </summary>
    internal sealed class StaticStorage : IStorage
    {
        #region consts

        private const string MASTER = "master";
        private const int WATCH_PERIOD = 1000;

        #endregion

        #region fields
        
        private readonly StaticStorageOptions _options;

        private readonly object _contentDirectoryLock = new object();
        private StaticContentDirectory _contentDirectory;

        private readonly Timer _watchTimer;
        private readonly FileSystemWatcher _watcher;

        private int _state = 0;

        #endregion

        #region .ctor
        
        /// <summary>
        ///     .ctor
        /// </summary>
        public StaticStorage([NotNull] StaticStorageOptions options)
        {
            _options = options;
            
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

        /// <inheritdoc />
        public event EventHandler<StorageChangedEventArgs> Changed;
        private void NotifyChanged() => Changed?.Invoke(this, new StorageChangedEventArgs());

        /// <summary>
        ///     Initializes storage provider
        /// </summary>
        public void Initialize() => RefreshAll();

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
                return new IContentDirectory[] { _contentDirectory };
            }
        }

        /// <summary>
        ///     Gets path to source directory for the specified documentation
        /// </summary>
        public IContentDirectory GetContentDirectory(string documentationId)
        {
            if (!string.Equals(documentationId, MASTER, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException(nameof(documentationId), $"No such documentation branch - '{documentationId}'");
            }

            lock (_contentDirectoryLock)
            {
                return _contentDirectory;
            }
        }

        /// <summary>
        ///     Refreshes source files for specified documentation from a remote source if supported.
        /// </summary>
        public void Refresh(string documentationId)
        {
            if (!string.Equals(documentationId, MASTER, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentOutOfRangeException(nameof(documentationId), $"No such documentation branch - '{documentationId}'");
            }

            lock (_contentDirectoryLock)
            {
                _contentDirectory = new StaticContentDirectory(MASTER, _options.Directory);
            }
        }

        /// <summary>
        ///     Refreshes source files for all documentation from a remote source if supported.
        ///     This method might also fetch some new documentations
        /// </summary>
        public void RefreshAll()
        {
            Refresh(MASTER);
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
            if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
            {
                _watchTimer.Change(WATCH_PERIOD, WATCH_PERIOD);
            }
        }
        
        #endregion
    }
}