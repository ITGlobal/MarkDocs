Git storage
===========

`GitStorage` uses a git repository as a source of documentation. It fetches all remote branches and tags (according to configured rules) and generates a documentation branch from each matching git tag and branch.

Use the following code to enable `GitStorage`:

```csharp
config.Storage.UseGit(new GitStorageOptions
{
    FetchUrl        = fetchUrl,
    Directory       = checkoutDirectory,
    UserName        = username,
    Password        = password,
    Branches        =
    {
        Use     = true,
        Include = new [] { "release/*" },
        Exclude = new [] { "*fix*" },
    },
    Tags            =
    {
        Use     = true,
        Include = new [] { "v*" },
        Exclude = new [] { "*fix*" },
    },
    EnablePolling   = true,
    PollingInterval = TimeSpan.FromMinutes(5)
});

// or

config.Storage.UseGit(SetupFunction);

```

Parameters
----------

+-------------------+--------------------------------------+----------------------------------+
| Parameter         | Description                          | Remarks                          |
+===================+======================================+==================================+
| `FetchUrl`        | Git repository fetch URL             | Required                         |
+-------------------+--------------------------------------+----------------------------------+
| `Directory`       | Path to root of content directory    | Required                         |
+-------------------+--------------------------------------+----------------------------------+
| `UserName`        | Git username                         |                                  |
+-------------------+--------------------------------------+----------------------------------+
| `Password`        | Git password                         |                                  |
+-------------------+--------------------------------------+----------------------------------+
| `Branches`        | Defines branch rules                 | Includes all branches by default |
+-------------------+--------------------------------------+----------------------------------+
| `Tags`            | Defines tag rules                    | Off by default                   |
+-------------------+--------------------------------------+----------------------------------+
| `EnablePolling`   | Enables periodic polling for changes | false by default                 |
+-------------------+--------------------------------------+----------------------------------+
| `PollingInterval` | Defines polling interval             | 5min by default                  |
+-------------------+--------------------------------------+----------------------------------+