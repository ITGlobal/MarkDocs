using System;
using ITGlobal.CommandLine.ProgressBars;
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
            private readonly IProgressBar _progressBar;
            private int _processedAssetCount;
            private int _assetCount;

            public GeneratorCompilationEventListener()
            {
                _progressBar = TerminalProgressBar.Create();
            }

            public override void ReadingAssetTree()
            {
                _progressBar.SetState(0, "reading assets");
            }

            public override void ProcessingAssets(AssetTree tree)
            {
                _assetCount = tree.Pages.Count;
                _progressBar.SetState(0, "compiling assets");
            }

            public override void Cached(Asset asset)
            {
                switch (asset)
                {
                    case PageAsset _:
                        _processedAssetCount++;
                        var progress = (int)Math.Ceiling(100f * _processedAssetCount / _assetCount);
                        _progressBar.SetState(progress);
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
                        _progressBar.SetState(progress);
                        break;
                }

                Log.Information("Processed {Id}", asset.Id);
            }

            public override void Committing() { }

            public override void Completed(TimeSpan elapsed)
            {
                Log.Information($"Completed in {elapsed.TotalSeconds:F1}s");
                _progressBar.SetState(100);
            }

            public override void Dispose()
            {
                _progressBar.Dispose();
            }
        }
    }
}