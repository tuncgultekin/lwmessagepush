var LWMessagePushConnectionMethod = {

    Automatic: "automatic",
    WebSocket: "websocket",
    LongPolling: "longpolling"

};

var LWMessagePushClient = function (onMessageReceived, serverUrl, topic, connectionMethod, logger) {

    if (!onMessageReceived)
        throw "Message receive callback (onMessageReceived) is required.";

    if (!serverUrl)
        throw "Server url (onMessageReceived) is required.";

    if (!topic)
        throw "Topic is required.";

    var _serverUrl = serverUrl;
    var _connectionMethod = (connectionMethod) ? connectionMethod : LWMessagePushConnectionMethod.Automatic;
    var _logger = (logger) ? logger : function (log) { console.log(log); };
    var _onMessageReceived = onMessageReceived;
    var _activeConnectionMethod = -1;
    var _topic = topic;
    var _lastReceiveDt = null;
    var _webSocketUri = ((serverUrl.startsWith("https://")) ? serverUrl.replace("https://", "wss://") : serverUrl.replace("http://", "ws:")) + "/__lwpush/ws";
    var _longPollUri = serverUrl + "/__lwpush/poll";
    var _socket;
    var _longPollReq;

    //////////////////////

    // Published methods

    this.connect = function () {

        var supportsWebSockets = "WebSocket" in window;

        if (_connectionMethod == LWMessagePushConnectionMethod.Automatic) {
            if (supportsWebSockets)
                try {
                    connectWithWebSocket();
                } catch (e) {
                    logger("Websocket connection error: " + e);
                    logger("Falling back to Long Polling...");
                    connectWithLongPolling();
                }

            else
                connectWithLongPolling();
        }
        else if (_connectionMethod == LWMessagePushConnectionMethod.WebSocket) {
            if (supportsWebSockets)
                connectWithWebSocket();
            else
                throw "Websockets is not supported for this browser";
        }
        else if (_connectionMethod == LWMessagePushConnectionMethod.LongPolling)
            connectWithLongPolling();
        else
            throw "LWMessagePushConnectionMethod '" + _connectionMethod + "' is not supported";
    }

    this.disconnect = function () {

        if (_activeConnectionMethod == LWMessagePushConnectionMethod.WebSocket) {
            if (!(!_socket || _socket.readyState != WebSocket.OPEN))
                _socket.close(1000, "Closing from client");
        }
        else if (_activeConnectionMethod == LWMessagePushConnectionMethod.LongPolling) {
            if ((_longPollReq) && (_longPollReq.abort)) {
                _longPollReq.abort();
                _longPollReq = null;
                logger("Long polling loop is stopped.");
            }
        }

        _activeConnectionMethod = -1;
    }

    // Private functions

    function connectWithWebSocket() {

        _activeConnectionMethod = LWMessagePushConnectionMethod.WebSocket;

        var uri = _webSocketUri + "?topic=" + _topic + ((_lastReceiveDt) ? "&lastr=" + _lastReceiveDt : "");

        _socket = new WebSocket(uri);
        _socket.onopen = function (event) {
            logger("WebSocket connection is opened.");
        };
        _socket.onclose = function (event) {
            logger("WebSocket connection is closed.");
        };
        _socket.onerror = function (event) { logger("WebSocket error: " + event.data); };
        _socket.onmessage = function (event) {

            logger("A message received via websocket.");
            processReceivedData(event.data);
        };
    }

    function connectWithLongPolling() {

        _activeConnectionMethod = LWMessagePushConnectionMethod.LongPolling;
        logger("Long polling request starting...");

        var uri = _longPollUri + "?topic=" + _topic + ((_lastReceiveDt) ? "&lastr=" + _lastReceiveDt : "");
        _longPollReq = $.get(uri, function (data) {

            processReceivedData(data);

            connectWithLongPolling();
        });
    }

    function processReceivedData(data) {

        if (!data)
            return;

        data = JSON.parse(data);
        if (data.Content) {
            onMessageReceived(data);
            _lastReceiveDt = data.CreationUTC;
        }
        else if ((data.length) && (data.length > 0)) {
            for (var i = 0; i < data.length; i++) {
                onMessageReceived(data[i]);
            }

            _lastReceiveDt = data[data.length - 1].CreationUTC;
        }
    }
}