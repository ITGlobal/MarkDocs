---
title: Common Markup
---

# Supported common markdown markup

:::toc:::

## Headers

:::col-md-6

    # H1
    ## H2
    ### H3
    #### H4
    ##### H5
    ###### H6

    Alternatively, for H1 and H2, an underline-ish style:

    Alt-H1
    ======

    Alt-H2
    ------

:::

:::col-md-6

# H1
## H2
### H3
#### H4
##### H5
###### H6

Alternatively, for H1 and H2, an underline-ish style:

Alt-H1
======

Alt-H2
------

:::

***

## Emphasis

:::col-md-6

    Italics: *use asterisks* or _use underscores_

    Bold: **use double asterisks** or __use double underscores__

    Bold italics: ***use triple asterisks*** or ___use triple underscores___ or **combine _both_ markups**

    ~~Strikethrough text~~

    ~Subscript~ Normal text

    ^Superscript^ Normal text

    ++Underline++

    ==Marked text==

:::

:::col-md-6

Italics: *use asterisks* or _use underscores_

Bold: **use double asterisks** or __use double underscores__

Bold italics: ***use triple asterisks*** or ___use triple underscores___ or **combine _both_ markups**

~~Strikethrough text~~

~Subscript~ Normal text

^Superscript^ Normal text

++Underline++

==Marked text==

:::

***

## Lists markup

### Unordered (bullet) lists

:::col-md-6

    * Item 1
    * Item 2.
      This is a continuation of "Item 2". Please note - there is no line break here. 
    * Item 3
      This is a new line within a list item. Please note that previous line ends with two spaces!
    * Item 4

      Nested paragraph within a list item. You'll need a blank line above it and leading spaces.
      Nested paragraph, just like an ordinary paragraph, might be split into few lines.

    * Item ...
    * Item ...
    * Item ...

:::

:::col-md-6

* Item 1
* Item 2.
  This is a continuation of "Item 2". Please note - there is no line break here.
* Item 3
  This is a new line within a list item. Please note that previous line ends with two spaces!
* Item 4

  Nested paragraph within a list item. You'll need a blank line above it and leading spaces.
  Nested paragraph, just like an ordinary paragraph, might be split into few lines.

* Item ...
* Item ...
* Item ...

:::

You can use different bullets: `*`, `+` or `-`:

:::col-md-6

    * Unordered list can use asterisks
    - Or minuses
    + Or pluses

:::

:::col-md-6

* Unordered list can use asterisks
- Or minuses
+ Or pluses

:::

***

## Ordered lists

:::col-md-6

    1. Item 1. Please note - you don't need to watch for numbering
    1. Item 2.
      This is a continuation of "Item 2". Please note - there is no line break here. 
    1. Item 3  
      This is a new line within a list item. Please note that previous line ends with two spaces!
    1. Item 4
    
       Nested paragraph within a list item. You'll need a blank line above it and leading spaces.
       Nested paragraph, just like an ordinary paragraph, might be split into few lines.
    
    1. Item ...
    1. Item ...
    1. Item ...

    You can use letters instead of numbers:

    a. Item ...
    a. Item ...
    a. Item ...
    a. Item ...
    a. Item ...

    A. Item ...
    A. Item ...
    A. Item ...
    A. Item ...
    A. Item ...
    A. Item ...

:::

:::col-md-6

1. Item 1. Please note - you don't need to watch for numbering
1. Item 2.
  This is a continuation of "Item 2". Please note - there is no line break here.
1. Item 3
  This is a new line within a list item. Please note that previous line ends with two spaces!
1. Item 4

   Nested paragraph within a list item. You'll need a blank line above it and leading spaces.
   Nested paragraph, just like an ordinary paragraph, might be split into few lines.

1. Item ...
1. Item ...
1. Item ...

You can use letters instead of numbers:

a. Item ...
a. Item ...
a. Item ...
a. Item ...
a. Item ...

A. Item ...
A. Item ...
A. Item ...
A. Item ...
A. Item ...
A. Item ...

:::

***

## Nested lists

