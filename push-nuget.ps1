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
    & dotnet nuget push --api-key $NUGET_API_KEY --force-english-output -s nuget.org $package
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'dotnet nuget push' exited with $LASTEXITCODE"
        exit 1
    }
}