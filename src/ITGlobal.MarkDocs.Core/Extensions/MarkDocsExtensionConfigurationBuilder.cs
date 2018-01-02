using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs extension configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsExtensionConfigurationBuilder
    {
        private readonly List<Func<MarkdocsFactoryContext, IExtensionFactory>> _extensions = new List<Func<MarkdocsFactoryContext, IExtensionFactory>>();

        /// <summary>
        ///     Adds an extension
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsExtensionConfigurationBuilder Add([NotNull] Func<MarkdocsFactoryContext, IExtensionFactory> func)
        {
            _extensions.Add(func);
            return this;
        }

        internal IExtensionFactory[] Build(MarkdocsFactoryContext context)
        {
            return _extensions.Select(factory => factory(context)).ToArray();
        }
    }
}