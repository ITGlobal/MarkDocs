---
title: Custom emphasis
order: 12
---

Custom emphasis
===============

Markup
------

```markdown

This is a ::special emphasis::

This is a ::special emphasis with attributes::{.text-warning}

```

Results
-------

The markup above will be rendered as

```html
<p>
    This is a <span>special emphasis</span>
</p>

<p>
    This is a <span class="text-warning">special emphasis with attributes</span>
</p>
```

:::well

This is a ::special emphasis::

This is a ::special emphasis with attributes::{.text-warning}

:::
