$SOLUTION_DIR = Resolve-Path "."
$ARTIFACTS = Join-Path $SOLUTION_DIR "artifacts"

if(-not (Test-Path $ARTIFACTS)) {
    New-Item -Path $ARTIFACTS -ItemType Directory | Out-Null    
}

Get-ChildItem -Path $ARTIFACTS | Remove-Item -Force -Recurse
Get-ChildItem -Filter "bin" -Directory -Recurse | Remove-Item -Force -Recurse
Get-ChildItem -Filter "obj" -Directory -Recurse | Remove-Item -Force -Recurse