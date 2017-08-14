function run($file) {
    & $file
    if ($LASTEXITCODE -ne 0) {
        Write-Host "'$file' exited with $LASTEXITCODE"
        exit $LASTEXITCODE
    }
}

run ./push-nuget.ps1
run ./push-choco.ps1