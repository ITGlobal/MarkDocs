using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///     Post tag with count
    /// </summary>
    [PublicAPI]
    public interface ITag : IBlogPostPagedList
    {

        /// <summary>
        ///     Tag name
        /// </summary>
        [NotNull]
        string Name { get; }

    }
}