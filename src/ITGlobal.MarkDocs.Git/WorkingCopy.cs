using System.IO;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Git
{
    internal sealed class WorkingCopy
    {
        #region fields

        private readonly ILogger _log;
        private readonly GitHelper _git;
        private readonly GitStorageOptions _options;

        #endregion

        #region .ctor

        public WorkingCopy(ILogger log, GitHelper git, GitStorageOptions options, string directoryName, string branchOrTag)
        {
            _log = log;
            _git = git;
            _options = options;
            Path = System.IO.Path.Combine(options.Directory, directoryName);
            DirectoryName = directoryName;
            BranchOrTag = branchOrTag;
        }

        #endregion

        #region properties

        public string Path { get; }
        public string DirectoryName { get; }
        public string BranchOrTag { get; }
        
        #endregion

        #region methods

        public void Refresh()
        {
            if (!Directory.Exists(Path))
            {
                CheckoutNewWorkingCopy();
            }
            else if (!_git.IsRepository(Path))
            {
                _log.LogWarning("Directory \"{0}\" already exists but it's not a working copy. It'll be recreated.", Path);
                Directory.Delete(Path, true);

                CheckoutNewWorkingCopy();
            }
            else
            {
                UpdateWorkingCopy();
            }

            _log.LogInformation("Working copy {0} is ready", BranchOrTag);
        }

        public GitContentDirectory CreateContentDirectory()
        {
            var version = GitContentVersion.FromWorkingCopy(_git, BranchOrTag, Path);
            var directory = new GitContentDirectory(BranchOrTag, Path, version);
            return directory;
        }

        private void CheckoutNewWorkingCopy()
        {
            _log.LogInformation("Checking out new working copy ({0}) into \"{1}\"", BranchOrTag, Path);
            _git.Clone(_options.Url, Path, BranchOrTag);
        }

        private void UpdateWorkingCopy()
        {
            _log.LogInformation("Updating working copy \"{0}\" from {1}", Path, BranchOrTag);
            _git.Reset(Path);
            _git.Clean(Path);
            _git.Checkout(Path, BranchOrTag);
            _git.Pull(Path);
        }
        
        #endregion
    }
}