using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A generic resource identifier
    /// </summary>
    [PublicAPI]
    public interface IResourceId
    { 
        /// <summary>
        ///     Resource ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }
    }
}