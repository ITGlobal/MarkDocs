FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as BUILDER
WORKDIR /app
COPY . /app
RUN dotnet publish -o /out -c Release -r linux-musl-x64 /app/src/markdocs/markdocs.csproj /p:PackAsTool=false /p:DisableSourceLink=true

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-alpine
RUN apk update && apk add git nodejs npm
WORKDIR /app
COPY --from=BUILDER /out /app
COPY ./src/markdocs/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh
ENV TEMP_DIR /cache
CMD [ "/docker-entrypoint.sh" ]