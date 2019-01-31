using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITGlobal.MarkDocs.Extensions;

namespace ITGlobal.MarkDocs.Impl
{
    /// <summary>
    ///     A collection of MarkDocs extensions
    /// </summary>
    internal sealed class ExtensionCollection : IDisposable
    {
        private sealed class UpdatedDisposableToken : IDisposable
        {
            private readonly ExtensionCollection _extensions;
            private readonly IDocumentation _documentation;

            public UpdatedDisposableToken(ExtensionCollection extensions, IDocumentation documentation)
            {
                _extensions = extensions;
                _documentation = documentation;
            }

            public void Dispose()
            {
                _extensions.UpdateCompleted(_documentation);
            }
        }

        private readonly IEnumerable<IExtensionFactory> _extensionFactories;
        private readonly List<IExtension> _extensions = new List<IExtension>();
        private readonly Dictionary<Type, IExtension> _extensionsByType = new Dictionary<Type, IExtension>();

        public ExtensionCollection(IEnumerable<IExtensionFactory> extensionFactories)
        {
            _extensionFactories = extensionFactories;
        }

        public void CreateExtensions(IMarkDocService service)
        {
            foreach (var factory in _extensionFactories)
            {
                var extension = factory.Create(service);
                _extensions.Add(extension);
                _extensionsByType.Add(extension.GetType(), extension);
            }
        }

        public TExtension GetExtension<TExtension>()
            where TExtension : class, IExtension
        {
            if (_extensionsByType.TryGetValue(typeof(TExtension), out var extension))
            {
                return extension as TExtension;
            }

            throw new InvalidOperationException($"Extension {typeof(TExtension).Name} is not registered");
        }

        public void Initialize(IMarkDocState state)
        {
            ForEachExtension(state, (e, s) => e.Initialize(s));
        }

        public void Created(IDocumentation documentation)
        {
            ForEachExtension(documentation, (e, d) => e.OnCreated(d));
        }

        public IDisposable Updated(IDocumentation documentation)
        {
            ForEachExtension(documentation, (e, d) => e.OnUpdated(d));

            return new UpdatedDisposableToken(this, documentation);
        }

        public void Removed(IDocumentation documentation)
        {
            ForEachExtension(documentation, (e, d) => e.OnRemoved(d));
        }

        private void UpdateCompleted(IDocumentation documentation)
        {
            ForEachExtension(documentation, (e, d) => e.OnUpdateCompleted(d));
        }

        private void ForEachExtension<T>(T arg, Action<IExtension, T> action)
        {
            var tasks = _extensions
                .Select(extension => Task.Run(() => action(extension, arg)))
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