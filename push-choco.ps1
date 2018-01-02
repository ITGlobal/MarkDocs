$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts/choco"
$VERSION = $env:MARKDOCS_BUILD_VERSION
if (!$VERSION) {
    $VERSION = "0.0.0-dev"
}

$package ="markdocs.$VERSION.nupkg"
$path = Join-Path $ARTIFACTS $package
if(-not (Test-Path $path)) {
    Write-Host "Package file `"$path`" doesn't exist"
    exit 1
}

function push-once($url, $apiKey, $isEnabled, $type) {
    if(-not ([string]::IsNullOrEmpty($url)) -and -not ([string]::IsNullOrEmpty($apiKey)) -and ([int]$isEnabled -ne 0)) {
        Write-host "Pushing package $package to $type..."
        
        choco push $path --apikey=$apiKey -v --source="$url"
        if ($LASTEXITCODE -ne 0) {
            Write-Host "'choco push' exited with $LASTEXITCODE"
            exit 1
        }
        
        Write-Host "Success"
    } else {
        Write-host "Won't push package to $type"
    }  
}

# push-once "https://push.chocolatey.org/" $env:CHOCO_API_KEY $env:CHOCO_PUSH_PUBLIC "chocolatey.org"
push-once $env:CHOCO_PRIVATE_URL $env:CHOCO_PRIVATE_API_KEY $env:CHOCO_PUSH_PRIVATE "private chocolatey feed"
