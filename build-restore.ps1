$VERSION = $env:MARKDOCS_BUILD_VERSION
if(!$VERSION) {
    $VERSION = "0.0.0-dev"
}

& dotnet restore /v:q /nologo /p:Version=$VERSION
if($LASTEXITCODE -ne 0) {
    Write-Host "'dotnet restore' exited with $LASTEXITCODE"
    exit 1
}