using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using LWMessagePush.Options;
using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Threading;
using LWMessagePush.DTOs;

namespace LWMessagePush.Handlers
{
    /// <summary>
    /// Web Socket connection handler
    /// </summary>
    public class WebSocketHandler : IConnectionHandler
    {
        #region Fields

        private const int GARBAGE_COLLECTION_PERIOD_MS = 10000;
        private Timer _garbageCollectorTimer = new Timer(GarbageCollectorTimerElapsed, null, 1000, GARBAGE_COLLECTION_PERIOD_MS);
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, WebSocketItemInfo>> _sockets = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, WebSocketItemInfo>>();        
        private IPersistanceService _persistanceService;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="persitanceService"></param>
        public WebSocketHandler(IPersistanceService persitanceService)
        {
            _persistanceService = persitanceService;
        }

        #region IConnectionHandler Members

        /// <summary>
        /// ConnectionHandler identifier
        /// </summary>
        public string Name => this.GetType().Name;

        /// <summary>
        /// Handles web socket requests
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="options">Middleware options</param>
        /// <returns></returns>
        public async Task HandleRequest(HttpContext context, LWMessagePushMiddlewareOptions options)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                if (!context.Request.Query.ContainsKey("topic"))
                    throw new ArgumentNullException("Topic missing");

                string topic = context.Request.Query["topic"];
                Guid socketId = Guid.NewGuid();
                string lastReceive = context.Request.Query["lastr"];
                DateTime lastReceivedMsgDT = DateTime.UtcNow;
                if (lastReceive != null)
                    DateTime.TryParse(lastReceive, out lastReceivedMsgDT);

                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                WebSocketItemInfo socketInfo = new WebSocketItemInfo(webSocket, lastReceivedMsgDT);
                var newTopicDict = new ConcurrentDictionary<Guid, WebSocketItemInfo>();
                newTopicDict.TryAdd(socketId, socketInfo);

                _sockets.AddOrUpdate(topic, newTopicDict, (a, b) => { b.TryAdd(socketId, socketInfo); return b; });
                await ProcessRequest(context, socketId, socketInfo, topic);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }

        /// <summary>
        /// Sends specified message to the subscribers of the given topic      
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message content</param>
        /// <returns></returns>
        public async Task PushMessage(string topic, PushMessage message)
        {
            ConcurrentDictionary<Guid, WebSocketItemInfo> subscribers;
            if (_sockets.TryGetValue(topic, out subscribers))
            {
                Parallel.ForEach(subscribers, (socketConnTuple =>
                {
                    var socketConn = socketConnTuple.Value.Socket;
                    
                    PushMessageToClient(message, socketConn);

                }));                
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Private

        /// <summary>
        /// Sends specified message to the given websocket connection
        /// </summary>
        /// <param name="message">Message content</param>
        /// <param name="socketConn">WebSocket connection</param>
        /// <returns></returns>
        private async Task PushMessageToClient(PushMessage message, WebSocket socketConn)
        {
            if (socketConn.State != WebSocketState.Open)
                return;

            byte[] messageAsBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));

            await socketConn.SendAsync(new ArraySegment<byte>(messageAsBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Processes initial web socket connection request, sends unreceived messages and keeps websocket connection alive
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="socketId">Web Socket Id</param>
        /// <param name="socketInfo">Web Socket item info</param>
        /// <param name="topic">Message topic</param>
        /// <returns></returns>
        private async Task ProcessRequest(HttpContext context, Guid socketId, WebSocketItemInfo socketInfo, string topic)
        {
            // Send old messages
            var oldMessages = _persistanceService.Get(topic, socketInfo.CreationDT);
            foreach (var message in oldMessages)
            {
                await PushMessageToClient(message, socketInfo.Socket);
            }

            // Wait in a loop
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await socketInfo.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {               
                result = await socketInfo.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);                
            }

            // Close connection
            await socketInfo.Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

            // Remove from dictionary
            ConcurrentDictionary<Guid, WebSocketItemInfo> socketList;
            _sockets.TryGetValue(topic, out socketList);

            WebSocketItemInfo dummyVar;
            socketList.TryRemove(socketId, out dummyVar);
        }

        /// <summary>
        /// Removes redundant websocket objects
        /// </summary>
        /// <param name="state"></param>
        private static void GarbageCollectorTimerElapsed(object state)
        {
            List<string> toBeDeletedTopics = new List<string>();

            foreach (var topic in _sockets)
            {
                if (topic.Value.Count == 0)
                    toBeDeletedTopics.Add(topic.Key);
            }

            foreach (var topic in toBeDeletedTopics)
            {
                ConcurrentDictionary<Guid, WebSocketItemInfo> dummyVar;
                _sockets.TryRemove(topic, out dummyVar);
            }

            //

            foreach (var topic in _sockets)
            {
                List<Guid> toBeDeletedSockets = new List<Guid>();
                foreach (var socket in topic.Value)
                {
                    if (socket.Value.Socket.State != WebSocketState.Open)
                        toBeDeletedSockets.Add(socket.Key);
                }

                foreach (var socketId in toBeDeletedSockets)
                {
                    WebSocketItemInfo dummyVar;
                    topic.Value.TryRemove(socketId, out dummyVar);
                }
            }
        }

        #endregion

        #region Internal Classes
        /// <summary>
        /// Keeps websocket connection info
        /// </summary>
        internal class WebSocketItemInfo
        {
            public WebSocket Socket { get; private set; }

            public DateTime? CreationDT { get; private set; }

            public WebSocketItemInfo(WebSocket socket, DateTime? lastReceiveDt)
            {
                this.Socket = socket;
                this.CreationDT = lastReceiveDt;
            }
        }

        #endregion
    }
}
