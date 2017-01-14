---
title: MDIgnore file
order: 20
---

MDIgnore file
==============

Place a `.mdignore` file in a directory to define content exclusion rules:

```gitignore
# Ignore
ignored/**/*

# Don't ignore
!**/not-ignored.md
```

* Any line starting with `#` is a comment
* Any line  **not** starting with `!` is an **exclusion** rule
* Any line starting with `!` is a **force inclusion** rule