---
title: Dependencies
---

External Dependencies
=====================

Libraries
---------

`MarkDocs` uses the following external libraries:

* [`Markdig`](https://github.com/lunet-io/markdig)
* [`SharpYaml`](https://github.com/xoofx/SharpYaml)
* [`Newtonsoft.Json`](http://www.newtonsoft.com/json)
* [`AngleSharp`](https://github.com/AngleSharp/AngleSharp)
* [`highlight.js`](https://github.com/isagalaev/highlight.js) (is used on server-side)

Web services
------------

`MarkDocs` uses some third-party web services during page generation:

* `http://latex.codecogs.com` - is used by [math extension](/markup/extensions.md#mathematics-expressions) to render images from LaTex expressions
* `http://www.plantuml.com/plantuml` - is used by [diagrams extension](/markup/diagrams.md) to render images from PlantUML markup

All these web-services can be disabled or overriden from configuration