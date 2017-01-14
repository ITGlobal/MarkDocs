---
title: TOC metadata file
order: 20
---

TOC metadata file
=================

Place a `_toc.json` file in a directory to define page metadata:

```json
{
    "pages": {
        "todo": {
            "order": 10000,
            "title": "TODO items",
            "tags" : [
                "todo",
                "markdocs"
            ]
        },
        "index": {
            "order": 1,
            "title": "Index page",
            "tags" : [
                "todo",
                "index",
                "markdocs"
            ]
        }
    }
}
```