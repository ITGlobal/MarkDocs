using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
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
        string ResolveUrl(
            [NotNull] IResourceUrlResolutionContext context,
            [NotNull] IResourceId resource
        );
    }
}