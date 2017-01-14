using System;
using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs service state
    /// </summary>
    internal sealed class MarkDocServiceState : IMarkDocServiceState
    {
        public MarkDocServiceState(IList<IDocumentation> documentations)
        {
            List = documentations.ToList();
            ById = documentations.ToDictionary(_ => _.Id, StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        ///     List of all documentations
        /// </summary>
        public IReadOnlyList<IDocumentation> List { get; }

        /// <summary>
        ///     Map of all documentations by theirs ID
        /// </summary>
        public IReadOnlyDictionary<string, IDocumentation> ById { get; }
    }
}