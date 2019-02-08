using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class GitSourceTreeProvider : ISourceTreeProvider, IDisposable
    {
        #region fields

        private readonly object _updateLock = new object();

        private readonly IMarkDocsLog _log;
        private readonly IAssetTreeReader _reader;
        private readonly GitSourceTreeOptions _options;
        private readonly GitHelper _git;

        private readonly object _stateLock = new object();
        private GitSourceTreeProviderState _state = GitSourceTreeProviderState.Empty;

        private Timer _pollTimer;
        private readonly SemaphoreSlim _pollMutex = new SemaphoreSlim(1, 1);
        private const int POLL_TIMER_WAIT = 1000;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public GitSourceTreeProvider(IMarkDocsLog log, IAssetTreeReader reader, GitSourceTreeOptions options)
        {
            _log = log;
            _reader = reader;
            _options = options;

            _git = new GitHelper(_log);

            if (!_git.IsGitInstalled)
            {
                throw new Exception("git executable is not found in PATH");
            }

            Initialize();

            if (_options.EnablePolling)
            {
                _pollTimer = new Timer(PollTimerCallback, null, _options.PollingInterval, _options.PollingInterval);
            }
        }

        #endregion

        #region ISourceTreeProvider

        public string[] IgnorePatterns { get; } = { ".git/**/*" };

        public event EventHandler Changed;
        private void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public ISourceTree[] GetSourceTrees()
        {
            GitSourceTreeProviderState state;
            lock (_stateLock)
            {
                state = _state;
            }

            return state.List.OfType<ISourceTree>().ToArray();
        }

        public ISourceTree GetSourceTree(string id)
        {
            GitSourceTreeProviderState state;
            lock (_stateLock)
            {
                state = _state;
            }

            state.ById.TryGetValue(id, out var sourceTree);
            return sourceTree;
        }

        void ISourceTreeProvider.Refresh() => Refresh();

        public void Dispose()
        {
            if (_pollTimer != null)
            {
                _pollTimer.Dispose();
                _pollTimer = null;
            }
        }

        #endregion

        #region private methods

        private void Initialize()
        {
            // Create root directory if not exists
            Directory.CreateDirectory(_options.Directory);

            GitSourceTreeProviderState oldState;
            lock (_stateLock)
            {
                oldState = _state;
            }

            // Load current content descriptor
            var contentDescriptor = ContentDescriptor.LoadOrCreate(_options.Directory, _log);

            // Create source trees
            foreach (var (id, item) in contentDescriptor.Items)
            {
                var sourceTree = new GitSourceTree(
                    provider: this,
                    log: _log,
                    reader: _reader,
                    git: _git,
                    options: _options,
                    id: id,
                    branchOrTagName: item.Name,
                    directoryPath: Path.Combine(_options.Directory, item.Path)
                );
                sourceTree.Initialize();
                oldState = oldState.AddOrUpdate(sourceTree);
            }

            lock (_stateLock)
            {
                _state = oldState;
            }

            // Delete all other directories
            CleanOldWorkingCopies(oldState);

            _log.Info("Documentation source is up to date");
        }

        private void Refresh()
        {
            lock (_updateLock)
            {
                // Create root directory if not exists
                Directory.CreateDirectory(_options.Directory);

                GitSourceTreeProviderState oldState;
                lock (_stateLock)
                {
                    oldState = _state;
                }

                // Load current content descriptor
                var contentDescriptor = ContentDescriptor.LoadOrCreate(_options.Directory, _log);

                // List and filter remotes
                var remotes = FetchRemotes().ToArray();

                // Checkout every branch/tag
                var newState = oldState;
                var toBeRemoved = new HashSet<GitSourceTree>(oldState.ByRemoteName.Values);
                var hasAnyChanges = false;
                for (var i = 0; i < remotes.Length; i++)
                {
                    var remote = remotes[i];
                    if (newState.ByRemoteName.TryGetValue(remote.Name, out var sourceTree))
                    {
                        _log.Debug($"Refreshing working copy \"{remote.Name}\" ({i + 1} out of {remotes.Length})");
                        toBeRemoved.Remove(sourceTree);
                        hasAnyChanges |= sourceTree.Refresh(remote.Hash);
                    }
                    else
                    {
                        _log.Debug($"Checking out working copy \"{remote.Name}\" ({i + 1} out of {remotes.Length})");

                        var id = GitSourceTree.NormalizeId(remote.Name);
                        if (!contentDescriptor.Items.TryGetValue(id, out var item))
                        {
                            item = new ContentDescriptorItem
                            {
                                Name = remote.Name,
                                Path = Guid.NewGuid().ToString("N")
                            };
                        }

                        sourceTree = new GitSourceTree(
                            provider: this,
                            log: _log,
                            reader: _reader,
                            git: _git,
                            options: _options,
                            id: id,
                            branchOrTagName: item.Name,
                            directoryPath: Path.Combine(_options.Directory, item.Path)
                        );
                        sourceTree.Initialize();
                        newState = newState.AddOrUpdate(sourceTree);
                        hasAnyChanges = true;
                    }
                }

                foreach (var sourceTree in toBeRemoved)
                {
                    newState = newState.Remove(sourceTree);
                    hasAnyChanges = true;
                }

                if (hasAnyChanges)
                {
                    contentDescriptor.Save(_options.Directory, _log);
                    lock (_stateLock)
                    {
                        _state = newState;
                    }

                    // Delete all other directories
                    CleanOldWorkingCopies(newState);

                    NotifyChanged();
                }

                _log.Info("Documentation source is up to date");
            }

            IEnumerable<RemoteInfo> FetchRemotes()
            {
                foreach (var remoteInfo in _options.Tags.Filter(_git.ListRemoteTags(_options.Url)))
                {
                    yield return remoteInfo;
                }

                foreach (var remoteInfo in _options.Branches.Filter(_git.ListRemoteBranches(_options.Url)))
                {
                    yield return remoteInfo;
                }
            }
        }

        private void PollTimerCallback(object _ = null)
        {
            if (!_pollMutex.Wait(POLL_TIMER_WAIT))
            {
                return;
            }

            try
            {
                Refresh();
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to check for new content");
            }
            finally
            {
                _pollMutex.Release();
            }
        }

        private void CleanOldWorkingCopies(GitSourceTreeProviderState state)
        {
            var knownDirectoryNames = new HashSet<string>(
                state.ByRemoteName.Values.Select(_ => _.DirectoryName),
                StringComparer.Ordinal
            );
            var unknownDirectories = Directory.EnumerateDirectories(_options.Directory)
                .Where(path => !knownDirectoryNames.Contains(Path.GetFileName(path)))
                .ToArray();
            foreach (var path in unknownDirectories)
            {
                try
                {
                    _log.Debug($"Deleting directory \"{path}\"");
                    Directory.Delete(path, true);
                }
                catch (Exception e)
                {
                    _log.Error(e, $"Unable to delete directory \"{path}\"");
                }
            }
        }

        #endregion
    }
}
