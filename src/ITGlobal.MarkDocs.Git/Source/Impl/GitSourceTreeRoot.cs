using System;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class GitSourceTreeRoot : ISourceTreeRoot
    {
        private readonly IMarkDocsLog _log;
        private readonly GitHelper _git;

        public GitSourceTreeRoot(IMarkDocsLog log, GitHelper git, string rootDirectory, ISourceTree sourceTree, ISourceInfo sourceInfo)
        {
            _log = log;
            _git = git;
            SourceTree = sourceTree;
            RootDirectory = rootDirectory;
            SourceInfo = sourceInfo;
        }

        public ISourceTree SourceTree { get; }
        public string RootDirectory { get; }
        public ISourceInfo SourceInfo { get; }

        public string GetContentId(string path)
        {
            try
            {
                if (!_git.FindOriginalFileInfo(RootDirectory, path, out var commit, out var name))
                {
                    return null;
                }

                var hash = _git.GetBlobHash(RootDirectory, commit, name);
                return hash;
            }
            catch (Exception e)
            {
                _log.Error(e, $"Failed to retrieve content ID for \"{path}\"");
                return null;
            }
        }

        public string GetLastChangeAuthor(string path)
        {
            try
            {
                var author = _git.GetLastChangeAutor(RootDirectory, path);

                return author;
            }
            catch (Exception e)
            {
                _log.Error(e, $"Failed to retrieve last change author for \"{path}\"");
                return null;
            }
        }
    }
}
