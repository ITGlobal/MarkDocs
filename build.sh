SCRIPT="./build/build.cake"
TARGET="default"
CONFIGURATION="Release"
VERBOSITY="normal"
DRYRUN=
SHOW_VERSION=false
SCRIPT_ARGUMENTS=()

# Parse arguments.
for i in "$@"; do
    case $1 in
        -t|--target) TARGET="$2"; shift ;;
        -c|--configuration) CONFIGURATION="$2"; shift ;;
        -v|--verbose) VERBOSITY="verbose"; shift ;;
        -d|--dryrun) DRYRUN="-dryrun" ;;
    esac
    shift
done

./scripts/build.sh -script ./scripts/build.cake -target $TARGET -configuration $CONFIGURATION -verbosity $VERBOSITY --dryrun $DRYRUN $SCRIPT_ARGUMENTS