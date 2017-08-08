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
    ///     Search service wrapper class
    /// </summary>
    internal sealed class SearchServiceWrapper : ISearchService
    {
        private readonly IDocumentation _documentation;
        private readonly ISearchEngine _engine;

        public SearchServiceWrapper(IDocumentation documentation, ISearchEngine engine)
        {
            _documentation = documentation;
            _engine = engine;
        }

        /// <summary>
        ///     Search documentation
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     Search results
        /// </returns>
        public IReadOnlyList<SearchResultItem> Search(string query)
            => _engine.Search(_documentation, query);

        /// <summary>
        ///     Gets a list of search query suggestions
        /// </summary>
        /// <param name="query">
        ///     Search query
        /// </param>
        /// <returns>
        ///     List of search suggestions
        /// </returns>
        public IReadOnlyList<string> Suggest(string query)
            => _engine.Suggest(_documentation, query);
    }
}
