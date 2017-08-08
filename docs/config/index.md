MarkDocs configuration
======================

Use `.AddMarkDocs()` method to configure MarkDocs:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // ...

    services.AddMarkDocs(config =>
    {
        // Add configuration code here
    });

    // ...
}
```

:::toc:::

Required services
-----------------

### Documentation markup format

Use `config.Format.UseXXX()` to configure MarkDocs to use specified markup format, e.g. [MarkDown](https://en.wikipedia.org/wiki/Markdown):

```csharp
config.Format.UseMarkdown(ResourceUrlResolverFactory);

// ResourceUrlResolverFactory here is a function like:
static IResourceUrlResolver ResourceUrlResolverFactory(IServiceProvider services)
{
    return new MyResourceUrlResolver();
}
```

Markdown format requires an external `IResourceUrlResolver` service to resolve hyperlinks.

### Cache service

Use `config.Cache.UseXXX()` to configure MarkDocs to use specified content cache implementation.

The following cache implementations are available:

* [DiskCache](disk-cache.md)
* [NullCache](null-cache.md)

Normally you should use [DiskCache](disk-cache.md).


### Storage provider

Use `config.Storage.UseXXX()` to configure MarkDocs to use specified content source implementation.

The following content sources are available:

* [StaticDirectoryStorage](static-storage.md)
* [GitStorage](git-storage.md)

Optional services
-----------------

### Page tags

Use the following code to enable Tags Extension:

```csharp
config.Extensions.AddTags();
```

### Text search

Use the following code to enable Tags Extension:

```csharp
config.Extensions.AddSearch("path-to-index-dir");
```