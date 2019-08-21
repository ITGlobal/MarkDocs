function build-version() {
    $gitVersion = $(git tag) | ? { $_ -match "v([0-9]+\.[0-9]+)" } | % { 
        $m = [regex]::Match($_, "v([0-9]+)\.([0-9]+)")
        $i = [int]($m.Groups[1].Value)
        $j = [int]($m.Groups[2].Value)
        return $i * 1000 + $j
    } | Sort -Descending | Select -First 1
    $verMajor = [int]([math]::Round($gitVersion / 1000))
    $verMinor = $gitVersion % 1000
    
    $buildNumber = 0
    if ($env:APPVEYOR) {
        $buildNumber = $env:APPVEYOR_BUILD_NUMBER
        write-host "APPVEYOR_BUILD_NUMBER: $env:APPVEYOR_BUILD_NUMBER"
    }
    if ($env:APPVEYOR) {
        if ($env:APPVEYOR_PULL_REQUEST_NUMBER) {
            $branch = "pullrequest-" + $env:APPVEYOR_PULL_REQUEST_NUMBER
        }
        else {
            $branch = $env:APPVEYOR_REPO_BRANCH
            write-host "APPVEYOR_REPO_BRANCH: $env:APPVEYOR_REPO_BRANCH"
        }    
    }
    else {
        $branch = $(git rev-parse --abbrev-ref HEAD)
    }
    $branch = $branch.Replace("/", "").Replace("-", "").Replace("\\", "")
    
    $VERSION = "$($verMajor).$($verMinor).$($buildNumber)"
    if ($branch -ne "master") {
        $VERSION += "-$branch"
    }
    
    if ($env:APPVEYOR) {
        & appveyor UpdateBuild -Version $VERSION
    }
    
    Write-Host "Version: $VERSION"
    
    $env:MARKDOCS_BUILD_VERSION = $VERSION
}

function build-clean() {
    $SOLUTION_DIR = Resolve-Path "."
    $ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"

    if (-not (Test-Path $ARTIFACTS)) {
        New-Item -Path $ARTIFACTS -ItemType Directory | Out-Null    
    }
    
    if (-not (Test-Path "$ARTIFACTS/nuget")) {
        New-Item -Path "$ARTIFACTS/nuget" -ItemType Directory | Out-Null    
    }
    
    if (-not (Test-Path "$ARTIFACTS/choco")) {
        New-Item -Path "$ARTIFACTS/choco" -ItemType Directory | Out-Null    
    }
    
    Get-ChildItem -Path $ARTIFACTS | Remove-Item -Force -Recurse
    Get-ChildItem -Filter "bin" -Directory -Recurse -ErrorAction "SilentlyContinue" | Remove-Item -Force -Recurse -ErrorAction "SilentlyContinue"
    Get-ChildItem -Filter "obj" -Directory -Recurse -ErrorAction "SilentlyContinue" | Remove-Item -Force -Recurse -ErrorAction "SilentlyContinue"
}

function build-restore() {
    $VERSION = $env:MARKDOCS_BUILD_VERSION
    if (!$VERSION) {
        $VERSION = "0.0.0-dev"
    }
    
    & dotnet restore /v:q /nologo /p:Version=$VERSION
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'dotnet restore' exited with $LASTEXITCODE"
        exit 1
    }
}

function build-packages() {
    $SOLUTION_DIR = Resolve-Path "."
    $ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts/nuget"
    $CONFIGURATION = "Release"
    $VERSION = $env:MARKDOCS_BUILD_VERSION
    if (!$VERSION) {
        $VERSION = "0.0.0-dev"
    }
    
    Get-ChildItem -Filter *.csproj -Path "./src" -Recurse -File | % {    
        & dotnet pack --output $ARTIFACTS --no-restore -v q /nologo /p:Version=$VERSION /p:Configuration=$CONFIGURATION $_.FullName
        if ($LASTEXITCODE -ne 0) {
            Write-Host "'dotnet pack' exited with $LASTEXITCODE"
            exit 1
        }
    
        Write-Host "Compiled project $($_.Name)"
    }

    Write-Host "Generated NuGet packages:"
    Get-ChildItem -Path $ARTIFACTS -Filter *.nupkg -File | %{ Write-Host "  $($_.Name)" }
}

function build-tools() {
    $SOLUTION_DIR = Resolve-Path "."
    $ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts/choco"
    $CONFIGURATION = "Release"
    $VERSION = $env:MARKDOCS_BUILD_VERSION
    if (!$VERSION) {
        $VERSION = "0.0.0-dev"
    }
    
    dotnet publish --runtime win7-x64 --configuration $CONFIGURATION -v q `
        --output $ARTIFACTS/markdocs-win7-x64 `
        ./src/markdocs/markdocs.csproj /nologo /p:Version=$VERSION /p:PackAsTool=false
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'dotnet publish' exited with $LASTEXITCODE"
        exit 1
    }
    
    copy ./tools $ARTIFACTS/markdocs-win7-x64 -rec -force
    copy ./markdocs.nuspec $ARTIFACTS/markdocs-win7-x64 -force
    
    pushd $ARTIFACTS/markdocs-win7-x64
    choco pack --version=$VERSION --outputdirectory="$ARTIFACTS"
    popd
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'choco pack' exited with $LASTEXITCODE"
        exit 1
    }
    
    Write-Host "Compiled markdocs cli tool"
    Write-Host "Generated Chocolatey packages:"
    Get-ChildItem -Path $ARTIFACTS -Filter *.nupkg -File | %{ Write-Host "  $($_.Name)" }
}

function build-usage() {
    write-host "USAGE:"
    write-host " ./build.ps1 [<command>]"
    write-host "COMMANDS:"
    write-host "  version   - generate version number"
    write-host "  clean     - clean build output"
    write-host "  restore   - restore extenal dependencies"
    write-host "  packages  - build NuGet packages"
    write-host "  tools     - build CLI tools"
    write-host "  all       - build everything"
    write-host "  -?|-h|--help|help"
}

if ($args.Length -gt 0) {
    switch ($args[0]) {
        "version" { build-version }
        "clean" { build-clean }
        "restore" { build-restore }
        "packages" { build-packages }
        "tools" { build-tools }
        "all" { 
            build-version
            build-clean
            build-restore
            build-packages
            build-tools
        }
        "-?" { build-usage }
        "-h" { build-usage }
        "--help" { build-usage }
        "help" { build-usage }
        Default {
            write-host "Invalid arguments: `"$args`"" -f red
            write-host "Type `"./build.ps1 --help`" to get help"
            exit 1
        }
    }
}
else {
    write-host "Type `"./build.ps1 --help`" to get help"
    exit 1
}