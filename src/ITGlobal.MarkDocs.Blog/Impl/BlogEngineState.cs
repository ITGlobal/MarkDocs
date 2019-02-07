using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Impl;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogEngineState
    {
        private readonly IDocumentation _documentation;

        public BlogEngineState(IBlogEngine engine, IDocumentation documentation, CompilationReportBuilder report)
        {
            _documentation = documentation;

            Index = new BlogIndex(engine, _documentation, report);
            CompilationReport = new CompilationReport(report.Merge(_documentation.CompilationReport));
        }

        public ISourceInfo SourceInfo => _documentation.SourceInfo;

        public ICompilationReport CompilationReport { get; }

        public BlogIndex Index { get; }

        public IBlogResource GetResource(string id)
        {
            if (!id.StartsWith("/"))
            {
                id = "/" + id;
            }

            Index.Resources.TryGetValue(id, out var resource);
            return resource;
        }

        public ITextSearchResult Search(string query)
        {
            query = (query ?? "").ToLowerInvariant().Trim();

            var results = _documentation.Search(query);

            IEnumerable<ITextSearchResultItem> Iterator()
            {
                foreach (var result in results)
                {
                    if (Index.Posts.TryGetValue(result.Page.Id, out var post))
                    {
                        yield return new TextSearchResultItem(post, result.Preview);
                    }
                }
            }

            return new TextSearchResult(Iterator().ToArray(), query);
        }

        public IReadOnlyList<string> Suggest(string query) => _documentation.Suggest(query);
    }
}