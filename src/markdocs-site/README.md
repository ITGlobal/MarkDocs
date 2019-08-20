# markdocs-site

A standalone web site that runs MarkDocs.

## How to run

```shell
docker run -d [options...] itglobal/markdocs-site:latest
```

### Serve documentation from file system

Use the following command:

```shell
# Assume that documentation source is placed in /doc/source
docker run -d --name markdocs \
  --restart always \
  -v /doc/source:/docs \
  -v $(pwd)/cache:/cache
  -e DOC_SOURCE_DIR=/docs \
  -p 8000:8000 \
  itglobal/markdocs-site:latest
```

### Serve documentation from git

```shell
docker run -d --name markdocs \
  --restart always \
  -v $(pwd)/cache:/cache
  -e DOC_GIT_FETCH_URL=$GIT_URL \
  -e DOC_GIT_USERNAME=$GIT_USERNAME \
  -e DOC_GIT_PASSWORD=$GIT_PASSWORD \
  -e DOC_GIT_BRANCH=$GIT_BRANCH \
  -p 8000:8000 \
  itglobal/markdocs-site:latest
```

## Configuration

Markdocs-Site is configured via environment variables:

| Variable                | Default value            | Description                                       |
|-------------------------|--------------------------|---------------------------------------------------|
| `LISTEN_URL`            | `http://0.0.0.0:8000`    | HTTP endpoint to listen                           |
| `PUBLIC_URL`            | `http://localhost:$port` | Public root URL                                   |
| `TEMP_DIR`              | `/cache`                 | Path to cache directory                           |
| `ENABLE_DEV_MODE`       |                          | Enables developer mode                            |
| `LOG_LEVEL`             | `DEBUG`                  | Configures logging level                          |
| `DOC_SOURCE_DIR`        |                          | Path to documentation source directory            |
| `DOC_GIT_FETCH_URL`     |                          | URL to git repository with  documentation sources |
| `DOC_GIT_USERNAME`      |                          | Username for git repository                       |
| `DOC_GIT_PASSWORD`      |                          | Password for git repository                       |
| `DOC_GIT_BRANCH`        | `master`                 | Branch for git repository                         |
| `DOC_GIT_POLL_INTERVAL` | `300`                    | Polling interval for git repository               |
