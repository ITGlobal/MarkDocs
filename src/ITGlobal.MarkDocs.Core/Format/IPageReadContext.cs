using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IFormat.Read"/>
    /// </summary>
    [PublicAPI]
    public interface IPageReadContext
    {
        /// <summary>
        ///     Gets current page resource
        /// </summary>
        [NotNull]
        IResourceId Page { get; }

        /// <summary>
        ///     Returns true if current page has any nested pages
        /// </summary>
        bool IsBranchPage { get; }

        /// <summary>
        ///     Resolves an actual URL for specified resource
        /// </summary>
        bool TryResolvePageResource([NotNull] string url, out string pageId, out string pageUrl);

        /// <summary>
        ///     Resolves an actual URL for specified resource
        /// </summary>
        bool TryResolveFileResource([NotNull] string url, out string fileId, out string fileUrl);

        /// <summary>
        ///     Adds a generated attachment
        /// </summary>
        void CreateAttachment(
            [NotNull] byte[] source,
            [NotNull] IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url
        );

        /// <summary>
        ///     Adds a generated attachment
        /// </summary>
        void CreateAttachment(
            [NotNull] string source,
            [NotNull] IGeneratedAssetContent content,
            out GeneratedFileAsset asset,
            out string url
        );

        /// <summary>
        ///     Adds a warning to compilation report
        /// </summary>
        void Warning(string message, int? lineNumber = null);

        /// <summary>
        ///     Adds an error to compilation report
        /// </summary>
        void Error(string message, int? lineNumber = null);
    }
}