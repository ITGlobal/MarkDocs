using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using System.Linq;
using Lucene.Net.Search.Spell;
using Lucene.Net.Util;

namespace ITGlobal.MarkDocs.Search
{

    /// <summary>
    ///     Search engine
    /// </summary>
    internal interface ISearchEngine
    {
        /// <summary>
        ///     Runs indexing on a documentation state
        /// </summary>
        void Index([NotNull] IMarkDocServiceState state, bool isInitial);

        /// <summary>
        ///     Gets a list of search suggestions
        /// </summary>
        [NotNull]
        IReadOnlyList<string> Suggest(IDocumentation documentation, [NotNull] string query);

        /// <summary>
        ///     Runs a search
        /// </summary>
        [NotNull]
        IReadOnlyList<SearchResultItem> Search(IDocumentation documentation, [NotNull] string query);
    }
}
