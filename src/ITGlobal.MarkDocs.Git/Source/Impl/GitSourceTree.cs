using System;
using System.IO;
using System.Text;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class GitSourceTree : ISourceTree
    {

        private readonly GitSourceTreeProvider _provider;
        private readonly IMarkDocsLog _log;
        private readonly IAssetTreeReader _reader;
        private readonly GitHelper _git;
        private readonly GitSourceTreeOptions _options;

        private readonly object _sourceInfoLock = new object();
        private GitSourceInfo _sourceInfo;

        public GitSourceTree(
            GitSourceTreeProvider provider,
            IMarkDocsLog log,
            IAssetTreeReader reader,
            GitHelper git,
            GitSourceTreeOptions options,
            string id,
            string branchOrTagName,
            string directoryPath
        )
        {
            _provider = provider;
            _log = log;
            _reader = reader;
            _git = git;
            _options = options;

            Id = id;
            BranchOrTagName = branchOrTagName;
            DirectoryPath = directoryPath;
            DirectoryName = Path.GetFileName(directoryPath);
        }

        public string Id { get; }
        public string BranchOrTagName { get; }
        public string DirectoryName { get; }
        public string DirectoryPath { get; }

        public event EventHandler Changed;
        private void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public AssetTree ReadAssetTree(ICompilationReportBuilder report)
        {
            GitSourceInfo sourceInfo;
            lock (_sourceInfoLock)
            {
                sourceInfo = _sourceInfo;
            }

            var treeRoot = new GitSourceTreeRoot(_log, _git, DirectoryPath, this, sourceInfo);
            return _reader.ReadAssetTree(_provider, treeRoot, report);
        }

        public void Refresh()
        {
            UpdateWorkingCopy();

            var sourceInfo = GitSourceInfo.FromWorkingCopy(_git, DirectoryPath);
            lock (_sourceInfoLock)
            {
                _sourceInfo = sourceInfo;
            }

            _log.Info($"Working copy {BranchOrTagName} is now up to date ({sourceInfo.LastChangeId})");
        }

        public bool Refresh(string currentHash)
        {
            GitSourceInfo sourceInfo;
            lock (_sourceInfoLock)
            {
                sourceInfo = _sourceInfo;
            }

            if (sourceInfo.LastChangeId == currentHash)
            {
                return false;
            }

            Refresh();
            return true;
        }

        public void Initialize()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                CheckoutNewWorkingCopy();
            }
            else if (!_git.IsRepository(DirectoryPath))
            {
                _log.Warning(
                    $"Directory \"{DirectoryPath}\" already exists but it's not a working copy. It'll be recreated."
                );
                Directory.Delete(DirectoryPath, true);

                CheckoutNewWorkingCopy();
            }
            else
            {
                if (ShouldPerformCleanCheckout())
                {
                    _log.Warning($"Working copy \"{DirectoryPath}\" is stale and will be recreated.");
                    Directory.Delete(DirectoryPath, true);

                    CheckoutNewWorkingCopy();
                }
                else
                {
                    UpdateWorkingCopy();
                }
            }

            var sourceInfo = GitSourceInfo.FromWorkingCopy(_git, DirectoryPath);
            lock (_sourceInfoLock)
            {
                _sourceInfo = sourceInfo;
            }

            _log.Info($"Working copy {BranchOrTagName} is up to date");
        }

        public static string NormalizeId(string id)
        {
            var builder = new StringBuilder();

            foreach (var c in id)
            {
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
                else if (char.IsPunctuation(c) || char.IsWhiteSpace(c))
                {
                    if (builder.Length > 0 && !char.IsPunctuation(builder[builder.Length - 1]))
                    {
                        builder.Append('-');
                    }
                }
            }

            while (builder.Length > 0 && char.IsPunctuation(builder[builder.Length - 1]))
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }

        private void CheckoutNewWorkingCopy()
        {
            _log.Info($"Checking out new working copy ({BranchOrTagName}) into \"{DirectoryPath}\"");
            _git.Clone(_options.Url, DirectoryPath, BranchOrTagName);
        }

        private bool ShouldPerformCleanCheckout()
        {
            var remoteUrl = _git.GetRemoteUrl(DirectoryPath);
            if (remoteUrl != _options.Url)
            {
                _log.Debug($"Working copy \"{DirectoryPath}\" doesn't match specified origin URL");
                return true;
            }

            return false;
        }

        private void UpdateWorkingCopy()
        {
            _log.Info($"Updating working copy \"{DirectoryPath}\" from {BranchOrTagName}");
            _git.Reset(DirectoryPath);
            _git.Clean(DirectoryPath);
            _git.Checkout(DirectoryPath, BranchOrTagName);
            _git.Pull(DirectoryPath);
        }

    }
}