:::col-md-6

    1. Item 1
       * Item 1.1
       * Item 1.2
       * Item 1.3
    1. Item 2
       1. Item 2.1
       1. Item 2.2
       1. Item 2.3
    1. Item 3
       1. Item 3.1
          * Item 3.1.1
          * Item 3.1.2
       1. Item 3.2

:::

:::col-md-6

1. Item 1
   * Item 1.1
   * Item 1.2
   * Item 1.3
1. Item 2
   1. Item 2.1
   1. Item 2.2
   1. Item 2.3
1. Item 3
   1. Item 3.1
      * Item 3.1.1
      * Item 3.1.2
   1. Item 3.2

:::

***

## Inline links

### Internet link

:::col-md-6

    [Open Google](https://www.google.com)

:::

:::col-md-6

[Open Google](https://www.google.com)

:::

### Internet link with tooltip

:::col-md-6

    [Open Google (hover to see a tooltip)](https://www.google.com "Open Google web site")

:::

:::col-md-6

[Open Google (hover to see a tooltip)](https://www.google.com "Open Google web site")

:::

### Link to another documentation page

:::col-md-6

    [Open root page](../)

    Or you can use absolute links: [Open root page](/)

:::

:::col-md-6

[Open root page](../)

Or you can use absolute links: [Open root page](/)

:::

***

## Reference links


:::col-md-6

    [Click here to find out more about markdown][markdown-term]

    [Click here to read more][1]

    See [1]

    ***

    You'll need to place definitions somewhere (they won't be visible):

    [markdown-term]: https://google.com
    [1]: ../markup/text

:::

:::col-md-6

[Click here to find out more about markdown][markdown-term]

[Click here to read more][1]

See [1]

***

You'll need to place definitions somewhere (they won't be visible):

[markdown-term]: https://google.com
[1]: ../markup/emphasis

:::

***

## Inline images from file

:::col-md-6

    ![](/images/example.png)

    This image will have a title text (hover to see it):

    ![](/images/example.png "Logo from file")

:::

:::col-md-6

![](/images/example.png)

This image will have a title text (hover to see it):

![](/images/example.png "Logo from file")

:::

***


## Inline images from URL

:::col-md-6

    ![](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png)

    This image will have a title text (hover to see it):

    ![](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png "Logo from internet")

:::

:::col-md-6

![](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png)

This image will have a title text (hover to see it):

![](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png "Logo from internet")

:::

***

## Inline images with alt text

:::col-md-6

    ![alt text](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png)

    ![alt text](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png.NOT_FOUND)

::::

:::col-md-6

![alt text](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png)

![alt text](https://raw.githubusercontent.com/lunet-io/markdig/master/img/markdig.png.NOT_FOUND)

:::

***

## Inline code highlight

:::col-md-6

    Use `back-ticks` around any text that should be rendered with `monospace font`

:::

:::col-md-6

Use `back-ticks` around any text that should be rendered with `monospace font`

:::

***

## [Fended code block](code.md)

***

## Indented code block

**Note:** Indented code blocks don't support syntax highlighting.

:::col-md-6

    Add 4 spaces in the beginning of each line to create an indented code block

        This section will be rendered as code block
        This section will be rendered as code block
        This section will be rendered as code block
        This section will be rendered as code block

:::

:::col-md-6

Add 4 spaces in the beginning of each line to create an indented code block

    This section will be rendered as code block
    This section will be rendered as code block
    This section will be rendered as code block
    This section will be rendered as code block

:::

***

## Pipe tables

:::col-md-6

    | Fruit    | Price | Advantages       |
    | -------- | ----- | ---------------- |
    | Bananas  | $1.34 | built-in wrapper |
    | Oranges  | $2.10 | cures scurvy     |

:::

:::col-md-6

| Fruit    | Price | Advantages       |
| -------- | ----- | ---------------- |
| Bananas  | $1.34 | built-in wrapper |
| Oranges  | $2.10 | cures scurvy     |

:::

***

## Pipe tables with text alignment

:::col-md-6

    | Left aligned | Centered      | Right aligned    |
    | :----------- |:-------------:| ----------------:|
    | Bananas      | $1.34         | built-in wrapper |
    | Oranges      | $2.10         | cures scurvy     |

:::

:::col-md-6

| Left aligned | Centered      | Right aligned    |
| :----------- |:-------------:| ----------------:|
| Bananas      | $1.34         | built-in wrapper |
| Oranges      | $2.10         | cures scurvy     |

:::

***

## Less pretty pipe tables

:::col-md-6

    Markdown | Less | Pretty
    --- | --- | ---
    *Still* | `renders` | **nicely**
    1 | 2 | 3

:::

:::col-md-6

Markdown | Less | Pretty
--- | --- | ---
*Still* | `renders` | **nicely**
1 | 2 | 3

:::

***

## Grid tables

:::col-md-6

    +---------------+---------------+--------------------+
    | Fruit         | Price         | Advantages         |
    +===============+===============+====================+
    | Bananas       | **$1.34**     | - built-in wrapper |
    |               |               | - bright color     |
    +---------------+---------------+--------------------+
    | `Oranges`     | $2.10         | 1. cures scurvy    |
    |               |               | 1. tasty           |
    +---------------+---------------+--------------------+

:::

:::col-md-6

+---------------+---------------+--------------------+
| Fruit         | Price         | Advantages         |
+===============+===============+====================+
| Bananas       | **$1.34**     | - built-in wrapper |
|               |               | - bright color     |
+---------------+---------------+--------------------+
| `Oranges`     | $2.10         | 1. cures scurvy    |
|               |               | 1. tasty           |
+---------------+---------------+--------------------+

:::

***

## Quotation blocks

:::col-md-6

    > # Quote header
    > Quote line 1
    >
    > Quote line 2
    >
    > ***
    >
    > Quote list:
    > * Item 1
    > * Item 2
    > * Item 3

:::

:::col-md-6

> # Quote header
> Quote line 1
>
> Quote line 2
>
> ***
>
> Quote list:
> * Item 1
> * Item 2
> * Item 3

:::

***

## Inline HTML

:::col-md-6

    <div class="row">
        <div class="col-md-6 bg-success">
            <h1>My Text</h1>
        </div>
        <div class="col-md-3 bg-info">
            <p>
                Paragraph 1
            </p>
            <p>
                Paragraph 2
            </p>
        </div>
        <div class="col-md-3 bg-warning">
            <button class="btn btn-default">
                <i class="fa fa-ok"></i> Button
            </button>
        </div>
    </div>

:::

:::col-md-6

<div class="row">
    <div class="col-md-6 bg-success">
        <h1>My Text</h1>
    </div>
    <div class="col-md-3 bg-info">
        <p>
            Paragraph 1
        </p>
        <p>
            Paragraph 2
        </p>
    </div>
    <div class="col-md-3 bg-warning">
        <button class="btn btn-default">
            <i class="fa fa-ok"></i> Button
        </button>
    </div>
</div>

:::

***

## Escaping

:::col-md-6

    \*not emphasized*
    \<br/> not a tag
    \[not a link](/foo)
    \`not code`
    1\. not a list
    \* not a list
    \# not a heading
    \[foo]: /url "not a reference"

    BUT!: \\*still emphasis*

:::

:::col-md-6

\*not emphasized*

\<br/> not a tag

\[not a link](/foo)
\`not code`

1\. not a list

\* not a list

\# not a heading

\[foo]: /url "not a reference"

BUT!: \\*still emphasis*

:::

***

## Entities and numeric characters

:::col-md-6

    Entity references:

    &nbsp; &amp; &copy; &AElig; &Dcaron;
    &frac34; &HilbertSpace; &DifferentialD;
    &ClockwiseContourIntegral; &ngE;

    Decimal numeric characters:

    &#35; &#1234; &#992; &#98765432; &#0;

    Hexadecimal numeric characters:

    &#X22; &#XD06; &#xcab;

:::

:::col-md-6

Entity references:

&nbsp; &amp; &copy; &AElig; &Dcaron;
&frac34; &HilbertSpace; &DifferentialD;
&ClockwiseContourIntegral; &ngE;

Decimal numeric characters:

&#35; &#1234; &#992; &#98765432; &#0;

Hexadecimal numeric characters:

&#X22; &#XD06; &#xcab;

:::