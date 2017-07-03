var hljs = require('highlightjs');
var fs = require('fs');

var EOF = 0x1A;
var buffer = [];

var BUFSIZE = 256;
var buf = new Buffer(BUFSIZE);

var bytesRead;
var shouldRead = true;

process.stdin.setEncoding('utf8');
while (shouldRead) {
    try {
        bytesRead = fs.readSync(process.stdin.fd, buf, 0, BUFSIZE);
    } catch (e) {
        break;
    }

    if (bytesRead === 0) {
        break;
    }

    for (var i = 0; i < bytesRead; i++) {
        if (buf[i] === EOF) {
            shouldRead = false;
            break;
        }
        if (buf[i] === '\r' || buf[i] === '\n') {
            continue;
        }
        buffer.push(buf[i]);
    }
}

var inputJson = new Buffer(buffer).toString('utf8');
var request = JSON.parse(inputJson);
var output = hljs.highlight(request.language, request.sourceCode, true);
console.log(JSON.stringify(output.value));