// Track WebSocket instances for Send-PupWebSocketMessage
(function() {
    const OriginalWebSocket = window.WebSocket;
    const trackedSockets = [];
    const urlMap = new WeakMap();

    // Use Symbol keys — invisible to Object.keys, Object.getOwnPropertyNames, for..in
    // Only Object.getOwnPropertySymbols can find them (rarely checked by detectors)
    Object.defineProperty(window, Symbol.for('__pup_ws'), {
        get: () => trackedSockets,
        enumerable: false,
        configurable: false
    });
    Object.defineProperty(window, Symbol.for('__pup_wsu'), {
        value: (ws) => urlMap.get(ws),
        enumerable: false,
        configurable: false,
        writable: false
    });

    // Named function gives correct .name property ("WebSocket")
    window.WebSocket = function WebSocket(url, protocols) {
        const ws = protocols ? new OriginalWebSocket(url, protocols) : new OriginalWebSocket(url);
        urlMap.set(ws, url);
        trackedSockets.push(ws);
        ws.addEventListener('close', () => {
            const idx = trackedSockets.indexOf(ws);
            if (idx > -1) trackedSockets.splice(idx, 1);
        });
        return ws;
    };
    window.WebSocket.prototype = OriginalWebSocket.prototype;
    window.WebSocket.CONNECTING = OriginalWebSocket.CONNECTING;
    window.WebSocket.OPEN = OriginalWebSocket.OPEN;
    window.WebSocket.CLOSING = OriginalWebSocket.CLOSING;
    window.WebSocket.CLOSED = OriginalWebSocket.CLOSED;

    // Native WebSocket has length=1 (url required, protocols optional)
    Object.defineProperty(window.WebSocket, 'length', { value: 1, configurable: true });

    // Chain onto Function.prototype.toString (stealth.js already patched it)
    const prevToString = Function.prototype.toString;
    const wsConstructor = window.WebSocket;
    const wsToString = function toString() {
        if (this === wsConstructor) return 'function WebSocket() { [native code] }';
        if (this === wsToString) return 'function toString() { [native code] }';
        return prevToString.call(this);
    };
    Function.prototype.toString = wsToString;
})();
