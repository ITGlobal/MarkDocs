#addin Cake.DoInDirectory
#addin Cake.Json

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
        var version = "0.0.0-dev";
        if(AppVeyor.IsRunningOnAppVeyor)
        {
             version = AppVeyor.Environment.Build.Version;
        }

        Information("Version = {0}", version);
        
        // TODO patch project files
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

        Information("Produced artifacts:");
        foreach(var artifact in GetFiles(artifactsDir + "/*.nupkg"))
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