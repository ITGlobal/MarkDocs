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
            FileName = "",
            Pages = Array.Empty<PageModel>(),
            Anchors = Array.Empty<PageAnchorModel>(),
            Preview = null,
            Description = "",
            Metadata = PageMetadata.Empty
        };

        private readonly DocumentationModel _model;

        private readonly ImmutableDictionary<string, IPage> _pages;
        private readonly List<IAttachment> _attachments;
        private readonly ImmutableDictionary<string, IAttachment> _attachmentsById;

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

            var attachments = new List<IAttachment>();
            foreach (var m in model.Attachments ?? Array.Empty<AttachmentModel>())
            {
                var attachment = new Attachment(this, cache, m);
                attachments.Add(attachment);
            }

            Attachments = attachments;
            _attachmentsById = attachments.ToImmutableDictionary(_ => _.Id, StringComparer.OrdinalIgnoreCase);

            SourceInfo = new SourceInfo(model.Info);
            CompilationReport = new CompilationReport(this, model.CompilationReport);
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
        public IReadOnlyList<IAttachment> Attachments { get; }

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
        ///     Attachment ID
        /// </param>
        /// <returns>
        ///     An attachment or null if it doesn't exist
        /// </returns>
        public IAttachment GetAttachment(string id)
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