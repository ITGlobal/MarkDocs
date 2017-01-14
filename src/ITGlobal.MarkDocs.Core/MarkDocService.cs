using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Content;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs documentation service. Provides access to the documentation data.
    /// </summary>
    internal sealed class MarkDocService : IMarkDocService
    {
        #region fields

        private readonly ExtensionCollection _extensions;

        private readonly ManualResetEventSlim _initialized = new ManualResetEventSlim(false);

        private readonly object _stateLock = new object();
        private MarkDocServiceState _state;

        #endregion

        #region .ctor

        /// <summary>
        ///     Constructor
        /// </summary>
        public MarkDocService(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IStorage storage,
            [NotNull] IFormat format,
            [NotNull] ICache cache,
            [NotNull] IEnumerable<IExtensionFactory> extensionFactories)
        {
            Log = loggerFactory.CreateLogger(typeof(MarkDocService));
            LoggerFactory = loggerFactory;
            Storage = storage;
            Format = format;
            Cache = cache;
            _extensions = new ExtensionCollection(extensionFactories);

            storage.Changed += OnStorageChanged;
        }

        #endregion

        #region properties

        private MarkDocServiceState State
        {
            get
            {
                _initialized.Wait();

                lock (_stateLock)
                {
                    return _state;
                }
            }
        }

        internal ILogger Log { get; }
        internal IContentTypeProvider ContentTypeProvider { get; } = new FileExtensionContentTypeProvider();
        internal ILoggerFactory LoggerFactory { get; }
        internal IStorage Storage { get; }
        internal IFormat Format { get; }
        internal ICache Cache { get; }

        #endregion

        #region IMarkDocService

        /// <inheritdoc />
        public IReadOnlyList<IDocumentation> Documentations => State.List;

        /// <inheritdoc />
        void IMarkDocService.Initialize()
        {
            using (Log.BeginScope("Initialize()"))
            {
                try
                {
                    Log.LogInformation("Initializing documentation...");
                    Storage.Initialize();

                    var state = RefreshAllDocumentations(_state);

                    _extensions.CreateExtensions(this, state);

                    Log.LogInformation("Documentation is ready");

                    _initialized.Set();

                }
                catch (Exception e)
                {
                    Log.LogError(0, e, "Unable to initialize MarkDocs");
                    throw;
                }
            }
        }

        /// <inheritdoc />
        IDocumentation IMarkDocService.GetDocumentation(string id)
        {
            Documentation.NormalizeId(ref id);

            IDocumentation doc;
            if (!State.ById.TryGetValue(id, out doc))
            {
                return null;
            }

            return doc;
        }

        /// <inheritdoc />
        void IMarkDocService.RefreshDocumentation(string id) => RefreshDocumentation(id);

        /// <inheritdoc />
        void IMarkDocService.RefreshAllDocumentations() => RefreshAllDocumentations(State);

        /// <inheritdoc />
        TExtension IMarkDocService.GetExtension<TExtension>() => _extensions.GetExtension<TExtension>();

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            Log.LogDebug("Shutting down...");

            // Dispose extensions
            _extensions.Dispose();

            // Dispose services
            (Storage as IDisposable)?.Dispose();
            (Cache as IDisposable)?.Dispose();
            (Format as IDisposable)?.Dispose();

            Log.LogDebug("Shutdown is complete");
        }

        #endregion

        #region private methods
        
        internal void RefreshDocumentation(string id)
        {
            Documentation.NormalizeId(ref id);

            using (Log.BeginScope("Refresh({0})", id))
            {
                Storage.Refresh(id);
                RebuildDocumentation(id);
            }
        }

        private MarkDocServiceState RefreshAllDocumentations(MarkDocServiceState originalState)
        {
            using (Log.BeginScope("RefreshAll()"))
            {
                Storage.RefreshAll();
                return RebuildDocumentations(originalState);
            }
        }

        private MarkDocServiceState RebuildDocumentations(MarkDocServiceState originalState)
        {
            try
            {
                var docs = new List<IDocumentation>();
                MarkDocServiceState state;

                using (var directoryScanner = new DirectoryScanner(Log, Format, Storage))
                using (var operation = Cache.BeginUpdate())
                {
                    foreach (var contentDirectory in Storage.GetContentDirectories())
                    {
                        var doc = CompileDocumentation(directoryScanner, operation, contentDirectory);
                        docs.Add(doc);
                    }

                    state = new MarkDocServiceState(docs);

                    operation.Flush();

                    lock (_stateLock)
                    {
                        operation.Commit();
                        _state = state;
                    }
                }

                _extensions.Update(state);

                return state;
            }
            catch (Exception e)
            {
                Log.LogError(0, e, "Failed to rebuild all documentations");
                return originalState;
            }
        }

        private void RebuildDocumentation(string id)
        {
            try
            {
                Documentation.NormalizeId(ref id);

                var docs = new List<IDocumentation>(from d in State.List where d.Id != id select d);
                MarkDocServiceState state;

                using (var directoryScanner = new DirectoryScanner(Log, Format, Storage))
                using (var operation = Cache.BeginUpdate())
                {
                    var contentDirectory = Storage.GetContentDirectory(id);
                    var doc = CompileDocumentation(directoryScanner, operation, contentDirectory);
                    docs.Add(doc);

                    state = new MarkDocServiceState(docs);

                    operation.Flush();

                    lock (_stateLock)
                    {
                        operation.Commit();
                        _state = state;
                    }
                }

                _extensions.Update(state);
            }
            catch (Exception e)
            {
                Log.LogError(0, e, "Failed to rebuild documentation [{0}]", id);
            }
        }

        private IDocumentation CompileDocumentation(
            DirectoryScanner directoryScanner,
            ICacheUpdateOperation operation,
            IContentDirectory contentDirectory)
        {
            var catalog = directoryScanner.ScanDirectory(contentDirectory.Path);

            var documentation = new Documentation(this, contentDirectory.Id, contentDirectory.ContentVersion, catalog);
            documentation.Compile(operation);

            return documentation;
        }

        private void OnStorageChanged(object sender, StorageChangedEventArgs e)
        {
            Log.LogInformation("Documentation change detected");
            if (!string.IsNullOrEmpty(e.DocumentationId))
            {
                RebuildDocumentation(e.DocumentationId);
            }
            else
            {
                RebuildDocumentations(State);
            }
        }

        #endregion
    }
}
