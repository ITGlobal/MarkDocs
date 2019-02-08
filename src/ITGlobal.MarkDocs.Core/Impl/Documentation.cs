using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class Documentation : IDocumentation
    {
        #region fields

        private static readonly PageModel DefaultRootPageModel = new PageModel
        {
            Id = "",
            Title = "",
            RelativePath = "",
            Pages = Array.Empty<PageModel>(),
            Anchors = Array.Empty<PageAnchorModel>(),
            Preview = null,
            Description = "",
            Metadata = PageMetadata.Empty
        };

        private readonly DocumentationModel _model;

        private readonly ImmutableDictionary<string, IPage> _pages;
        private readonly ImmutableDictionary<string, IFileResource> _attachmentsById;

        #endregion

        #region .ctor

        /// <summary>
        ///     .ctor
        /// </summary>
        public Documentation(
            IMarkDocService service,
            ICacheReader cache,
            DocumentationModel model)
        {
            Service = service;
            _model = model;

            var pages = ImmutableDictionary.CreateBuilder<string, IPage>(StringComparer.OrdinalIgnoreCase);
            RootPage = Page.CreateRootPage(cache, this, model.RootPage ?? DefaultRootPageModel);
            BuildPageIndexRec(RootPage);
            _pages = pages.ToImmutable();

            void BuildPageIndexRec(IPage page)
            {
                pages[page.Id] = page;
                foreach (var nestedPage in page.NestedPages)
                {
                    BuildPageIndexRec(nestedPage);
                }
            }

            var files = new List<IFileResource>();
            foreach (var m in model.Files ?? Array.Empty<FileModel>())
            {
                var file = new FileResource(this, cache, m);
                files.Add(file);
            }

            Files = files;
            _attachmentsById = files.ToImmutableDictionary(_ => _.Id, StringComparer.OrdinalIgnoreCase);

            SourceInfo = new SourceInfo(model.Info);
            CompilationReport = new CompilationReport(model.CompilationReport);
        }

        #endregion

        #region IDocumentation

        /// <summary>
        ///     Documentation service
        /// </summary>
        public IMarkDocService Service { get; }

        /// <summary>
        ///     Documentation ID
        /// </summary>
        public string Id => _model.Id;

        /// <summary>
        ///     Documentation title
        /// </summary>
        public string Title => RootPage.Title;

        /// <summary>
        ///     Documentation version
        /// </summary>
        public ISourceInfo SourceInfo { get; }

        /// <summary>
        ///     Root page
        /// </summary>
        public IPage RootPage { get; }

        /// <summary>
        ///     Provides errors and warning for documentation
        /// </summary>
        public ICompilationReport CompilationReport { get; }

        /// <summary>
        ///     Documentation attached files
        /// </summary>
        public IReadOnlyList<IFileResource> Files { get; }

        /// <summary>
        ///     Gets a documentation page by its ID
        /// </summary>
        /// <param name="id">
        ///     Page ID
        /// </param>
        /// <returns>
        ///     A documentation page or null if page doesn't exist
        /// </returns>
        public IPage GetPage(string id)
        {
            ResourceId.Normalize(ref id);

            if (!_pages.TryGetValue(id, out var page))
            {
                return null;
            }

            return page;
        }

        /// <summary>
        ///     Gets an attachment by its ID
        /// </summary>
        /// <param name="id">
        ///     FileResource ID
        /// </param>
        /// <returns>
        ///     An attachment or null if it doesn't exist
        /// </returns>
        public IFileResource GetAttachment(string id)
        {
            ResourceId.Normalize(ref id);

            if (!_attachmentsById.TryGetValue(id, out var attachment))
            {
                return null;
            }

            return attachment;
        }

        #endregion
    }
}