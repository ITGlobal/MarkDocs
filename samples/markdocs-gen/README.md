markdocs-gen
============

Static website generator based on MarkDocs

Compiling
---------

Run the following commands:

```shell
dotnet restore
dotnet publish -r ubuntu.14.04-x64 -c Release
docker build -t markdocs-gen .
```

Usage
-----

```shell
dotnet run <SRC_DIR> <TARGET_DIR> [-t <TEMPLATE>] [--watch] [--serve]
# or if you're using docker
docker run -it -v <SRC_DIR>:/src -v <TARGET_DIR>:/dest markdocs-gen [-t <TEMPLATE>] [--watch] [--serve]
# "markdocs-gen" above is a docker container name
```