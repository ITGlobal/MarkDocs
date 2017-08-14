$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"
$CONFIGURATION = "Release"
$VERSION = $env:MARKDOCS_BUILD_VERSION
if (!$VERSION) {
    $VERSION = "0.0.0-dev"
}

dotnet restore --runtime win7-x64 ./src/markdocs/markdocs.csproj /nologo /p:Version=$VERSION
if ($LASTEXITCODE -ne 0) {
    Write-Host "'dotnet restore' exited with $LASTEXITCODE"
    exit 1
}

dotnet publish --runtime win7-x64 --configuration $CONFIGURATION -v q `
    --output $ARTIFACTS/markdocs-win7-x64 `
    ./src/markdocs/markdocs.csproj /nologo /p:Version=$VERSION
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