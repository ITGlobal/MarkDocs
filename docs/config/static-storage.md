Static directory storage
========================

`StaticDirectoryStorage` uses a directory as a source of documentation.

Use the following code to enable `StaticDirectoryStorage`:

```csharp
config.Storage.UseStaticDirectory(directoryPath);
// or
config.Storage.UseStaticDirectory(directoryPath, enableWatch: false, useSubdirectories: false);
// or
config.Storage.UseStaticDirectory(new StaticStorageOptions
{
    Directory         = directoryPath,
    EnableWatch       = false,
    UseSubdirectories = false
});
```

Parameters
----------

+---------------------+-------------------------------------------+------------------+
| Parameter           | Description                               | Remarks          |
+=====================+===========================================+==================+
| `Directory`         | Path to root of content directory         | Required         |
+---------------------+-------------------------------------------+------------------+
| `EnableWatch`       | Enables watching for file chaches         | false by default |
+---------------------+-------------------------------------------+------------------+
| `UseSubdirectories` | Enable multi-version documentation source | false by default |
+---------------------+-------------------------------------------+------------------+

### UseSubdirectories

If `UseSubdirectories` parameter is set to `true` then `StaticDirectoryStorage` will generate a separate documentation branch for each subdirectory of `Directory`:

```
/Directory/Branch_1 -> Branch_1
/Directory/Branch_2 -> Branch_2
/Directory/Branch_3 -> Branch_3
```

Otherwise only one branch will be produced from root of `Directory`:

```
/Directory -> Directory
```

### EnableWatch

If `EnableWatch` parameter is set to `true` then `StaticDirectoryStorage` will try to monitor file changes in `Directory` and trigger documentation refresh.