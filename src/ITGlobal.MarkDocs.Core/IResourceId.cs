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
        [NotNull]
        string Id { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        ResourceType Type { get; }
    }
}