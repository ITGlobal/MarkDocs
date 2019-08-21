using System.IO;
using ITGlobal.CommandLine.Parsing;
using ITGlobal.MarkDocs.Tools.Lint;

namespace ITGlobal.MarkDocs.Tools
{
    public static class LintCommand
    {
        public static void Setup(ICliCommandRoot app, CliOption<string> tempDir, CliSwitch quiet)
        {
            var command = app.Command("lint")
                .HelpText("Run a linter on a documentation");

            var pathParameter = command
                .Argument("path")
                .DefaultValue(".")
                .HelpText("Path to documentation root directory")
                .Required();

            var summary = command
                .Switch('s', "summary")
                .HelpText("Display summary");

            command.OnExecute(_ =>
            {
                var path = Path.GetFullPath(pathParameter);
                Linter.Run(path, Program.DetectTempDir(tempDir, path, "lint"), quiet, summary);
            });
        }
    }
}