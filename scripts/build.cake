#addin Cake.DoInDirectory
#addin Cake.FileHelpers
#addin Cake.SemVer
#tool GitVersion.CommandLine
#addin Newtonsoft.Json

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var solutionDir = MakeAbsolute(Directory("../"));
DoInDirectory(solutionDir, () =>
{
  var artifactsDir = MakeAbsolute(solutionDir.Combine("./artifacts"));
  var srcDir = MakeAbsolute(solutionDir.Combine("./src"));
  var samplesDir = MakeAbsolute(solutionDir.Combine("./samples"));

  var libraryProjects = GetFiles(srcDir + "/**/project.json");
  var sampleProjects = GetFiles(samplesDir + "/**/project.json");
  var projects = GetFiles(solutionDir + "/**/project.json");

  var configuration = Argument("configuration", "Release");

  Task("init")
    .Does(() => 
    {
        Information("{0} = \"{1}\"", nameof(solutionDir), solutionDir);
        EnsureDirectoryExists(artifactsDir);
    });

  Task("clean")
    .IsDependentOn("init")
    .Does(() => 
    {
        CleanDirectory(artifactsDir);
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
        var version = sv.ToString();
        if(AppVeyor.IsRunningOnAppVeyor)
        {
          AppVeyor.UpdateBuildVersion(version);
        }
        
        if(TeamCity.IsRunningOnTeamCity)
        {
          TeamCity.SetBuildNumber(version);
        }

        Information("Version = {0}", version);

        if(AppVeyor.IsRunningOnAppVeyor || TeamCity.IsRunningOnTeamCity)
        {
          foreach(var project in projects) 
          {
            Debug("Patching project {0}", project.GetDirectory().GetDirectoryName());
            var raw = FileReadText(proje​ct);
            var json = JToken.Parse(raw);
            json["version"] = version;
            foreach(JProperty p in json["dependencies"])
            {
              if(p.Name.StartsWith("ITGlobal.MarkDocs"))
              {
                p.Value = version;
              }            
            }
            FileWriteText(project, json.ToString());
          }
        }
    });

  Task("restore")
    .IsDependentOn("init")
    .Does(() => 
    {
        foreach(var project in projects) 
        {
          Information("Restoring packages for {0}", project.GetDirectory().GetDirectoryName());
          Exec("dotnet", "restore", project.FullPath);
        }    
    });

  Task("compile")
    .IsDependentOn("init")
    .IsDependentOn("restore")
    .IsDependentOn("version")
    .Does(() => 
    {
        foreach(var project in projects) 
        {
          Information("Compiling {0} ({1})", project.GetDirectory().GetDirectoryName(), configuration);
          Exec("dotnet", "build", project.FullPath, "-c" , configuration);
        }    
    });  

  Task("pack")
    .IsDependentOn("init")
    .IsDependentOn("version")
    .IsDependentOn("compile")
    .Does(() => 
    {
        foreach(var project in libraryProjects) 
        {
          Information("Packing {0}", project.GetDirectory().GetDirectoryName());
          Exec("dotnet", "pack", project.FullPath, "-o" , artifactsDir.FullPath);
        }

        var dirName = System.IO.Path.Combine(artifactsDir.FullPath, "markdocs-gen");        
        Exec("dotnet", "publish", "./samples/markdocs-gen", "-c", "Release", "-r" ,"win7-x64", "-o", dirName);
        var zipFileName = System.IO.Path.Combine(artifactsDir.FullPath, "markdocs-gen.zip");
        Zip(dirName, zipFileName);
        if(AppVeyor.IsRunningOnAppVeyor)
        {
          AppVeyor.UploadArtifact(System.IO.Path.GetFullPath(zipFileName));
        }

        Information("Produced artifacts:");
        foreach(var artifact in GetFiles(artifactsDir + "/*.nupkg"))
        {
            Information("\t{0}", artifact.GetFilename());
        }
        foreach(var artifact in GetFiles(artifactsDir + "/*.zip"))
        {
            Information("\t{0}", artifact.GetFilename());
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