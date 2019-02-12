using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     An asset tree reader service
    /// </summary>
    [PublicAPI]
    public interface IAssetTreeReader
    {
        /// <summary>
        ///     Read an asset tree from a directory
        /// </summary>
        [NotNull]
        AssetTree ReadAssetTree(
            [NotNull] ISourceTreeProvider sourceTreeProvider,
            [NotNull] ISourceTreeRoot sourceTreeRoot,
            [NotNull] ICompilationReportBuilder report
        );
    }
}