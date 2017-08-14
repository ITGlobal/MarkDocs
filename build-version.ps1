$gitVersion = $(git tag) | ? { $_ -match "v([0-9]+\.[0-9]+)" } | % { 
    $m = [regex]::Match($_, "v([0-9]+)\.([0-9]+)")
    $i = [int]($m.Groups[1].Value)
    $j = [int]($m.Groups[2].Value)
    return $i * 1000 + $j
} | Sort -Descending | Select -First 1
$verMajor = [int]([math]::Round($gitVersion / 1000))
$verMinor = $gitVersion % 1000

$buildNumber = 0
if ($env:APPVEYOR) {
    $buildNumber = $env:APPVEYOR_BUILD_NUMBER
    write-host "APPVEYOR_BUILD_NUMBER: $env:APPVEYOR_BUILD_NUMBER"
}
if ($env:APPVEYOR) {
    if ($env:APPVEYOR_PULL_REQUEST_NUMBER) {
        $branch = "pullrequest-" + $env:APPVEYOR_PULL_REQUEST_NUMBER
    }
    else {
        $branch = $env:APPVEYOR_REPO_BRANCH
        write-host "APPVEYOR_REPO_BRANCH: $env:APPVEYOR_REPO_BRANCH"
    }    
}
else {
    $branch = $(git rev-parse --abbrev-ref HEAD)
}
$branch = $branch.Replace("/", "").Replace("-", "").Replace("\\", "")

$VERSION = "$($verMajor).$($verMinor).$($buildNumber)"
if ($branch -ne "master") {
    $VERSION += "-$branch"
}

if ($env:APPVEYOR) {
    & appveyor UpdateBuild -Version $VERSION
}

Write-Host "Version: $VERSION"

$env:MARKDOCS_BUILD_VERSION = $VERSION