FROM ubuntu:14.04

ADD bin/Release/netcoreapp1.6/ubuntu.14.04-x64/publish /app

RUN chmod +x /app/markdocs-gen
VOLUME /src
VOLUME /dest

ENTRYPOINT ["/app/markdocs-gen", "/src", "/dest"]