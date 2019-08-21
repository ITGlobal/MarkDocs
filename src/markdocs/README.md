# markdocs

A command line tool for MarkDocs.

## Install

Execute command:

```shell
dotnet tool install -g ITGlobal.MarkDocs.CommandLineTool
```

Then run:

```shell
markdocs [command]
```

## Commands

* `markdocs lint` - runs a linter over a directory
* `markdocs build` - generates a static HTML website from a directory
* `markdocs serve` - serves a website from a directory (updating on the fly!)
* `markdocs serve-git` - serves a website from a git repository (updating on the fly!)

Try `markdocs --help` or `markdocs <command> --help` to find out more.

## Docker image

Commands `markdocs serve`  and `markdocs serve-git` are also available
as [a Dockerized application](https://hub.docker.com/r/itglobal/markdocs).
