using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     A factory for search extension
    /// </summary>
    internal sealed class SearchExtensionFactory : IExtensionFactory
    {
        private readonly ISearchEngine _engine;

        public SearchExtensionFactory(ISearchEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        ///     Create an instance of an extension
        /// </summary>
        /// <param name="service">
        ///     MarkDocs service
        /// </param>
        /// <param name="state">
        ///     Initial documentation state
        /// </param>
        /// <returns>
        ///     MarkDocs extension
        /// </returns>
        public IExtension Create(IMarkDocService service, IMarkDocServiceState state) => new SearchExtension(_engine, state);
    }
}
