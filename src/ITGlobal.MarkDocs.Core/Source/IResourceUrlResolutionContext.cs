using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     A context for <see cref="IResourceUrlResolver"/>
    /// </summary>
    [PublicAPI]
    public interface IResourceUrlResolutionContext
    {
        /// <summary>
        ///     Source tree ID
        /// </summary>
        [NotNull]
        string SourceTreeId { get; }

        /// <summary>
        ///    A current page reference
        /// </summary>
        [NotNull]
        IResourceId Page { get; }

        /// <summary>
        ///     True if current page is a branch page
        /// </summary>
        bool IsBranchPage { get; }
    }
}