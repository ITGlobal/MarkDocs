using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal interface IShallowPageAssetReader
    {
        IMarkDocsLog Log { get; }
        IFormat Format { get; }
        ISourceTreeRoot Root { get; }
        ICompilationReportBuilder Report { get; }

        IResourceUrlResolver ResourceUrlResolver { get; }
        ShallowPageAsset ResolvePageAsset(string path);
        FileAsset ResolveFileAsset(string path);
        PageMetadata GetMetadata(string filename, bool isIndexFile);
        void RegisterAsset(GeneratedFileAsset asset);
    }
}