$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"
$CONFIGURATION = "Release"
$VERSION = "0.0.0-dev"

# init
if(-not (Test-Path $ARTIFACTS)) {
    New-Item -Path $ARTIFACTS -ItemType Directory | Out-Null    
}

# clean
Get-ChildItem -Path $ARTIFACTS | Remove-Item -Force -Recurse
Get-ChildItem -Filter "bin" -Directory -Recurse | Remove-Item -Force -Recurse
Get-ChildItem -Filter "obj" -Directory -Recurse | Remove-Item -Force -Recurse

# version
$gitVersion = $(git tag) | ? { $_ -match "v([0-9]+\.[0-9]+)" } | Select -Last 1 | % { $_.Substring(1) }
$buildNumber = 0
if($env:APPVEYOR) {
    $buildNumber = $env:APPVEYOR_BUILD_NUMBER
    write-host "APPVEYOR_BUILD_NUMBER: $env:APPVEYOR_BUILD_NUMBER"
}
if($env:APPVEYOR) {
    if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
        $branch = "pullrequest-" + $env:APPVEYOR_PULL_REQUEST_NUMBER
    } else {
        $branch = $env:APPVEYOR_REPO_BRANCH
        write-host "APPVEYOR_REPO_BRANCH: $env:APPVEYOR_REPO_BRANCH"
    }    
} else {
    $branch = $(git rev-parse --abbrev-ref HEAD)
}
$branch = $branch.Replace("/", "").Replace("-", "").Replace("\\", "")

$VERSION = $gitVersion + "." + $buildNumber
if($branch -ne "master") {
    $VERSION += "-$branch"
}

if($env:APPVEYOR) {
    & appveyor UpdateBuild -Version $VERSION
}

Write-Host "Version: $VERSION"

# restore
& dotnet restore /v:q /nologo /p:Version=$VERSION
if($LASTEXITCODE -ne 0) {
    Write-Host "'dotnet restore' exited with $LASTEXITCODE"
    exit 1
}

# packages

Get-ChildItem -Filter *.csproj -Path "./src" -Recurse -File | %{ 
    & dotnet pack --output $ARTIFACTS -v q /nologo /p:Version=$VERSION /p:Configuration=$CONFIGURATION $_.FullName
    if($LASTEXITCODE -ne 0) {
        Write-Host "'dotnet pack' exited with $LASTEXITCODE"
        exit 1
    }

    Write-Host "Compiled package $($_.Name)"
}