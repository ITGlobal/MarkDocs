---
title: Page properties
order: 1
tags:
  - tag1
  - tag2
  - tag3
---

Page property block
===================

Add the following markup in the beginning of the page markup to define page properties:

```yaml

---
property1: Value of property 1
property2: Value of property 2
property3: Value of property 3
---

```

Known properties
----------------

+---------------+--------------------+-----------------------------------------+
| Property      | Type               | Description                             |
+===============+====================+=========================================+
| `id`          | arbitrary string   | Overrides content Id for page           |
+---------------+--------------------+-----------------------------------------+
| `title`       | arbitrary string   | Overrides page title for catalog tree   |
+---------------+--------------------+-----------------------------------------+
| `order`       | integer number     | Overrides page order for catalog tree   |
+---------------+--------------------+-----------------------------------------+
| `tags`        | list of strings    | Defines page tags (used by Tags plugin) |
+---------------+--------------------+-----------------------------------------+
| `meta`        | list of key-values | Defines page metadata                   |
+---------------+--------------------+-----------------------------------------+
| `description` | arbitrary string   | Defines page metadata's description     |
+---------------+--------------------+-----------------------------------------+

For example, this very page has the following properties:

```yaml
---
title: Page properties
order: 1
tags:
  - tag1
  - tag2
  - tag3
---
```
