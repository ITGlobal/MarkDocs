using System;
using System.IO;
using System.Threading;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal sealed class StaticDirectorySourceTree : ISourceTree, IDisposable
    {
        private const int WATCH_PERIOD = 1000;

        private readonly ISourceTreeProvider _provider;
        private readonly IAssetTreeReader _reader;
        private readonly string _directory;

        private readonly Timer _watchTimer;
        private readonly FileSystemWatcher _watcher;

        private int _state = 0;

        public StaticDirectorySourceTree(
            ISourceTreeProvider provider,
            IAssetTreeReader reader, 
            string id,
            string directory,
            bool enableWatch)
        {
            _provider = provider;
            _reader = reader;
            _directory = directory;
            Id = id;

            if (enableWatch)
            {
                _watchTimer = new Timer(TimerCallback, null, -1, -1);
                _watcher = new FileSystemWatcher(_directory);
                _watcher.Changed += FileSystemChangeHandler;
                _watcher.Created += FileSystemChangeHandler;
                _watcher.Deleted += FileSystemChangeHandler;
                _watcher.Renamed += FileSystemChangeHandler;
                _watcher.IncludeSubdirectories = true;
                _watcher.EnableRaisingEvents = true;
            }
        }

        public string Id { get; }

        public event EventHandler Changed;
        private void NotifyChanged() => Changed?.Invoke(this, EventArgs.Empty);

        public AssetTree ReadAssetTree(ICompilationReportBuilder report)
        {
            return _reader.ReadAssetTree(_provider, new StaticDirectorySourceTreeRoot(this, _directory), report);
        }

        public void Refresh() { }

        public void Dispose()
        {
            _watcher?.Dispose();
            _watchTimer?.Dispose();
        }

        private void TimerCallback(object state)
        {
            _watchTimer.Change(-1, -1);
            NotifyChanged();

            Interlocked.Exchange(ref _state, 0);
        }

        private void FileSystemChangeHandler(object sender, FileSystemEventArgs e)
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
            {
                _watchTimer.Change(WATCH_PERIOD, WATCH_PERIOD);
            }
        }
    }
}