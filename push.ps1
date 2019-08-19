function push-nuget() {
    $SOLUTION_DIR = Resolve-Path "."
    $ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts/nuget"
    
    $NUGET_API_KEY = $env:NUGET_API_KEY
    if (!$NUGET_API_KEY) {
        Write-Host "`$NUGET_API_KEY is not set" -f yellow
        return
    }
    
    Get-ChildItem -Path $ARTIFACTS -Filter *.nupkg -File | % {
        $package = $_.FullName
        Write-Host "Pushing package $package"
        & dotnet nuget push --api-key $NUGET_API_KEY --force-english-output -s https://www.nuget.org/api/v2/package $package
        if ($LASTEXITCODE -ne 0) {
            Write-Host "'dotnet nuget push' exited with $LASTEXITCODE"
            exit 1
        }
    }
}

function push-choco() {
    $SOLUTION_DIR = Resolve-Path "."
    $ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts/choco"
    $VERSION = $env:MARKDOCS_BUILD_VERSION
    if (!$VERSION) {
        $VERSION = "0.0.0-dev"
    }
    
    $package = "markdocs.$VERSION.nupkg"
    $path = Join-Path $ARTIFACTS $package
    if (-not (Test-Path $path)) {
        Write-Host "Package file `"$path`" doesn't exist"
        exit 1
    }
    
    function push-once($url, $apiKey, $isEnabled, $type) {
        if (-not ([string]::IsNullOrEmpty($url)) -and -not ([string]::IsNullOrEmpty($apiKey)) -and ([int]$isEnabled -ne 0)) {
            Write-host "Pushing package $package to $type..."
            
            choco push $path --apikey=$apiKey -v --source="$url"
            if ($LASTEXITCODE -ne 0) {
                Write-Host "'choco push' exited with $LASTEXITCODE"
                exit 1
            }
            
            Write-Host "Success"
        }
        else {
            Write-host "Won't push package to $type"
        }  
    }
    
    # push-once "https://push.chocolatey.org/" $env:CHOCO_API_KEY $env:CHOCO_PUSH_PUBLIC "chocolatey.org"
    push-once $env:CHOCO_PRIVATE_URL $env:CHOCO_PRIVATE_API_KEY $env:CHOCO_PUSH_PRIVATE "private chocolatey feed"
}

function push-usage() {
    write-host "USAGE:"
    write-host " ./push.ps1 [<command>]"
    write-host "COMMANDS:"
    write-host "  nuget     - publish NuGet packages"
    write-host "  choco     - publish CLI tools"
    write-host "  all       - publish everything"
    write-host "  -?|-h|--help|help"
}

if ($args.Length -gt 0) {
    switch ($args[0]) {
        "nuget" { push-nuget }
        "choco" { push-choco }
        "all" { 
            push-nuget
            push-choco
        }
        "-?" { push-usage }
        "-h" { push-usage }
        "--help" { push-usage }
        "help" { push-usage }
        Default {
            write-host "Invalid arguments: `"$args`"" -f red
            write-host "Type `"./push.ps1 --help`" to get help"
            exit 1
        }
    }
}
else {
    write-host "Type `"./push.ps1 --help`" to get help"
    exit 1
}