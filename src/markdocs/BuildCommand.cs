using System.IO;
using ITGlobal.CommandLine.Parsing;
using ITGlobal.MarkDocs.Tools.Generate;

namespace ITGlobal.MarkDocs.Tools
{
    public static class BuildCommand
    {
        public static void Setup(ICliCommandRoot app, CliOption<string> tempDir, CliSwitch quiet)
        {
            var command = app.Command("build")
                .HelpText("Generate static website from documentation");

            var pathParameter = command
                .Argument("path")
                .DefaultValue(".")
                .HelpText("Path to documentation root directory")
                .Required();
            var targetDirParameter = command
                .Option('o', "output")
                .HelpText("Path to output directory");
            var templateParameter = command
                .Option('t', "template")
                .HelpText("Template name");

            command.OnExecute(ctx =>
            {
                var path = Path.GetFullPath(pathParameter.Value);
                var outputPath = targetDirParameter.IsSet
                    ? targetDirParameter.Value
                    : Path.Combine(path, "__output__");
                var templateName = templateParameter.IsSet
                    ? templateParameter.Value
                    : "";

                var exitCode = Generator.Run(
                    sourceDir: path,
                    targetDir: outputPath,
                    templateName: templateName,
                    tempDir: Program.DetectTempDir(tempDir, "gen", path),
                    quiet: quiet
                );
                ctx.Break(exitCode);
            });
        }
    }
}