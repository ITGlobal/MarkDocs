using System.Collections.Generic;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs service state
    /// </summary>
    [PublicAPI]
    public interface IMarkDocServiceState
    {
        /// <summary>
        ///     List of all documentations
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyList<IDocumentation> List { get; }

        /// <summary>
        ///     Map of all documentations by theirs ID
        /// </summary>
        [PublicAPI, NotNull]
        IReadOnlyDictionary<string, IDocumentation> ById { get; }
    }
}
