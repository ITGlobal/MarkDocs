$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"
$CONFIGURATION = "Release"
$VERSION = $env:MARKDOCS_BUILD_VERSION
if (!$VERSION) {
    $VERSION = "0.0.0-dev"
}

Get-ChildItem -Filter *.csproj -Path "./src" -Recurse -File | % {
    if ($_.Name -eq "markdocs.csproj") {
        return
    }

    & dotnet pack --output $ARTIFACTS -v q /nologo /p:Version=$VERSION /p:Configuration=$CONFIGURATION $_.FullName
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'dotnet pack' exited with $LASTEXITCODE"
        exit 1
    }

    Write-Host "Compiled package $($_.Name)"
}