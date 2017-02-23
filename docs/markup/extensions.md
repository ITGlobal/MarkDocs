---
title: Markdown extensions
order: 2
---

# Supported markdown extensions

:::toc:::

## Abbreviations

:::col-md-6

    *[HTML]: Hyper Text Markup Language
    *[W3C]:  World Wide Web Consortium

    The HTML specification
    is maintained by the W3C.

:::

:::col-md-6

*[HTML]: Hyper Text Markup Language
*[W3C]:  World Wide Web Consortium

The HTML specification
is maintained by the W3C.

:::

***

## HTML attributes

You can use the following markup to place custom HTML attributes on specific elements.

:::col-md-6

    Use the `#` prefix to set `id` attribute:

    This H1 element will have "id"="header1" {#header1}
    ======================================

    Use the `.` prefix to set CSS classes:

    [This link will look like a button](../) {.btn .btn-default}

    ## This H2 element will have attribute "lang"="fr" {lang=fr}

:::

:::col-md-6

Use the `#` prefix to set `id` attribute:

This H1 element will have "id"="header1" {#header1}
======================================

Use the `.` prefix to set CSS classes:

[This link will look like a button](../){.btn .btn-default}

This paragraph will have a `alert alert-info` classes {.alert .alert-info}

## This H2 element will have attribute "lang"="fr" {lang=fr}

:::

***

## Auto identifiers

:::col-md-6

    This header will have `"id"="header-identifiers-in-html"`:

    # Header identifiers in HTML

    This header will have `"id"="dogsin-my-house"`:

    # *Dogs*?--in *my* house?

    This header will have `"id"="html-s5-or-rtf"`:

    # [HTML], [S5], or [RTF]?

    This header will have `"id"="applications"`:

    # 3. Applications

:::

:::col-md-6

This header will have `"id"="header-identifiers-in-html"`:

# Header identifiers in HTML

This header will have `"id"="dogsin-my-house"`:

# *Dogs*?--in *my* house?

This header will have `"id"="html-s5-or-rtf"`:

# [HTML], [S5], or [RTF]?

This header will have `"id"="applications"`:

# 3. Applications

:::

***

## Auto links

:::col-md-6

    Any text that looks like a hyperlink, like http://http-site.com/page.html, https://https-site.com, ftp://ftp-site.com, mailto:email@server.com or event google.com will be automatically converted into a hyperlink.

:::

:::col-md-6

Any text that looks like a hyperlink, like http://http-site.com/page.html, https://https-site.com, ftp://ftp-site.com, mailto:email@server.com or event google.com will be automatically converted into a hyperlink.

:::

***

## Citations

:::col-md-6

    The following text will be rendered as a citation ""Citation text"". And this is not a part of citation.

:::

:::col-md-6

The following text will be rendered as a citation ""Citation text"". And this is not a part of citation.

:::

***

## [Custom containers](/markup/containers.md)

***

## Custom emphasis

:::col-md-6

    This is a ::special emphasis::

    This is a ::special emphasis with attributes::{.text-warning}

:::

:::col-md-6

This markup will be rendered as

    <p>
        This is a <span>special emphasis</span>
    </p>

    <p>
        This is a <span class="text-warning">special emphasis with attributes</span>
    </p>

This is a ::special emphasis::

This is a ::special emphasis with attributes::{.text-warning}

:::

***

## [Diagrams](/markup/diagrams.md)

***

## [Emoji](/markup/emoji.md)

***

## Definition lists

:::col-md-6

    Apple
    :   Pomaceous fruit of plants of the genus Malus in 
        the family Rosaceae.

    Orange
    :   The fruit of an evergreen tree of the genus Citrus.

    Apple
    :   Pomaceous fruit of plants of the genus Malus in 
    the family Rosaceae.

    Orange
    :   The fruit of an evergreen tree of the genus Citrus.

:::

:::col-md-6

Apple
:   Pomaceous fruit of plants of the genus Malus in 
    the family Rosaceae.

Orange
:   The fruit of an evergreen tree of the genus Citrus.

Apple
:   Pomaceous fruit of plants of the genus Malus in 
the family Rosaceae.

Orange
:   The fruit of an evergreen tree of the genus Citrus.

:::

***

## Figures

:::col-md-6

    ^^^
    ![](/images/example.png "Image tooltip 1")
    ![](/images/example.png "Image tooltip 2")
    ![](/images/example.png "Image tooltip 3")
    ![](/images/example.png "Image tooltip 4")
    ^^^ Figure caption

:::

:::col-md-6

^^^
![](/images/example.png "Image tooltip 1")
![](/images/example.png "Image tooltip 2")
![](/images/example.png "Image tooltip 3")
![](/images/example.png "Image tooltip 4")
^^^ Figure caption

:::

***

## Footers

:::col-md-6

    ^^ This is a multiline
    ^^ footer with a ""citation for name""

    > This is a block quote that contains a footer
    > ^^ This is a multiline
    > ^^ footer with a ""citation for name""

:::

:::col-md-6

^^ This is a multiline
^^ footer with a ""citation for name""

> This is a block quote that contains a footer
> ^^ This is a multiline
> ^^ footer with a ""citation for name""

:::

***

## Footnotes

:::col-md-6

    That's some text with a footnote[^1]

    [^1]: And that's the footnote.

:::

:::col-md-6

That's some text with a footnote [^1]

[^1]: And that's the footnote.

:::

***

## [Icons](/markup/icons.md)

***

## Mathematics expressions

:::col-md-6

    The *Gamma function* satisfying $\Gamma(n) = (n-1)!\quad\forall n\in\mathbb N$ is via the Euler integral

    $$
    \Gamma(z) = \int_0^\infty t^{z-1}e^{-t}dt\,.
    $$

:::

:::col-md-6

The *Gamma function* satisfying $\Gamma(n) = (n-1)!\quad\forall n\in\mathbb N$ is via the Euler integral

$$
\Gamma(z) = \int_0^\infty t^{z-1}e^{-t}dt\,.
$$

:::

***

## Embedded media

Use the following markup to embed audio files, video files and YouTube videos: 

:::col-md-6

    ![](/images/video.mp4)

    ![](https://www.youtube.com/watch?v=iIZFvNlZHf0)

:::

:::col-md-6

![](/images/video.mp4)

![](https://www.youtube.com/watch?v=iIZFvNlZHf0)

:::

***

## Typography helpers

:::col-md-6

    (c) (C) (r) (R) (tm) (TM) (p) (P) +-

    test.. test... test..... test?..... test!....

    !!!!!! ???? ,,  -- ---

    "Smartypants, double quotes" and 'single quotes'

    <<Angle quotes>>

:::

:::col-md-6

(c) (C) (r) (R) (tm) (TM) (p) (P) +-

test.. test... test..... test?..... test!....

!!!!!! ???? ,,  -- ---

"Smartypants, double quotes" and 'single quotes'

<<Angle quotes>>

:::

***

## Task lists

Use the following markup to define task lists:

:::col-md-6

    - [ ] a task list item
    - [ ] list syntax required
    - [ ] normal **formatting**, @mentions, #1234 refs
    - [ ] incomplete
    - [x] completed

:::

:::col-md-6

- [ ] a task list item
- [ ] list syntax required
- [ ] normal **formatting**, @mentions, #1234 refs
- [ ] incomplete
- [x] completed

:::

***

## Custom containers

:::col-md-6

    :::spoiler
    This is a spoiler
    :::

:::

:::col-md-6

This markup will be rendered as

```html
<div class="spoiler">
    <p>This is a spoiler</p>
</div>
```

:::spoiler
This is a spoiler
:::

:::