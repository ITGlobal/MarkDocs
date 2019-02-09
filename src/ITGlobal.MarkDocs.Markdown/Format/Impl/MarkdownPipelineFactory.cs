using System;
using Markdig;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal sealed class MarkdownPipelineFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MarkdownOptions _options;

        public MarkdownPipelineFactory(IServiceProvider serviceProvider, MarkdownOptions options)
        {
            _serviceProvider = serviceProvider;
            _options = options;
        }

        public MarkdownPipeline Create() => _options.CreateMarkdownPipeline(_serviceProvider);
    }
}