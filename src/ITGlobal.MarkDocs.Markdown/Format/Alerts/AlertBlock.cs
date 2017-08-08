using System;
using System.Collections.Generic;
using System.Text;
using ITGlobal.MarkDocs.Format.ChildrenList;
using Markdig.Parsers;
using Markdig.Syntax;

namespace ITGlobal.MarkDocs.Markdown.Format
{
    public class AlertBlock : ContainerBlock
    {
        internal AlertBlock(BlockParser parser)
            : base(parser)
        { }

        public AlertBlockType Type { get; set; }
        internal int Level { get; set; }
    }
}
