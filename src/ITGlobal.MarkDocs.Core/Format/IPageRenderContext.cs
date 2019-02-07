using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;
using System;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IPageContent.Render"/>
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
        ///     Writes a generated asset
        /// </summary>
        CreateAttachmentResult Store([NotNull] GeneratedFileAsset asset);
        
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
