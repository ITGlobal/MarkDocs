using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class EmptyTag : ITag
    {
        private static readonly IReadOnlyList<IBlogPost> EmptyList = new IBlogPost[0];

        public EmptyTag(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int Count => 0;

        public IReadOnlyList<IBlogPost> List(int page, int pageSize = BlogEngineConstants.PageSize)
            => EmptyList;
    }
}