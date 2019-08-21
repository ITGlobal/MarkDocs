using System;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Source;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Serve.Extensions
{
    public sealed class DefaultEventListener : MarkDocsEventListener
    {
        private readonly bool _quiet;

        public DefaultEventListener(bool quiet)
        {
            _quiet = quiet;
        }

        public override CompilationEventListener CompilationStarted(string id)
        {
            if (_quiet)
            {
                return new QuietEventListener();
            }
            return new VerboseEventListener();
        }

        public override void SourceChanged() { }

        public override void SourceChanged(ISourceTree sourceTree)
        {
            if (!_quiet)
            {
                Console.Error.WriteLine("Source changed, will recompile");
            }
        }

        private sealed class VerboseEventListener : CompilationEventListener
        {
            private readonly ITerminalOutput _output;
            private int _processedAssetCount;
            private int _assetCount;

            public VerboseEventListener()
            {
                _output = TerminalOutput.Create("preparing");
            }

            public override void ReadingAssetTree()
            {
                _output.Write(0, "reading assets");
            }

            public override void ProcessingAssets(AssetTree tree)
            {
                _assetCount = tree.Pages.Count;
                _output.Write(0, $"processing pages (0/{_assetCount})");
            }
            
            public override void Written(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _output.Write(progress, $"processing pages ({_processedAssetCount}/{_assetCount})");
                        break;
                }
            }
            
            public override void Cached(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _output.Write(progress, $"processing pages ({_processedAssetCount}/{_assetCount})");
                        break;
                }
            }
            
            public override void Committing()
            {
                _output.Write(null, "writing results");
            }

            public override void Completed(TimeSpan elapsed)
            {
                _output.Write(100, "completed");
                Console.Error.WriteLine($"Completed in {$"{elapsed.TotalSeconds:F1} s".Cyan()}");
            }

            public override void Dispose()
            {
                _output.Dispose();
            }
        }

        private sealed class QuietEventListener : CompilationEventListener
        {
            public override void ReadingAssetTree()
            {
                Log.Debug("Reading assets");
            }

            public override void ProcessingAssets(AssetTree tree)
            {
                Log.Debug("Compiling assets");
            }

            public override void Committing()
            {
                Log.Debug("Completing");
            }

            public override void Completed(TimeSpan elapsed)
            {
                Log.Information("Compiled in {T:F1}s", elapsed.TotalSeconds);
            }

            public override void Warning(string filename, string message, string location = null)
            {
                var src = location != null ? $"{filename}:{location}" : filename;
                Log.Warning("{Where}: {What}", src, message);
            }

            public override void Error(string message)
            {
                Log.Error("{What}", message);
            }

            public override void Error(string filename, string message, string location = null)
            {
                var src = location != null ? $"{filename}:{location}" : filename;
                Log.Error("{Where}: {What}", src, message);
            }

            public override void Written(Asset asset)
            {
                Log.Debug("Added {Id}", asset.Id);
            }
        }
    }
}