---
title: Fenced code block
---

Fenced code block
=================

> Syntax highlighting is configurable and uses [Highlight.js](https://highlightjs.org/) by default

## Simple code block

:::col-md-6

    ```
    This section will be rendered as code block
    ```

:::

:::col-md-6

```
This section will be rendered as code block
```

:::

***

## Code block with syntax higtlight

Place a language code after the fence start to enable syntax highlighting.

:::col-md-6

    ```javascript
    var s = "JavaScript syntax highlighting";
    alert(s);
    ```
:::

:::col-md-6

```javascript
var s = "JavaScript syntax highlighting";
alert(s);
```

:::

***

:::col-md-6
 
    ```python
    s = "Python syntax highlighting"
    print s
    ```

:::

:::col-md-6

```python
s = "Python syntax highlighting"
print s
```
:::

***

:::col-md-6

    ```go
    package main

    import "fmt"

    func main() {
        ch := make(chan float64)
        ch <- 1.0e10    // magic number
        x, ok := <- ch
        defer fmt.Println(`exitting now\`)
        go println(len("hello world!"))
        return
    }
    ```

:::

:::col-md-6

```go
package main

import "fmt"

func main() {
    ch := make(chan float64)
    ch <- 1.0e10    // magic number
    x, ok := <- ch
    defer fmt.Println(`exitting now\`)
    go println(len("hello world!"))
    return
}
```

:::

***

:::col-md-6

```bash
    #!/bin/bash

    ###### CONFIG
    ACCEPTED_HOSTS="/root/.hag_accepted.conf"
    BE_VERBOSE=false

    if [ "$UID" -ne 0 ]
    then
     echo "Superuser rights required"
     exit 2
    fi

    genApacheConf(){
     echo -e "# Host ${HOME_DIR}$1/$2 :"
    }
    ```

:::

:::col-md-6

```bash
#!/bin/bash

###### CONFIG
ACCEPTED_HOSTS="/root/.hag_accepted.conf"
BE_VERBOSE=false

if [ "$UID" -ne 0 ]
then
 echo "Superuser rights required"
 exit 2
fi

genApacheConf(){
 echo -e "# Host ${HOME_DIR}$1/$2 :"
}
```

:::

***

### Supported languages

See [Highlight.js documentation](http://highlightjs.readthedocs.io/en/latest/css-classes-reference.html)

* 1c
* abnf
* accesslog
* ada
* armasm
* arm
* avrasm
* actionscript
* as
* apache
* apacheconf
* applescript
* osascript
* asciidoc
* adoc
* aspectj
* autohotkey
* autoit
* awk
* mawk
* nawk
* gawk
* axapta
* bash
* sh
* zsh
* basic
* bnf
* brainfuck
* bf
* cs
* csharp
* cpp
* c
* cc
* h
* c++
* h++
* hpp
* cal
* cos
* cls
* cmake
* cmake.in
* coq
* csp
* css
* capnproto
* capnp
* clojure
* clj
* coffeescript
* coffee
* cson
* iced
* crmsh
* crm
* pcmk
* crystal
* cr
* d
* dns
* zone
* bind
* dos
* bat
* cmd
* dart
* delphi
* dpr
* dfm
* pas
* pascal
* freepascal
* lazarus
* lpr
* lfm
* diff
* patch
* django
* jinja
* dockerfile
* docker
* dsconfig
* dts
* dust
* dst
* ebnf
* elixir
* elm
* erlang
* erl
* excel
* xls
* xlsx
* fsharp
* fs
* fix
* fortran
* f90
* f95
* gcode
* nc
* gams
* gms
* gauss
* gss
* gherkin
* go
* golang
* golo
* gololang
* gradle
* groovy
* xml
* html
* xhtml
* rss
* atom
* xjb
* xsd
* xsl
* plist
* http
* https
* haml
* handlebars
* hbs
* html.hbs
* html.handlebars
* haskell
* hs
* haxe
* hx
* ini
* inform7
* i7
* irpf90
* json
* java
* jsp
* javascript
* js
* jsx
* lasso
* ls
* lassoscript
* less
* ldif
* lisp
* livecodeserver
* livescript
* ls
* lua
* makefile
* mk
* mak
* markdown
* md
* mkdown
* mkd
* mathematica
* mma
* matlab
* maxima
* mel
* mercury
* mizar
* mojolicious
* monkey
* moonscript
* moon
* nsis
* nginx
* nginxconf
* nimrod
* nim
* nix
* ocaml
* ml
* objectivec
* mm
* objc
* obj-c
* glsl
* openscad
* scad
* ruleslanguage
* oxygene
* pf
* pf.conf
* php
* php3
* php4
* php5
* php6
* parser3
* perl
* pl
* pm
* pony
* powershell
* ps
* processing
* prolog
* protobuf
* puppet
* pp
* python
* py
* gyp
* profile
* k
* kdb
* qml
* r
* rib
* rsl
* graph
* instances
* ruby
* rb
* gemspec
* podspec
* thor
* irb
* rust
* rs
* scss
* sql
* p21
* step
* stp
* scala
* scheme
* scilab
* sci
* smali
* smalltalk
* st
* stan
* stata
* stylus
* styl
* subunit
* swift
* tap
* tcl
* tk
* tex
* thrift
* tp
* twig
* craftcms
* typescript
* ts
* vbnet
* vb
* vbscript
* vbs
* vhdl
* vala
* verilog
* v
* vim
* x86asm
* xl
* tao
* xpath
* xq
* zephir
* zep