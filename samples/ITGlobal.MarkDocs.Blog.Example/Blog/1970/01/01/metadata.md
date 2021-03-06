---
# Override blog post title
title: Blog post metadata (custom title)
# Set post description
description: Page descriptions
# Set blog post title image
title_image: example.png
# Override blog post URL slug
slug: custom-page-slug
# Override blog post date
date: 1970-01-02
# Set blog post time
time: 15:30
# Define blog post permalink
permalink: about-metadata
# Define blog post tags
tags:
- tag-1
- tag-2
- tag-N
# Override post author
author: Test Author
# Set page metatags
meta:
- <meta name="author" content="Blog post autho" />
- <meta name="copyright" lang="en" content="MarkDocs" />
- <meta name="description" content="MarkDocs Blog Engine" />
---

# Blog post metadata

Add YAML frontmatter to markup and define metadata:

```yaml
---
# Override blog post title
title: Blog post metadata (custom title)

# Set post description
description: Page descriptions

# Set blog post title image
title_image: example.png
# or 
# image: example.png
# Path can be absolute or relative to blog post page

# Override blog post URL slug
slug: custom-page-slug

# Override blog post date
date: 1970-01-02

# Set blog post time
time: 15:30

# Define blog post permalink
permalink: about-metadata

# Define blog post tags
tags:
- tag-1
- tag-2
- tag-N

# Override post author
author: Test Author

# Set page metatags
meta:
- <meta name="author" content="Blog post autho" />
- <meta name="copyright" lang="en" content="MarkDocs" />
- <meta name="description" content="MarkDocs Blog Engine" />
---
```

All properties are optional.