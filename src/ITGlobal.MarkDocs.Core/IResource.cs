using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     A generic resource
    /// </summary>
    [PublicAPI]
    public interface IResource : IResourceId
    {
        /// <summary>
        ///     A reference to a documentation
        /// </summary>
        [NotNull]
        IDocumentation Documentation { get; }

        /// <summary>
        ///     Name of resource file with extension
        /// </summary>
        [NotNull]
        string RelativePath { get; }
    }
}