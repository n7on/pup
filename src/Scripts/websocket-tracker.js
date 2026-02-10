// Track WebSocket instances for Send-PupWebSocketMessage
const OriginalWebSocket = window.WebSocket;
window.__pup_websockets = [];
window.WebSocket = function(url, protocols) {
    const ws = protocols ? new OriginalWebSocket(url, protocols) : new OriginalWebSocket(url);
    ws.__pup_url = url;
    window.__pup_websockets.push(ws);
    ws.addEventListener('close', () => {
        const idx = window.__pup_websockets.indexOf(ws);
        if (idx > -1) window.__pup_websockets.splice(idx, 1);
    });
    return ws;
};
window.WebSocket.prototype = OriginalWebSocket.prototype;
window.WebSocket.CONNECTING = OriginalWebSocket.CONNECTING;
window.WebSocket.OPEN = OriginalWebSocket.OPEN;
window.WebSocket.CLOSING = OriginalWebSocket.CLOSING;
window.WebSocket.CLOSED = OriginalWebSocket.CLOSED;
