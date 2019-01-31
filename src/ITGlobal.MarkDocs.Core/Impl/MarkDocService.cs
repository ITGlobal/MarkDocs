using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Source;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class MarkDocService : IMarkDocService
    {
        private readonly IMarkDocsLog _log;
        private readonly ICacheProvider _cache;
        private readonly ISourceTreeProvider _sourceTreeProvider;
        private readonly ExtensionCollection _extensions;

        private readonly object _synchronizationLock = new object();

        private readonly object _stateLock = new object();
        private MarkDocState _state = MarkDocState.Empty;

        public MarkDocService(
            IMarkDocsLog log,
            ICacheProvider cache,
            ISourceTreeProvider sourceTreeProvider,
            ExtensionCollection extensions)
        {
            _log = log;
            _cache = cache;
            _sourceTreeProvider = sourceTreeProvider;
            _extensions = extensions;

            Initialize();
        }

        private void Initialize()
        {
            lock (_stateLock)
            {
                _extensions.CreateExtensions(this);

                var state = RecoverState();
                if (state == null)
                {
                    Synchronize(fullResync: true);
                }
                else
                {
                    _state = state;
                    _extensions.Initialize(state);

                    foreach (var sourceTree in _sourceTreeProvider.GetSourceTrees())
                    {
                        sourceTree.Changed += OnSourceTreeChanged;
                    }
                }

                _sourceTreeProvider.Changed += (_, e) =>
                {
                    _log.Debug("Source tree list change detected");
                    Task.Run(() => Rebuild(fullResync: false));
                };
            }

            MarkDocState RecoverState()
            {
                try
                {
                    var models = _cache.Load();
                    if (models.Length == 0)
                    {
                        _log.Warning("Cache is missing or damaged, will run a full resync");
                        return null;
                    }

                    var documentations = new List<IDocumentation>();
                    foreach (var (model, cacheReader) in models)
                    {
                        var documentation = new Documentation(this, cacheReader, model);
                        documentations.Add(documentation);
                    }

                    return MarkDocState.New(documentations);
                }
                catch (Exception e)
                {
                    _log.Error(e, "Cache is missing or damaged, will run a full resync");
                    return null;
                }
            }
        }

        public IReadOnlyList<IDocumentation> Documentations
        {
            get
            {
                lock (_stateLock)
                {
                    return _state.List;
                }
            }
        }

        public IDocumentation GetDocumentation(string documentationId)
        {
            lock (_stateLock)
            {
                _state.ById.TryGetValue(documentationId, out var documentation);
                return documentation;
            }
        }

        public void Synchronize()
        {
            Synchronize(fullResync: false);
        }

        public void Synchronize(string id)
        {
            lock (_synchronizationLock)
            {
                var sourceTree = _sourceTreeProvider.GetSourceTree(id);
                if (sourceTree != null)
                {
                    sourceTree.Refresh();
                    Rebuild(sourceTree);
                }
            }
        }

        public TExtension GetExtension<TExtension>()
            where TExtension : class, IExtension
        {
            return _extensions.GetExtension<TExtension>();
        }

        public void Dispose()
        {
            // TODO
        }

        private void Synchronize(bool fullResync)
        {
            lock (_synchronizationLock)
            {
                _sourceTreeProvider.Refresh();
                Rebuild(fullResync: fullResync);
            }
        }

        private void Rebuild(bool fullResync)
        {
            MarkDocState state;
            lock (_stateLock)
            {
                state = _state;
            }

            var toRemove = state.ById;
            foreach (var sourceTree in _sourceTreeProvider.GetSourceTrees())
            {
                var isUpdate = fullResync || state.ById.ContainsKey(sourceTree.Id);
                Rebuild(sourceTree, isUpdate);
                toRemove = toRemove.Remove(sourceTree.Id);

                if (!state.ById.ContainsKey(sourceTree.Id))
                {
                    sourceTree.Changed += OnSourceTreeChanged;
                }
            }

            lock (_stateLock)
            {
                foreach (var (_, documentation) in toRemove)
                {
                    _state = _state.Remove(documentation);
                    _extensions.Removed(documentation);
                }
            }
        }

        private void Rebuild(ISourceTree sourceTree, bool isUpdate = true)
        {
            var w = Stopwatch.StartNew();
            var reportBuilder = new CompilationReportBuilder();
            var tree = sourceTree.ReadAssetTree(reportBuilder);

            using (var transaction = _cache.BeginTransaction(sourceTree, tree.SourceInfo, forceCacheClear: false))
            {
                var compiler = new DocumentationCompiler(transaction);
                var model = compiler.Compile(tree);

                var reader = transaction.Commit();

                var documentation = new Documentation(this, reader, model);

                IDisposable disposable = null;
                try
                {
                    if (!isUpdate)
                    {
                        _extensions.Created(documentation);
                    }
                    else
                    {
                        disposable = _extensions.Updated(documentation);
                    }

                    lock (_stateLock)
                    {
                        _state = _state.AddOrUpdate(documentation);
                    }
                }
                finally
                {
                    disposable?.Dispose();
                }
            }
            w.Stop();
            _log.Info($"Documentation \"{sourceTree.Id}\" has been rebuilt in {w.ElapsedMilliseconds / 1000f:F1}s");
        }

        private void OnSourceTreeChanged(object sender, EventArgs e)
        {
            if (sender is ISourceTree sourceTree)
            {
                Rebuild(sourceTree);
            }
        }
    }
}
