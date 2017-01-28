using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ITGlobal.MarkDocs.Extensions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using SpellChecker.Net.Search.Spell;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Lucene.Net-bases search engine
    /// </summary>
    internal sealed class LuceneSearchEngine : ISearchEngine
    {
        #region consts

        private const int MAX_SUGGESTIONS = 10;
        private const int MAX_SEARCH_RESULTS = 10;
        private const int MAX_PREVIEW_LENGTH = 10;

        private const string IDX_PAGE_ID = "page_id";
        private const string IDX_PAGE_TITLE = "page_title";
        private const string IDX_PAGE_TEXT = "page_text";

        #endregion

        #region fields

        private readonly ILogger _log;
        private readonly string _indexRootDirectory;
        private readonly object _descriptorLock = new object();

        private IndexDescriptor _descriptor;

        private readonly object _reindexLock = new object();

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public LuceneSearchEngine(ILoggerFactory loggerFactory, string indexRootDirectory)
        {
            _log = loggerFactory.CreateLogger<LuceneSearchEngine>();
            _indexRootDirectory = indexRootDirectory;

            lock (_descriptorLock)
            {
                _descriptor = IndexDescriptor.LoadOrCreate(_log, _indexRootDirectory);
            }
        }

        #endregion

        #region ISearchEngine

        /// <summary>
        ///     Runs indexing on a documentation state
        /// </summary>
        public void Index(IMarkDocServiceState state, bool isInitial) => Reindex(state, force: isInitial);

        /// <summary>
        ///     Gets a list of search suggestions
        /// </summary>
        public IReadOnlyList<string> Suggest(IDocumentation documentation, string q)
        {
            return ExecuteWithActiveIndex<IReadOnlyList<string>>(
                documentation,
                () => new string[0],
                indexDir =>
                {
                    using (var indexReader = IndexReader.Open(indexDir, true))
                    using (var spellChecker = new SpellChecker.Net.Search.Spell.SpellChecker(indexDir))
                    {
                        spellChecker.IndexDictionary(new LuceneDictionary(indexReader, IDX_PAGE_TITLE));
                        spellChecker.IndexDictionary(new LuceneDictionary(indexReader, IDX_PAGE_TEXT));

                        var suggestions = spellChecker.SuggestSimilar(q, MAX_SUGGESTIONS);
                        return suggestions;
                    }
                });
        }

        /// <summary>
        ///     Runs a search
        /// </summary>
        public IReadOnlyList<SearchResultItem> Search(IDocumentation documentation, string q)
        {
            return ExecuteWithActiveIndex<IReadOnlyList<SearchResultItem>>(
                documentation,
                () => new SearchResultItem[0],
                indexDir =>
                {
                    var searchResuls = SearchIterator(documentation, indexDir, q).ToArray();
                    return searchResuls;
                });
        }

        #endregion

        #region private methods

        private IEnumerable<SearchResultItem> SearchIterator(
            IDocumentation documentation,
            FSDirectory indexDir,
            string q)
        {
            using (var indexReader = IndexReader.Open(indexDir, true))
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                var parser = new QueryParser(Version.LUCENE_30, IDX_PAGE_TEXT, analyzer);
                var query = parser.Parse(q);

                var searcher = new IndexSearcher(indexReader);
                var hits = searcher.Search(query, MAX_SEARCH_RESULTS);

                if (hits.TotalHits > 0)
                {
                    var scorer = new QueryScorer(query);

                    var highlighter = new Highlighter(new SimpleHTMLFormatter(), scorer)
                    {
                        TextFragmenter = new SimpleFragmenter()
                    };

                    foreach (var hit in hits.ScoreDocs)
                    {
                        var document = searcher.Doc(hit.Doc);
                        var id = document.GetField(IDX_PAGE_ID).StringValue;
                        var text = document.GetField(IDX_PAGE_TEXT).StringValue;

                        var page = documentation.GetPage(id);
                        if (page == null)
                        {
                            _log.LogWarning("Page [{0}!{1}] has not been found while searching for '{2}'", documentation.Id, id, q);
                            continue;
                        }

                        using (var stream = analyzer.TokenStream(IDX_PAGE_TEXT, new StringReader(text)))
                        {
                            var previews = highlighter.GetBestFragments(stream, text, MAX_PREVIEW_LENGTH);
                            var preview = string.Join("\n", previews.Select(html => html.Trim()));
                            yield return new SearchResultItem(page, preview);
                        }
                    }
                }
            }
        }

        private T ExecuteWithActiveIndex<T>(IDocumentation documentation, Func<T> defaultValueFactory, Func<FSDirectory, T> func)
        {
            IndexDescriptor descriptor;
            lock (_descriptorLock)
            {
                descriptor = _descriptor;
            }

            IndexDescriptorItem item;
            if (!descriptor.Items.TryGetValue(documentation.Id, out item))
            {
                return defaultValueFactory();
            }

            var directory = Path.Combine(_indexRootDirectory, item.Path);
            using (var indexDir = FSDirectory.Open(directory))
            {
                return func(indexDir);
            }
        }

        private void Reindex(IMarkDocServiceState state, bool force)
        {
            lock (_reindexLock)
            {
                var newDescriptor = new IndexDescriptor();
                IndexDescriptor oldDescriptor;
                lock (_descriptorLock)
                {
                    oldDescriptor = _descriptor;
                }

                foreach (var documentation in state.List)
                {
                    Reindex(oldDescriptor, newDescriptor, documentation, force);
                }

                // Update index descriptor
                newDescriptor.Save(_log, _indexRootDirectory);
                lock (_descriptorLock)
                {
                    _descriptor = newDescriptor;
                }

                // Remove all old indices
                var indexDirectoriesToDelete = oldDescriptor.Items
                    .Select(_ => _.Value.Path)
                    .Except(newDescriptor.Items.Select(_ => _.Value.Path))
                    .ToArray();
                foreach (var directoryName in indexDirectoriesToDelete)
                {
                    var path = Path.Combine(_indexRootDirectory, directoryName);
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, recursive: true);
                    }
                }

                _log.LogInformation("Search index has been updated");
            }
        }

        private void Reindex(IndexDescriptor oldDescriptor, IndexDescriptor newDescriptor, IDocumentation documentation, bool force)
        {
            var oldItem = oldDescriptor.GetOrCreateItem(documentation.Id);
            var newItem = newDescriptor.GetOrCreateItem(documentation.Id);

            if (!force && oldItem.Version == documentation.ContentVersion.LastChangeId)
            {
                _log.LogDebug("Search index for documentation {0} is already up-to-date", documentation.Id);
                newItem.Path = oldItem.Path;
                newItem.Version = oldItem.Version;
                return;
            }

            _log.LogDebug("Updating search index for documentation {0}", documentation.Id);
            newItem.Version = documentation.ContentVersion.LastChangeId;

            var path = Path.Combine(_indexRootDirectory, newItem.Path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var writer = new IndexWriter(
                FSDirectory.Open(path),
                new StandardAnalyzer(Version.LUCENE_30),
                true,
                new KeepOnlyLastCommitDeletionPolicy(),
                IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var node in documentation.PageTree.Pages)
                {
                    var page = documentation.GetPage(node.Id);
                    IndexPage(writer, page);
                }

                writer.Optimize(true);
                writer.Commit();
            }
        }

        private void IndexPage(IndexWriter writer, IPage page)
        {
            _log.LogDebug("Indexing page [{0}!{1}]...", page.Documentation.Id, page.Id);
            
            // Render page into plain text
            var plainText = page.RenderPlainText();

            // Add document into index
            var document = new Document();
            document.Add(new Field(IDX_PAGE_ID, page.Id, Field.Store.YES, Field.Index.NO));
            document.Add(new Field(IDX_PAGE_TITLE, page.Title, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field(IDX_PAGE_TEXT, plainText, Field.Store.YES, Field.Index.ANALYZED));

            writer.AddDocument(document);
        }
        
        #endregion
    }
}