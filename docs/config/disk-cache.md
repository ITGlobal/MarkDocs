Disk cache
==========

`DiskCache` is a production-ready cache implementation that stores content in a specified disk directory.

Use the following code to enable `DiskCache`:

```csharp
config.Cache.UseDisk(directoryPath);
// or
config.Cache.UseDisk(directoryPath, enableConcurrentWrites: true);
// or
config.Cache.UseDisk(new DiskCacheOptions
{
    Directory              = directoryPath,
    EnableConcurrentWrites = true
});
```

Parameters
----------

+--------------------------+-----------------------------------------+-----------------+
| Parameter                | Description                             | Remarks         |
+==========================+=========================================+=================+
| `directoryPath`          | Path to root of cache directory         | Required        |
+--------------------------+-----------------------------------------+-----------------+
| `enableConcurrentWrites` | Enables parallel cache write operations | true by default |
+--------------------------+-----------------------------------------+-----------------+