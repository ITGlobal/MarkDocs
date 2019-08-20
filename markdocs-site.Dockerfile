FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as BUILDER
WORKDIR /app
COPY . /app
RUN dotnet publish -o /out -r linux-musl-x64 /app/src/markdocs-site/markdocs-site.csproj

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-alpine
RUN apk update && apk add git nodejs npm
WORKDIR /app
COPY --from=BUILDER /out /app
ENV TEMP_DIR /cache
CMD [ "/app/markdocs-site" ]