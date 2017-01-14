using System.IO;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Cache item content
    /// </summary>
    [PublicAPI]
    public interface ICacheItemContent
    {
        /// <summary>
        ///     Gets a content
        /// </summary>
        [PublicAPI, NotNull]
        Stream GetContent();
    }
}