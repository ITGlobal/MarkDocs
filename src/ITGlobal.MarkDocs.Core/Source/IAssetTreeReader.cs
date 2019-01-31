using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    [PublicAPI]
    public interface IAssetTreeReader
    {
        [NotNull]
        AssetTree ReadAssetTree(
            [NotNull] ISourceTreeProvider sourceTreeProvider,
            [NotNull] ISourceTreeRoot sourceTreeRoot,
            [NotNull] ICompilationReportBuilder report
        );
    }
}