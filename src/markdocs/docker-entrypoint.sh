#!/usr/bin/env sh
set -e

CMD="/app/markdocs"
if [ -n "$DOC_SOURCE_DIR" ]; then
    CMD="$CMD serve \"$DOC_SOURCE_DIR\""
elif [ -n "$DOC_GIT_FETCH_URL" ]; then
    CMD="$CMD serve-git \"$DOC_GIT_FETCH_URL\""

    if [ -z "$DOC_GIT_USERNAME" ]; then
        CMD="$CMD --username \"$DOC_GIT_USERNAME\""
    fi

    if [ -z "$DOC_GIT_PASSWORD" ]; then
        CMD="$CMD --password \"$DOC_GIT_PASSWORD\""
    fi

    if [ -z "$DOC_GIT_BRANCH" ]; then
        CMD="$CMD --branch \"$DOC_GIT_BRANCH\""
    fi

    if [ -z "$DOC_GIT_POLL_INTERVAL" ]; then
        CMD="$CMD --poll-interval \"$DOC_GIT_POLL_INTERVAL\""
    fi
else
    echo "Neither \$DOC_SOURCE_DIR nor \$DOC_GIT_FETCH_URL variables are set"
    exit 1
fi

if [ -z "$CACHE_DIR" ]; then
    CACHE_DIR=/tmp/markdocs
fi
CMD="$CMD --cache \"$CACHE_DIR\""

if [ -n "$LISTEN_URL" ]; then
    CMD="$CMD --listen-url \"$LISTEN_URL\""
fi

if [ -n "$PUBLIC_URL" ]; then
    CMD="$CMD --public-url \"$PUBLIC_URL\""
fi

if [ -n "$THEME" ]; then
    CMD="$CMD --theme \"$THEME\""
fi

case "$DEV_MODE" in
Y | y | 1 | yes | YES | true | TRUE | on | ON)
    CMD="$CMD --dev"
    ;;
esac

case "$VERBOSE" in
Y | y | 1 | yes | YES | true | TRUE | on | ON)
    CMD="$CMD -vv"
    ;;
esac

echo "+ $CMD"
sh -c "$CMD"
