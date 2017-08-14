function run($file) {
    & $file
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'$file' exited with $LASTEXITCODE"
        exit $LASTEXITCODE
    }
}

run ./build-version.ps1
run ./build-clean.ps1
run ./build-restore.ps1
run ./build-packages.ps1
run ./build-tools.ps1