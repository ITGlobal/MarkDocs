function initDevServices(url) {
    url = (window.location.protocol === 'http:' ? 'ws://' : 'wss://') + window.location.host + url;
    console.log('[dev] connecting to websocket at ', url);

    var webSocket = new WebSocket(url);
    webSocket.onopen = function () {
        console.log('[dev] connected');
    };
    webSocket.onerror = function (event) {
        console.log('[dev] error: ', event);
    };
    webSocket.onmessage = function (event) {
        var e = JSON.parse(event.data);
        console.log('dev: received message: ', e);
        if (e.type === 'update') {
            window.location.reload();
        }
    };
    webSocket.onclose = function (event) {
        console.log('[dev] closed: ', event.code);
    };
}