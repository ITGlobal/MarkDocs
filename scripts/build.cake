#addin Cake.DoInDirectory
#addin Cake.FileHelpers
#addin Cake.SemVer
#tool GitVersion.CommandLine
#addin Newtonsoft.Json

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var SOLUTION_DIR = MakeAbsolute(Directory("../"));
DoInDirectory(SOLUTION_DIR, () =>
{
  var DIR_ARTIFACTS = MakeAbsolute(SOLUTION_DIR.Combine("./artifacts"));
  var CONFIGURATION = Argument("configuration", "Release");
  var VERSION = "0.0.0-dev";

  Task("init")
    .Does(() => 
    {
        Information("SolutionDir: {0}", SOLUTION_DIR);
        EnsureDirectoryExists(DIR_ARTIFACTS);
    });

  Task("clean")
    .IsDependentOn("init")
    .Does(() => 
    {
        CleanDirectory(DIR_ARTIFACTS);

        foreach(var dir in GetDirectories("**/bin")) {
          CleanDirectory(dir);
        }

        foreach(var dir in GetDirectories("**/obj")) {
          CleanDirectory(dir);
        }
    });

  Task("version")
    .Does(() => 
    {
        Debug("Fetching version information from git");
        var gitVersion = GitVersion();
        Debug("GitVersion = {0}", JsonConvert.SerializeObject(gitVersion, Formatting.Indented));

        Debug("Detecting build number");
        int buildNumber = 0;
        if(AppVeyor.IsRunningOnAppVeyor)
        {
          buildNumber = AppVeyor.Environment.Build.Number;
          Debug("Got build number #{0} from AppVeyor", buildNumber);
        }
        else if(TeamCity.IsRunningOnTeamCity)
        {
          buildNumber = int.Parse(EnvironmentVariable("BUILD_NUMBER"));
          Debug("Got build number #{0} from TeamCity", buildNumber);
        }
        else if (HasEnvironmentVariable("BUILD_NUMBER") && int.TryParse(EnvironmentVariable("BUILD_NUMBER"), out buildNumber))
        {          
          Debug("Got build number #{0} from $BUILD_NUMBER", buildNumber);
        }
        else 
        {
          buildNumber = 0;
          Debug("Got build number #{0} (default value)", buildNumber);
        }

        Debug("Detecting branch name");
        var branchName = gitVersion.BranchName;
        branchName = branchName.Replace("/", "").Replace("-", "").Replace("\\", "");
        
        var sv = branchName != "master" 
          ? CreateSemVer(gitVersion.Major, gitVersion.Minor, buildNumber, branchName)
          : CreateSemVer(gitVersion.Major, gitVersion.Minor, buildNumber);
        VERSION = sv.ToString();
        if(AppVeyor.IsRunningOnAppVeyor)
        {
          AppVeyor.UpdateBuildVersion(VERSION);
        }
        
        if(TeamCity.IsRunningOnTeamCity)
        {
          TeamCity.SetBuildNumber(VERSION);
        }

        Information("Version: {0}", VERSION);
    });

  Task("restore")
    .IsDependentOn("init")
    .IsDependentOn("clean")
    .IsDependentOn("version")
    .Does(() => 
    {
        Exec("dotnet", "restore", "/v:q", "/nologo", $"/p:Version=\"{VERSION}\"");
    });

  Task("compile")
    .IsDependentOn("init")
    .IsDependentOn("restore")
    .IsDependentOn("version")
    .Does(() => 
    {
        Exec("dotnet", "build", "/nologo", "/v:q", $"/p:Configuration={CONFIGURATION}", $"/p:Version=\"{VERSION}\"");
    });  

  Task("packages")
    .IsDependentOn("init")
    .IsDependentOn("clean")
    .IsDependentOn("version")
    .IsDependentOn("compile")
    .Does(() => 
    {
        var projects = GetFiles("./src/**/*.csproj");
        foreach(var project in projects)
        {
           Information("Packaging {0}", System.IO.Path.GetFileNameWithoutExtension(project.FullPath));
           Exec(
             "dotnet",
             "pack",
             "--output",
             DIR_ARTIFACTS.FullPath,
             "/nologo", 
             "-v", 
             "q",
             "/v:q",
             $"/p:Configuration={CONFIGURATION}", 
             $"/p:Version=\"{VERSION}\"",
             project.FullPath);
        }
    });

Task("tools")
    .IsDependentOn("init")
    .IsDependentOn("clean")
    .IsDependentOn("version")
    .IsDependentOn("compile")
    .Does(() => 
    {
        var targets = new[] { Tuple.Create("windows", "win7-x64"), Tuple.Create("linux", "ubuntu.14.04-x64") };
        foreach(var t in targets)
        {
          var platform = t.Item1;
          var rid = t.Item2;

          Information($"Packaging {{0}} ({platform})", "markdocs-gen");
          Exec("dotnet", "restore", "--runtime", rid, "./samples/markdocs-gen");
          var binDir = System.IO.Path.Combine(DIR_ARTIFACTS.FullPath, $"markdocs-gen-{platform}");
          Exec(
            "dotnet",
            "publish",
            "--runtime",
            rid,
            "--output",
            binDir,
            "/nologo", 
            "-v", 
            "q",
            "/v:q",
            $"/p:Configuration={CONFIGURATION}", 
            $"/p:Version=\"{VERSION}\"",
            "./samples/markdocs-gen");
          var zipFileName = System.IO.Path.Combine(DIR_ARTIFACTS.FullPath, $"markdocs-gen-{platform}-{VERSION}.zip");
          Zip(binDir, zipFileName);
        }
    });

Task("pack")
    .IsDependentOn("packages")
    .IsDependentOn("tools")
    .Does(() => 
    {
        foreach(var artifact in GetFiles(DIR_ARTIFACTS + "/*.nupkg"))
        {
            Information("\t{0}", artifact.GetFilename());
            if(AppVeyor.IsRunningOnAppVeyor)
            {
              AppVeyor.UploadArtifact(artifact);
            }
        }
        foreach(var artifact in GetFiles(DIR_ARTIFACTS + "/*.zip"))
        {
            Information("\t{0}", artifact.GetFilename());
            if(AppVeyor.IsRunningOnAppVeyor)
            {
              AppVeyor.UploadArtifact(artifact);
            }
        }
    });


  Task("default")
    .IsDependentOn("pack")
    .Does(() =>
  { });

  var target = Argument("target", "default");
  RunTarget(target);
});

void Exec(string program, params string[] args)
{
  var commandLine = string.Join(" ", args.Select(arg => arg.Any(char.IsWhiteSpace) ? "\"" + arg + "\"" : arg));
  Debug("exec: {0} {1}", program, commandLine);
  var exitCode = StartProcess(program, commandLine);  
  if(exitCode != 0) 
  {
    Error("Command [{0} {1}] failed (exitcode={2})", program, commandLine, exitCode);
    throw new Exception($"Command [{program} {commandLine}] failed (exitcode={exitCode})");
  }
}