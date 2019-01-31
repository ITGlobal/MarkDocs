using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;
using System;
using System.Collections.Generic;
using System.IO;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class Page : IPage
    {
        private readonly ICacheReader _cache;
        private readonly PageModel _model;

        private Page(ICacheReader cache, IDocumentation documentation, PageModel model, IPage parent)
        {
            _cache = cache;
            Documentation = documentation;
            _model = model;

            if (model.Preview != null)
            {
                Preview = new PagePreview(cache, documentation, model.Preview, this);
            }

            if (model.Pages != null && model.Pages.Length > 0)
            {
                NestedPages = new IPage[model.Pages.Length];
                for (var i = 0; i < model.Pages.Length; i++)
                {
                    NestedPages[i] = new Page(_cache, documentation, model.Pages[i], this);
                }
            }
            else
            {
                NestedPages = Array.Empty<IPage>();
            }
        }

        public static IPage CreateRootPage(ICacheReader cache, IDocumentation documentation, PageModel model)
            => new Page(cache, documentation, model, null);

        public IDocumentation Documentation { get; }
        public IPage[] NestedPages { get; }
        public IPage Parent { get; }
        public IPagePreview Preview { get; }

        public string Id => _model.Id;
        public string FileName => _model.FileName;
        public ResourceType Type => ResourceType.Page;
        public string Title => _model.Title;
        public string Description => _model.Description;
        public PageMetadata Metadata => _model.Metadata;
        public IReadOnlyDictionary<string, string> Anchors { get; }//todo

        public Stream OpenRead() => _cache.Read(this);
    }
}