using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IParsedPage.Render"/>
    /// </summary>
    [PublicAPI]
    public interface IPageRenderContext
    {
        /// <summary>
        ///    A page reference
        /// </summary>
        [NotNull]
        PageAsset Page { get; }

        /// <summary>
        ///    A whole asset tree reference
        /// </summary>
        [NotNull]
        AssetTree AssetTree { get; }

        /// <summary>
        ///     Add a generated attachment
        /// </summary>
        CreateAttachmentResult CreateAttachment([NotNull] byte[] source, [NotNull] IGeneratedAssetContent content);
        
        /// <summary>
        ///     Add a generated attachment
        /// </summary>
        CreateAttachmentResult CreateAttachment([NotNull] string source, [NotNull] IGeneratedAssetContent content);
        
        /// <summary>
        ///     Add a warning to compilation report
        /// </summary>
        void Warning([NotNull] string message, int? lineNumber = null, Exception exception = null);

        /// <summary>
        ///     Add an error to compilation report
        /// </summary>
        void Error([NotNull] string message, int? lineNumber = null);
    
        /// <summary>
        ///     Add an error to compilation report
        /// </summary>
        void Error([NotNull] string message, Exception exception);
    }
}
