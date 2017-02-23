using System;
using System.Collections.Generic;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using static ITGlobal.MarkDocs.Format.MarkdownRenderingContext;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal sealed class CustomCodeBlockRenderer : CodeBlockRenderer
    {
        private readonly Dictionary<string, ICodeBlockRenderer> _renderers = new Dictionary<string, ICodeBlockRenderer>();

        public CustomCodeBlockRenderer(MarkdownOptions options)
        {
            if (options.SyntaxColorizer != null)
            {
                var renderer = new SourceCodeRenderer(options.SyntaxColorizer);
                foreach (var language in options.SyntaxColorizer.SupportedLanguages)
                {
                    Register(language, renderer);
                }
            }

            Register("plantuml", new PlantUmlRenderer());
        }

        private void Register(string lang, ICodeBlockRenderer renderer)
        {
            _renderers[lang] = renderer;
        }

        protected override void Write(HtmlRenderer renderer, CodeBlock obj)
        {
            renderer.EnsureLine();

            var fencedCodeBlock = obj as FencedCodeBlock;
            if (fencedCodeBlock?.Info != null)
            {
                ICodeBlockRenderer r;
                if (_renderers.TryGetValue(fencedCodeBlock.Info, out r))
                {
                    try
                    {
                        if (r.Write(renderer, fencedCodeBlock))
                        {
                            return;
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.LogError(0, exception, "Error while rendering {0} with {1}", nameof(CodeBlock), r.GetType().Name);
                    }
                }

                if (BlocksAsDiv.Contains(fencedCodeBlock.Info))
                {
                    var infoPrefix = (fencedCodeBlock.Parser as FencedCodeBlockParser)?.InfoPrefix ??
                                     FencedCodeBlockParser.DefaultInfoPrefix;
                    
                    renderer.Write("<div")
                        .WriteAttributes(fencedCodeBlock.TryGetAttributes(),
                            cls => cls.StartsWith(infoPrefix) ? cls.Substring(infoPrefix.Length) : cls)
                        .Write(">");
                    renderer.WriteLeafRawLines(fencedCodeBlock, true, true, true);
                    renderer.WriteLine("</div>");

                    return;
                }
            }

            renderer.Write("<pre");
            if (OutputAttributesOnPre)
            {
                renderer.WriteAttributes(obj);
            }
            renderer.Write("><code");
            if (!OutputAttributesOnPre)
            {
                renderer.WriteAttributes(obj);
            }
            renderer.Write(">");
            renderer.WriteLeafRawLines(obj, true, true);
            renderer.WriteLine("</code></pre>");
        }
    }
}