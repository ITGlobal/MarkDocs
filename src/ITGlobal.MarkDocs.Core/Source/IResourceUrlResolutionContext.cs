using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    [PublicAPI]
    public interface IResourceUrlResolutionContext
    {
        [NotNull]
        string SourceTreeId { get; }

        [NotNull]
        IResourceId Page { get; }

        bool IsBranchPage { get; }
    }
}