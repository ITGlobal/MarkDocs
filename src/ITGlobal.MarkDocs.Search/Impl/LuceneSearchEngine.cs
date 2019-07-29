using ITGlobal.MarkDocs.Extensions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Search.Spell;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Directory = System.IO.Directory;

namespace ITGlobal.MarkDocs.Search.Impl
{
    /// <summary>
    ///     Lucene.Net-bases search engine
    /// </summary>
    internal sealed class LuceneSearchEngine
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

        private readonly IMarkDocsLog _log;
        private readonly string _indexRootDirectory;
        private readonly object _descriptorLock = new object();

        private IndexDescriptor _descriptor;
        private IndexDescriptor _tempDescriptor;

        private readonly object _reindexLock = new object();

        private readonly bool _verbose;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public LuceneSearchEngine(IMarkDocsLog log, SearchOptions options)
        {
            _log = log;
            _indexRootDirectory = options.IndexDirectory;
            _verbose = options.VerboseLogging;

            Directory.CreateDirectory(_indexRootDirectory);

            Directory.CreateDirectory(_indexRootDirectory);
            lock (_descriptorLock)
            {
                _descriptor = IndexDescriptor.LoadOrCreate(_indexRootDirectory, log);
            }
        }

        #endregion

        #region public methods

        public void InitializeIndex(IMarkDocState state)
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
                    Reindex(oldDescriptor, newDescriptor, documentation, force: false);
                }

                // Update index descriptor
                newDescriptor.Save(_indexRootDirectory, _log);
                lock (_descriptorLock)
                {
                    _descriptor = newDescriptor;
                }

                // Remove all old indices
                CleanupOldIndices();

                _log.Info("Search index has been initialized");
            }
        }

        public void AddIndex(IDocumentation documentation)
        {
            try
            {
                BeginUpdateIndex(documentation);
                EndUpdateIndex(documentation);
            }
            catch
            {
                lock (_descriptorLock)
                {
                    _tempDescriptor = null;
                }
                throw;
            }
        }

        public void BeginUpdateIndex(IDocumentation documentation)
        {
            IndexDescriptor newDescriptor;
            IndexDescriptor oldDescriptor;
            lock (_descriptorLock)
            {
                oldDescriptor = _descriptor;
                if (_tempDescriptor == null)
                {
                    _tempDescriptor = new IndexDescriptor();
                }

                newDescriptor = _tempDescriptor;
            }

            Reindex(oldDescriptor, newDescriptor, documentation, force: false);
        }

        public void EndUpdateIndex(IDocumentation documentation)
        {
            IndexDescriptor newDescriptor;
            lock (_descriptorLock)
            {
                if (_tempDescriptor == null)
                {
                    return;
                }

                newDescriptor = _tempDescriptor;
            }

            newDescriptor.Save(_indexRootDirectory, _log);
            lock (_descriptorLock)
            {
                _descriptor = _tempDescriptor ?? _descriptor;
                _tempDescriptor = null;
            }
            _log.Info($"Documentation \"{documentation.Id}\" has been indexed");
        }

        public void DropIndex(IDocumentation documentation)
        {
            IndexDescriptor oldDescriptor;
            lock (_descriptorLock)
            {
                oldDescriptor = _descriptor;
            }

            var newDescriptor = oldDescriptor.Clone();
            if (newDescriptor.Items.Remove(documentation.Id))
            {
                newDescriptor.Save(_indexRootDirectory, _log);
                lock (_descriptorLock)
                {
                    _descriptor = newDescriptor;
                }

                CleanupOldIndices();

                _log.Debug($"Search index for documentation \"{documentation.Id}\" has been dropped");
            }
        }

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
                    using (var indexReader = DirectoryReader.Open(indexDir))
                    using (var spellChecker = new SpellChecker(indexDir))
                    using (var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48))
                    {
                        var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                        spellChecker.IndexDictionary(new LuceneDictionary(indexReader, IDX_PAGE_TITLE), config, true);
                        config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
                        spellChecker.IndexDictionary(new LuceneDictionary(indexReader, IDX_PAGE_TEXT), config, true);

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
                    var searchResults = SearchIterator(documentation, indexDir, q).ToArray();
                    return searchResults;
                });
        }

        #endregion

        #region private methods

        private IEnumerable<SearchResultItem> SearchIterator(
            IDocumentation documentation,
            FSDirectory indexDir,
            string q)
        {
            using (var indexReader = DirectoryReader.Open(indexDir))
            using (var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48))
            {
                var parser = new QueryParser(LuceneVersion.LUCENE_48, IDX_PAGE_TEXT, analyzer);
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
                        var id = document.GetField(IDX_PAGE_ID).GetStringValue();
                        var text = document.GetField(IDX_PAGE_TEXT).GetStringValue();

                        var page = documentation.GetPage(id);
                        if (page == null)
                        {
                            _log.Warning($"Page {documentation.Id}:{id} has not been found while searching for \"{q}\"");
                            continue;
                        }

                        using (var stream = analyzer.GetTokenStream(IDX_PAGE_TEXT, new StringReader(text)))
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

            if (!descriptor.Items.TryGetValue(documentation.Id, out var item))
            {
                return defaultValueFactory();
            }

            var directory = Path.Combine(_indexRootDirectory, item.Path);
            using (var indexDir = FSDirectory.Open(directory))
            {
                return func(indexDir);
            }
        }

        private void Reindex(
            IndexDescriptor oldDescriptor,
            IndexDescriptor newDescriptor,
            IDocumentation documentation,
            bool force)
        {
            var oldDescriptorItem = oldDescriptor.GetOrCreateItem(documentation.Id);
            var newDescriptorItem = newDescriptor.GetOrCreateItem(documentation.Id);

            if (!force && oldDescriptorItem.Version == documentation.SourceInfo.LastChangeId)
            {
                _log.Debug($"Search index for documentation {documentation.Id} is already up-to-date");
                newDescriptorItem.Path = oldDescriptorItem.Path;
                newDescriptorItem.Version = oldDescriptorItem.Version;
                return;
            }

            _log.Debug($"Updating search index for documentation {documentation.Id}");
            newDescriptorItem.Version = documentation.SourceInfo.LastChangeId;

            var path = Path.Combine(_indexRootDirectory, newDescriptorItem.Path);
            Directory.CreateDirectory(path);

            using (var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48))
            {
                var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
                {
                    IndexDeletionPolicy = new KeepOnlyLastCommitDeletionPolicy()
                };
                using (var writer = new IndexWriter(FSDirectory.Open(path), config))
                {
                    foreach (var page in documentation.Pages.Values)
                    {
                        IndexPage(writer, page);
                    }

                    writer.Commit();
                }
            }
        }

        private void IndexPage(IndexWriter writer, IPage page)
        {
            // Render page into plain text
            var plainText = page.RenderPlainText();

            // Add document into index
            var document = new Document
            {
                new StringField(IDX_PAGE_ID, page.Id, Field.Store.YES),
                new TextField(IDX_PAGE_TITLE, page.Title, Field.Store.YES),
                new TextField(IDX_PAGE_TEXT, plainText, Field.Store.YES)
            };

            writer.AddDocument(document);

            if (_verbose)
            {
                _log.Debug($"[{page.Documentation.Id}]: indexed page {page.Id}");
            }
        }

        private void CleanupOldIndices()
        {
            IndexDescriptor descriptor;
            lock (_descriptorLock)
            {
                descriptor = _descriptor;
            }

            var knownDirectoryNames = new HashSet<string>(descriptor.Items.Select(_ => _.Value.Path));
            var indexDirectoriesToDelete =
                Directory.EnumerateDirectories(_indexRootDirectory, "*", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(path => !knownDirectoryNames.Contains(path))
                .ToArray();
            foreach (var directoryName in indexDirectoriesToDelete)
            {
                var path = Path.Combine(_indexRootDirectory, directoryName);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, recursive: true);
                }
            }
        }

        #endregion
    }
}
