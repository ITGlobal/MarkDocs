using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ITGlobal.MarkDocs.Storage;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Git
{
    /// <summary>
    ///     Git-based storage
    /// </summary>
    internal sealed class GitStorage : IStorage, IDisposable
    {
        #region fields

        private readonly ManualResetEventSlim _initialized = new ManualResetEventSlim();
        private readonly object _updateLock = new object();

        private readonly ILogger _log;
        private readonly GitStorageOptions _options;
        private readonly GitHelper _git;

        private readonly object _stateLock = new object();
        private GitStorageState _state;

        private Timer _pollTimer;
        private readonly SemaphoreSlim _pollMutex = new SemaphoreSlim(1, 1);
        private const int POLL_TIMER_WAIT = 1000;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public GitStorage(ILoggerFactory loggerFactory, GitStorageOptions options)
        {
            _log = loggerFactory.CreateLogger<GitStorage>();
            _options = options;

            _git = new GitHelper(_log);
        }

        #endregion

        #region IStorage

        /// <summary>
        ///     Glob patterns to ignore (e.g. ".git" directory)
        /// </summary>
        public string[] IgnorePatterns { get; } = { ".git/**/*" };

        /// <summary>
        ///     This event is raised when documentation source is changed.
        ///     This event is raised only if storage supports change tracking.
        /// </summary>
        public event EventHandler<StorageChangedEventArgs> Changed;

        private void OnChanged(string documentationId = null)
            => Changed?.Invoke(this, new StorageChangedEventArgs(documentationId));

        /// <summary>
        ///     Initializes storage provider
        /// </summary>
        public void Initialize()
        {
            if (!_git.IsGitInstalled)
            {
                throw new Exception("git executable is not found in PATH");
            }

            Refresh();

            if (_options.EnablePolling)
            {
                _pollTimer = new Timer(PollTimerCallback, null, TimeSpan.Zero, _options.PollingInterval);
            }

            _initialized.Set();
        }

        /// <summary>
        ///     Gets a list of available documentations
        /// </summary>
        public IContentDirectory[] GetContentDirectories()
        {
            _initialized.Wait();

            lock (_stateLock)
            {
                return _state.ContentDirectories;
            }
        }

        /// <summary>
        ///     Gets path to source directory for the specified documentation
        /// </summary>
        public IContentDirectory GetContentDirectory(string documentationId)
        {
            _initialized.Wait();

            lock (_stateLock)
            {
                IContentDirectory contentDirectory;
                if (!_state.ContentDirectoriesById.TryGetValue(documentationId, out contentDirectory))
                {
                    throw new ArgumentOutOfRangeException(nameof(documentationId),
                        $"No such documentation branch - '{documentationId}'");
                }

                return contentDirectory;
            }
        }

        /// <summary>
        ///     Refreshes source files for specified documentation from a remote source if supported.
        /// </summary>
        public void Refresh(string documentationId)
        {
            _initialized.Wait();

            lock (_updateLock)
            {
                // Load current content descriptor
                var contentDescriptor = ContentDescriptor.LoadOrCreate(_log, _options.Directory);

                var workingCopies = contentDescriptor.Items.Select(pair => new WorkingCopy(
                    _log,
                    _git,
                    _options,
                    pair.Value.Path,
                    pair.Key
                )).ToDictionary(_ => _.BranchOrTag, StringComparer.OrdinalIgnoreCase);

                WorkingCopy workingCopy;
                if (!workingCopies.TryGetValue(documentationId, out workingCopy))
                {
                    throw new ArgumentOutOfRangeException(nameof(documentationId),
                        $"No such documentation branch - '{documentationId}'");
                }

                // Refresh only one documentation
                workingCopy.Refresh();

                // Update content state
                UpdateState(workingCopies.Values, contentDescriptor);

                _log.LogInformation("Content of \"{0}\" documentation branch is up to date", documentationId);
            }
        }

        /// <summary>
        ///     Refreshes source files for all documentation from a remote source if supported.
        ///     This method might also fetch some new documentations
        /// </summary>
        public void RefreshAll()
        {
            _initialized.Wait();
            Refresh();
        }

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

        private void Refresh()
        {
            lock (_updateLock)
            {
                // Create root directory if not exists
                if (!Directory.Exists(_options.Directory))
                {
                    Directory.CreateDirectory(_options.Directory);
                }

                // Load current content descriptor
                var contentDescriptor = ContentDescriptor.LoadOrCreate(_log, _options.Directory);

                // List and filter remotes
                var remotes = FetchRemotes().ToArray();

                // Actualize content descriptor
                List<WorkingCopy> workingCopies;
                CategorizeWorkingCopies(remotes, ref contentDescriptor, out workingCopies);

                // Checkout every branch/tag
                for (var i = 0; i < workingCopies.Count; i++)
                {
                    var workingCopy = workingCopies[i];

                    _log.LogDebug("Refreshing working copy \"{0}\" ({1} out of {2})",
                        workingCopy.BranchOrTag,
                        i + 1,
                        workingCopies.Count
                    );
                    workingCopy.Refresh();
                }

                // Delete all other directories
                var knownDirectoryNames = new HashSet<string>(
                    contentDescriptor.Items.Select(_ => _.Value.Path),
                    StringComparer.Ordinal
                );
                var unknownDirectories = Directory.EnumerateDirectories(_options.Directory).Where(path =>
                            !knownDirectoryNames.Contains(Path.GetFileName(path))
                );
                foreach (var path in unknownDirectories)
                {
                    try
                    {
                        _log.LogDebug("Deleting directory \"{0}\"", path);
                        Directory.Delete(path, true);
                    }
                    catch (Exception e)
                    {
                        _log.LogError(0, e, "Unable to delete directory \"{0}\"", path);
                    }
                }

                // Update content state
                UpdateState(workingCopies, contentDescriptor);

                _log.LogInformation("Documentation content is up to date");
            }
        }

        private IEnumerable<RemoteInfo> FetchRemotes()
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

        private void CategorizeWorkingCopies(
            RemoteInfo[] remotes,
            ref ContentDescriptor contentDescriptor,
            out List<WorkingCopy> workingCopies)
        {
            var oldContentDescriptor = contentDescriptor;
            var newContentDescriptor = new ContentDescriptor();
            workingCopies = new List<WorkingCopy>();

            foreach (var remote in remotes.Select(_ => _.Name))
            {
                ContentDescriptorItem item;
                if (!oldContentDescriptor.Items.TryGetValue(remote, out item))
                {
                    item = new ContentDescriptorItem { Path = Guid.NewGuid().ToString("N") };
                }
                else
                {
                    oldContentDescriptor.Items.Remove(remote);
                }

                newContentDescriptor.Items[remote] = item;
                workingCopies.Add(new WorkingCopy(_log, _git, _options, item.Path, remote));
            }

            contentDescriptor = newContentDescriptor;
        }

        private void UpdateState(IEnumerable<WorkingCopy> workingCopies, ContentDescriptor contentDescriptor)
        {
            var directories = workingCopies.Select(_ => _.CreateContentDirectory()).ToList();
            contentDescriptor.Save(_log, _options.Directory);

            foreach (var directory in directories)
            {
                _log.LogDebug("Branch {0} updated to {1} ({2})", directory.Id, directory.ContentVersion.LastChangeId, directory.ContentVersion.Version);
            }

            lock (_stateLock)
            {
                _state = new GitStorageState(directories);
            }
        }

        private void PollTimerCallback(object _)
        {
            if (!_pollMutex.Wait(POLL_TIMER_WAIT))
            {
                return;
            }

            try
            {
                if (!CheckForUpdates())
                {
                    return;
                }

                _log.LogInformation("New content detected");

                Refresh();

                OnChanged();
            }
            catch (Exception e)
            {
                _log.LogError(0, e, "Failed to run polling");
            }
            finally
            {
                _pollMutex.Release();
            }
        }

        private bool CheckForUpdates()
        {
            var remoteBranches = new HashSet<string>(FetchRemotes().Select(_ => $"{_.Name}::{_.Hash}"));
            HashSet<string> localBranches;
            lock (_stateLock)
            {
                localBranches = new HashSet<string>(_state.ContentDirectories.Select(_ => $"{_.Id}::{_.ContentVersion.LastChangeId}"));
            }

            var localBranchesToPrune = localBranches.Except(remoteBranches).ToArray();
            var remoteBranchesToCheckout = remoteBranches.Except(localBranches).ToArray();

            return localBranchesToPrune.Length > 0 || remoteBranchesToCheckout.Length > 0;
        }

        #endregion
    }
}