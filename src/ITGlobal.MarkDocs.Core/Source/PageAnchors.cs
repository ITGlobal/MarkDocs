using ITGlobal.MarkDocs.Cache.Model;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     Page anchor list
    /// </summary>
    [PublicAPI]
    public sealed class PageAnchors : IReadOnlyList<PageAnchor>
    {
        private readonly PageAnchor[] _array;
        private readonly Dictionary<string, PageAnchor> _byId
            = new Dictionary<string, PageAnchor>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     .ctor
        /// </summary>
        public PageAnchors([CanBeNull] PageAnchorModel[] array)
            : this(array?.Select(PageAnchor.FromModel).ToArray() ?? System.Array.Empty<PageAnchor>())
        {
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        public PageAnchors([NotNull] PageAnchor[] array)
        {
            _array = array;

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

        /// <inheritdoc />
        public int Count => _array.Length;

        /// <inheritdoc />
        [NotNull]
        public PageAnchor this[int index] => _array[index];

        /// <summary>
        ///     Gets an anchor by its href
        /// </summary>
        [CanBeNull]
        public PageAnchor this[string id]
        {
            get
            {
                _byId.TryGetValue(id, out var anchor);
                return anchor;
            }
        }

        /// <inheritdoc />
        public IEnumerator<PageAnchor> GetEnumerator()
        {
            foreach (var a in _array)
            {
                yield return a;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}