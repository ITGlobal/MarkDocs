using System.Collections.Immutable;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs state
    /// </summary>
    [PublicAPI]
    public interface IMarkDocState
    {
        /// <summary>
        ///     List of all documentations
        /// </summary>
        ImmutableArray<IDocumentation> List { get; }

        /// <summary>
        ///     Map of all documentations by theirs ID
        /// </summary>
        [NotNull]
        ImmutableDictionary<string, IDocumentation> ById { get; }
    }
}
