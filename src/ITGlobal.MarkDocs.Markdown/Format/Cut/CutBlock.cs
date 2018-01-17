using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Format.TableOfContents;
using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Markdown.Format.Cut
{
    internal sealed class CutBlock: Block
    {
        public CutBlock(BlockParser parser)
            : base(parser)
        {
        }
    }
}
