using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Resolves actual resource URLs according to web application routing
    /// </summary>
    [PublicAPI]
    public interface IResourceUrlResolver
    {
        /// <summary>
        ///     Resolves an actual URL for specified resource
        /// </summary>
        [PublicAPI, NotNull]
        string ResolveUrl([NotNull] IPage page, [NotNull] string resourceId);
    }
}