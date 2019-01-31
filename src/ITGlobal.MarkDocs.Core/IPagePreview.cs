using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A documentation page preview
    /// </summary>
    [PublicAPI]
    public interface IPagePreview : IResource
    {
        /// <summary>
        ///     Parent page
        /// </summary>
        [NotNull]
        IPage Page { get; }

        /// <summary>
        ///     Opens a read-only content stream
        /// </summary>
        [NotNull]
        Stream OpenRead();
    }
}