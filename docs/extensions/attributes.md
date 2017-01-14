---
title: HTML attributes
order: 2
---

HTML attributes
===============

You can use the following markup to place custom HTML attributes on specific elements.

Markup
------

```markdown

Use the `#` prefix to set `id` attribute:

This H1 element will have "id"="header1" {#header1}
======================================

Use the `.` prefix to set CSS classes:

[This link will look like a button](../) {.btn .btn-default}

## This H2 element will have attribute "lang"="fr" {lang=fr}

```

Results
-------

:::well

Use the `#` prefix to set `id` attribute:

This H1 element will have "id"="header1" {#header1}
======================================

Use the `.` prefix to set CSS classes:

[This link will look like a button](../){.btn .btn-default}

This paragraph will have a `alert alert-info` classes {.alert .alert-info}

## This H2 element will have attribute "lang"="fr" {lang=fr}


:::