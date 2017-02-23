---
title: Content
---

# Documentation Content

Documentation content is generated from file tree with the following rules:

* Any `index.md` or `README.md` file is an index page (default page for a directory)
* All other `*.md` files are pages
* All other files are attachments
* Files and directories are ordered according to page metadata, which can be defines using any of methods:
  * [Page properites block](/markup/properties.md) in the beginning of a page
  * [_toc.json](toc.md) file in a directory
* Directories and files can be excluded from documentation with help of [`.mdignore`](mdignore.md) file