$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"
$VERSION = $env:MARKDOCS_BUILD_VERSION
if (!$VERSION) {
    $VERSION = "0.0.0-dev"
}

$CHOCO_API_KEY = $env:CHOCO_API_KEY
if(!$CHOCO_API_KEY) {
    Write-Host "`$CHOCO_API_KEY is not set" -f yellow
    return
}

$package ="markdocs.$VERSION.nupkg"
$path = Join-Path $ARTIFACTS $package
if(-not (Test-Path $path)) {
    Write-Host "Package file `"$path`" doesn't exist"
    exit 1
}

Write-host "Pushing package $package"

choco push $path --apikey=$CHOCO_API_KEY -v --source="https://push.chocolatey.org/"
if ($LASTEXITCODE -ne 0) {
    Write-Host "'choco push' exited with $LASTEXITCODE"
    exit 1
}

Write-Host "Success"