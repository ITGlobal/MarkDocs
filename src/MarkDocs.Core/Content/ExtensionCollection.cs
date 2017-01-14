using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Content
{
    /// <summary>
    ///     A collection of MarkDocs extensions
    /// </summary>
    internal sealed class ExtensionCollection : IDisposable
    {
        private readonly IEnumerable<IExtensionFactory> _extensionFactories;
        private readonly List<IExtension> _extensions = new List<IExtension>();
        private readonly Dictionary<Type, IExtension> _extensionsByType = new Dictionary<Type, IExtension>();

        public ExtensionCollection(IEnumerable<IExtensionFactory> extensionFactories)
        {
            _extensionFactories = extensionFactories;
        }

        public void CreateExtensions(IMarkDocService service, MarkDocServiceState state)
        {
            foreach (var factory in _extensionFactories)
            {
                var extension = factory.Create(service, state);
                _extensions.Add(extension);
                _extensionsByType.Add(extension.GetType(), extension);
            }
        }

        public TExtension GetExtension<TExtension>()
            where TExtension : class, IExtension
        {
            IExtension extension;
            if (_extensionsByType.TryGetValue(typeof(TExtension), out extension))
            {
                return extension as TExtension;
            }

            return null;
        }

        public void Update(MarkDocServiceState state)
        {
            var tasks = _extensions
                .Select(extension => Task.Run((Action) (() => extension.Update(state))))
                .ToArray();
            Task.WaitAll(tasks);
        }

        public void Dispose()
        {
            foreach (var extension in _extensions)
            {
                (extension as IDisposable)?.Dispose();
            }
        }
    }
}