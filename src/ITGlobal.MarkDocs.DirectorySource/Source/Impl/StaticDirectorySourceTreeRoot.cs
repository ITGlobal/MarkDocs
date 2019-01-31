namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class StaticDirectorySourceTreeRoot : ISourceTreeRoot
    {
        public StaticDirectorySourceTreeRoot(ISourceTree sourceTree, string rootDirectory)
        {
            SourceTree = sourceTree;
            RootDirectory = rootDirectory;
            SourceInfo = new StaticDirectorySourceInfo(rootDirectory);
        }

        public ISourceTree SourceTree { get; }

        public string RootDirectory { get; }

        public ISourceInfo SourceInfo { get; }

        public string GetContentId(string path) => null;

        public string GetLastChangeAuthor(string path) => null;
    }
}