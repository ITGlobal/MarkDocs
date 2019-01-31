using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Impl
{
    /// <summary>
    ///     MarkDocs service state
    /// </summary>
    internal sealed class MarkDocState : IMarkDocState
    {
        private MarkDocState(ImmutableDictionary<string,IDocumentation> documentations)
        {
            List = documentations.Values.ToImmutableArray();
            ById = documentations;
        }
        
        /// <summary>
        ///     List of all documentations
        /// </summary>
        public ImmutableArray<IDocumentation> List { get; }

        /// <summary>
        ///     Map of all documentations by theirs ID
        /// </summary>
        public ImmutableDictionary<string, IDocumentation> ById { get; }

        public static MarkDocState Empty { get; } 
            = new MarkDocState(ImmutableDictionary<string, IDocumentation>.Empty);

        public static MarkDocState New(IEnumerable<IDocumentation> documentations)
        {
            var dict= documentations.ToImmutableDictionary(_ => _.Id, StringComparer.OrdinalIgnoreCase);
            return new MarkDocState(dict);
        }

        public MarkDocState AddOrUpdate(IDocumentation documentation)
        {
            var dict = ById.SetItem(documentation.Id, documentation);
            return new MarkDocState(dict);
        }

        public MarkDocState Remove(IDocumentation documentation)
        {
            var dict = ById.Remove(documentation.Id);
            return new MarkDocState(dict);
        }
    }
}