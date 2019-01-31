using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     A content source for generated assets
    /// </summary>
    [PublicAPI]
    public interface IGeneratedAssetContent
    {
        /// <summary>
        ///     Asset content MIME type
        /// </summary>
        [NotNull]
        string ContentType { get; }

        /// <summary>
        ///     Formats content file name
        /// </summary>
        [NotNull]
        string FormatFileName([NotNull] string name);

        /// <summary>
        ///     Writes generated content 
        /// </summary>
        void Write([NotNull] Stream stream);
    }
}