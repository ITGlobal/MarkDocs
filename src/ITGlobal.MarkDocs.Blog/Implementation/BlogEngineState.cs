using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Content;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Search;

namespace ITGlobal.MarkDocs.Blog.Implementation
{
    internal sealed class BlogEngineState
    {
        private readonly IDocumentation _documentation;

        public BlogEngineState(IBlogEngine engine, IMarkDocServiceState state)
        {
            var report = new CompilationReportBuilder();
            if (state.List.Count > 1)
            {
                report.Error($"More than one source found. Choosing \"{state.List[0].Id}\" arbitrarily");
            }

            _documentation = state.List[0];

            Index = new BlogIndex(engine, _documentation, report);
            report.MergeWith(_documentation.CompilationReport);
            CompilationReport = report.Build();
        }

        public IContentVersion ContentVersion => _documentation.ContentVersion;

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