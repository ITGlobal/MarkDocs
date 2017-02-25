using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A generic resource
    /// </summary>
    [PublicAPI]
    public interface IResource
    {
        /// <summary>
        ///     Resource ID
        /// </summary>
        [PublicAPI, NotNull]
        string Id { get; }

        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        [PublicAPI, NotNull]
        IDocumentation Documentation { get; }

        /// <summary>
        ///     Name of resource file with extension (only name, not a full path)
        /// </summary>
        [PublicAPI, NotNull]
        string FileName { get; }

        /// <summary>
        ///     Resource type
        /// </summary>
        [PublicAPI]
        ResourceType Type { get; }
    }
}