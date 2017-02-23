using Markdig.Renderers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    internal interface ICodeBlockRenderer
    {
        bool Write(HtmlRenderer renderer, FencedCodeBlock block);
    }
}