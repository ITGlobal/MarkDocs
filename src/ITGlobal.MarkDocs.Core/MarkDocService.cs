using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        internal MarkDocService(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IStorage storage,
            [NotNull] IFormat format,
            [NotNull] ICache cache,
            [NotNull] IMarkDocsEventCallback callback,
            [NotNull] IEnumerable<IExtensionFactory> extensionFactories)
        {
            Log = loggerFactory.CreateLogger(typeof(MarkDocService));
            LoggerFactory = loggerFactory;
            Storage = storage;
            Format = format;
            Cache = cache;
            Callback = callback;
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
        internal IMarkDocsEventCallback Callback { get; }

        #endregion

        #region IMarkDocService

        /// <inheritdoc />
        public IReadOnlyList<IDocumentation> Documentations => State.List;

        /// <inheritdoc />
        void IMarkDocService.Initialize()
        {
            using (Log.BeginScope("Initialize"))
            {
                try
                {
                    Log.LogInformation("Initializing documentation...");
                    Storage.Initialize();
                    var contentDirectories = Storage.GetContentDirectories();

                    var result = Cache.Verify(contentDirectories);
                    if (result == CacheVerifyResult.UpToDate)
                    {
                        // TODO restore state from cache instead of recompiling
                    }

                    using (Callback.CompilationStarted())
                    {
                        Recompile();
                    }

                    _extensions.CreateExtensions(this, _state);

                    Storage.EnableChangeNotifications();
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

            if (!State.ById.TryGetValue(id, out var doc))
            {
                return null;
            }

            return doc;
        }

        /// <inheritdoc />
        void IMarkDocService.Synchronize() => Resync();

        /// <inheritdoc />
        TExtension IMarkDocService.GetExtension<TExtension>() => _extensions.GetExtension<TExtension>();

        #endregion

        #region IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            Log.LogDebug("Shutting down...");

            // Dispose extensions
            _extensions.Dispose();

            // Dispose services
            (Callback as IDisposable)?.Dispose();
            (Storage as IDisposable)?.Dispose();
            (Cache as IDisposable)?.Dispose();
            (Format as IDisposable)?.Dispose();

            Log.LogDebug("Shutdown is complete");
        }

        #endregion
        
        #region private methods

        private void Resync(string documentationId = null)
        {
            using (Log.BeginScope("Resync"))
            {
                using (Callback.StorageRefresh(documentationId))
                {
                    if (!string.IsNullOrEmpty(documentationId))
                    {
                        Storage.Refresh(documentationId);
                    }
                    else
                    {
                        Storage.RefreshAll();
                    }
                }

                var contentDirectories = Storage.GetContentDirectories();

                var result = Cache.Verify(contentDirectories);
                if (result == CacheVerifyResult.UpToDate)
                {
                    Log.LogInformation("Documentation is already up to date");
                    return;
                }

                using (Callback.CompilationStarted())
                {
                    Recompile(documentationId);
                }
            }
        }

        private void Recompile(string id = null)
        {
            try
            {
                var docs = new List<IDocumentation>();
                if (!string.IsNullOrEmpty(id))
                {
                    Documentation.NormalizeId(ref id);
                    lock (_stateLock)
                    {
                        foreach (var doc in from d in _state.List where d.Id != id select d)
                        {
                            docs.Add(doc);
                        }
                    }
                }

                MarkDocServiceState state;
                using (var directoryScanner = new DirectoryScanner(Log, Format, Storage))
                using (var operation = Cache.BeginUpdate())
                {
                    var contentDirectories = Storage.GetContentDirectories();
                    if (!string.IsNullOrEmpty(id))
                    {
                        contentDirectories = contentDirectories.Where(d => d.Id == id).ToArray();
                    }

                    foreach (var contentDirectory in contentDirectories)
                    {
                        using (Callback.CompilationStarted(contentDirectory.Id))
                        {
                            var doc = new Documentation(this, contentDirectory);
                            doc.Compile(directoryScanner, operation);
                            docs.Add(doc);
                        }
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
            }
            catch (Exception e)
            {
                Log.LogError(0, e, "Failed to rebuild documentation [{0}]", id);
            }
        }

        private void OnStorageChanged(object sender, StorageChangedEventArgs e)
        {
            Log.LogInformation("Documentation change detected");
            Callback.StorageChanged(e.DocumentationId);

            Resync(e.DocumentationId);
        }

        #endregion
    }
}
