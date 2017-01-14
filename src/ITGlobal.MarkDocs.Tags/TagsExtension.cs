using System.Collections.Generic;
using System.Text;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Tags
{
    /// <summary>
    ///     Tags extension
    /// </summary>
    internal sealed class TagsExtension : ITagService, IExtension
    {
        #region fields

        private static readonly string[] EmptyTags = new string[0];
        private static readonly IPage[] EmptyPageList = new IPage[0];

        private readonly object _stateLock = new object();
        private TagsExtensionState _state;

        #endregion

        #region .ctor

        public TagsExtension(IMarkDocServiceState state)
        {
            Update(state);
        }

        #endregion

        #region ITagService

        /// <summary>
        ///     Gets a list of all known tags
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        public IReadOnlyList<string> GetTags(IDocumentation documentation)
        {
            lock (_stateLock)
            {
                var branch = _state.GetBranch(documentation);
                return branch?.Tags ?? EmptyTags;
            }
        }

        /// <summary>
        ///     Gets a list of normalized page tags
        /// </summary>
        /// <param name="page">
        ///     Page
        /// </param>
        /// <returns>
        ///     List of tags
        /// </returns>
        public IReadOnlyList<string> GetPageTags(IPage page)
        {
            lock (_stateLock)
            {
                var branch = _state.GetBranch(page.Documentation);
                if (branch == null)
                {
                    return EmptyTags;
                }

                string[] tags;
                if (!branch.PageTags.TryGetValue(page, out tags))
                {
                    return EmptyTags;
                }

                return tags;
            }
        }

        /// <summary>
        ///     Gets a list of pages with specified tag
        /// </summary>
        /// <param name="documentation">
        ///     Documentation
        /// </param>
        /// <param name="tag">
        ///     Tag
        /// </param>
        /// <returns>
        ///     List of pages
        /// </returns>
        public IReadOnlyList<IPage> GetPagesByTag(IDocumentation documentation, string tag)
        {
            tag = NormalizeTag(tag);

            lock (_stateLock)
            {
                var branch = _state.GetBranch(documentation);
                if (branch == null)
                {
                    return EmptyPageList;
                }

                IPage[] pages;
                if (!branch.PageIndex.TryGetValue(tag, out pages))
                {
                    return EmptyPageList;
                }

                return pages;
            }
        }

        #endregion

        #region IExtension

        /// <summary>
        ///     Handle a documentation state update
        /// </summary>
        /// <param name="state">
        ///     New documentation state
        /// </param>
        public void Update(IMarkDocServiceState state)
        {
            var newState = new TagsExtensionState(state);

            lock (_stateLock)
            {
                _state = newState;
            }
        }

        #endregion

        #region internal methods

        internal static string NormalizeTag(string tag)
        {
            var builder = new StringBuilder();

            foreach (var c in tag)
            {
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
                else if (char.IsPunctuation(c) || char.IsWhiteSpace(c))
                {
                    if (builder.Length > 0 && !char.IsPunctuation(builder[builder.Length - 1]))
                    {
                        builder.Append('-');
                    }
                }
            }

            while (builder.Length > 0 && char.IsPunctuation(builder[builder.Length - 1]))
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }

        #endregion
    }
}
