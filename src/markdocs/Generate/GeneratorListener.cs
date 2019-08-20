using System;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Source;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class GeneratorListener : MarkDocsEventListener
    {
        public override CompilationEventListener CompilationStarted(string id)
           => new GeneratorCompilationEventListener();

        private sealed class GeneratorCompilationEventListener : CompilationEventListener
        {
            private readonly ILiveOutputManager _manager;
            private readonly ITerminalLiveProgressBar _progressBar;
            private int _processedAssetCount;
            private int _assetCount;

            public GeneratorCompilationEventListener()
            {
                _manager = LiveOutputManager.Create();
                _progressBar = _manager.CreateProgressBar("preparing");
                _progressBar.WipeAfter();
            }

            public override void ReadingAssetTree()
            {
                _progressBar.Write(0, "reading assets");
            }

            public override void ProcessingAssets(AssetTree tree)
            {
                _assetCount = tree.Pages.Count;
                _progressBar.Write(0, "compiling assets");
            }

            public override void Cached(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _progressBar.Write(progress);
                        break;
                }

                Log.Information("Processed {Id}", asset.Id);
            }

            public override void Cached(string assetId)
            {
                Log.Information("Processed {Id}", assetId);
            }

            public override void Written(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _progressBar.Write(progress);
                        break;
                }

                Log.Information("Processed {Id}", asset.Id);
            }

            public override void Committing() { }

            public override void Completed(TimeSpan elapsed)
            {
                Log.Information("Completed in {T:F1}s", elapsed.TotalSeconds);
                _progressBar.Write(100);
            }

            public override void Dispose()
            {
                _manager.Dispose();
            }
        }
    }
}