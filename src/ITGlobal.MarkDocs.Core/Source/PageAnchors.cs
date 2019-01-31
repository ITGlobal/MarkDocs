using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Source
{
    [PublicAPI]
    public sealed class PageAnchors : IReadOnlyList<PageAnchor>
    {
        private readonly Dictionary<string, PageAnchor> _byId
            = new Dictionary<string, PageAnchor>(StringComparer.OrdinalIgnoreCase);

        public PageAnchors([NotNull] PageAnchor[] array)
        {
            Array = array;

            foreach (var a in array)
            {
                Walk(a);
            }

            void Walk(PageAnchor anchor)
            {
                _byId[anchor.Id] = anchor;
                if (anchor.Nested != null)
                {
                    foreach (var a in anchor.Nested)
                    {
                        Walk(a);
                    }
                }
            }
        }

        [NotNull]
        public PageAnchor[] Array { get; }

        public int Count => Array.Length;

        [NotNull]
        public PageAnchor this[int index] => Array[index];

        [CanBeNull]
        public PageAnchor this[string id]
        {
            get
            {
                _byId.TryGetValue(id, out var anchor);
                return anchor;
            }
        }

        [NotNull]
        public static implicit operator PageAnchor[] (PageAnchors t) => t.Array;

        public IEnumerator<PageAnchor> GetEnumerator()
        {
            foreach (var a in Array)
            {
                yield return a;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}