if($args.Length -ne 1) {
    echo "Wrong arguments!"
    echo "Usage:"
    echo "./markdocs-serve.ps1 <path-to-directory>"
    exit 1
}

$env:DOC_SOURCE_DIR = $args[0]
$env:ENABLE_DEV_MODE = "Y"
dotnet run --no-launch-profile --project ./src/markdocs-site/markdocs-site.csproj