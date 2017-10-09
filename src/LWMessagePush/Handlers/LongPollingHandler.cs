using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using LWMessagePush.Options;
using System.Threading;
using LWMessagePush.DTOs;

namespace LWMessagePush.Handlers
{
    /// <summary>
    /// Long Polling connection handler
    /// </summary>
    public class LongPollingHandler : IConnectionHandler
    {
        #region Fields    

        private IPersistanceService _persistanceService;
        private static EventWaitHandle _waitHandle = new ManualResetEvent(false);
        private object _waitHandleTransitionLock = new object();

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="persitanceService">Persistance service instance</param>
        public LongPollingHandler(IPersistanceService persitanceService)
        {
            _persistanceService = persitanceService;
        }

        #region IConnectionHandler Members

        /// <summary>
        /// ConnectionHandler identifier
        /// </summary>
        public string Name => this.GetType().Name;

        /// <summary>
        /// Handles long polling requests
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="options">Middleware options</param>
        /// <returns></returns>
        public async Task HandleRequest(HttpContext context, LWMessagePushMiddlewareOptions options)
        {
            try
            {
                var startDateTime = DateTime.UtcNow;
                var endDateTime = startDateTime.AddSeconds(options.LongPollingConfiguration.TimeoutSeconds);
                string topic = context.Request.Query["topic"];
                string lastReceive = context.Request.Query["lastr"];

                options.LogListener.Log(string.Format("LongPolling request is received at {0} UTC for {1}", startDateTime.ToString(), topic), Logging.LogLevel.Verbose);

                if (topic == null)
                {
                    string err = "Topic is required.";
                    options.LogListener.Log(string.Format("LongPolling handler error: {0}", err), Logging.LogLevel.Error);
                    throw new ArgumentNullException(err);
                }

                DateTime lastReceivedMsgDT = DateTime.UtcNow;
                if (lastReceive != null)
                    DateTime.TryParse(lastReceive, out lastReceivedMsgDT);

                List<PushMessage> messageList = Internal_HandleRequest(topic, endDateTime, lastReceivedMsgDT);
                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(messageList));
            }
            catch (Exception e)
            {
                options.LogListener.Log(string.Format("LongPolling handler error: {0}", e.Message), Logging.LogLevel.Error);
                throw e;
            }
           
        }

        /// <summary>
        /// Awakes waiting threds to sends specified message to the subscribers of the given topic      
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message content</param>
        /// <returns></returns>
        public async Task PushMessage(string topic, PushMessage message)
        {
            lock (_waitHandleTransitionLock)
            {
                _waitHandle.Set();
                _waitHandle.Reset();
            }

            await Task.CompletedTask;
        }

        #endregion

        #region Private

        /// <summary>
        /// Long polling wait loop, incoming requests are slept until timeout or a new message
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="endDateTime">Ending time of sleep operation</param>
        /// <param name="lastReceivedMsgDT">UTC DateTime value of the last send message</param>
        /// <returns></returns>
        private List<PushMessage> Internal_HandleRequest(string topic, DateTime endDateTime, DateTime? lastReceivedMsgDT)
        {
            lock(_waitHandleTransitionLock)
            {
                // Waitfor proper _waitHandle transition to avoid stackoverflow
            }

            var messages = _persistanceService.Get(topic, lastReceivedMsgDT);
            if ((messages != null) && (messages.Count>0))
                return messages;
            else
            {
                int remaingWaitMiliseconds = (int)endDateTime.Subtract(DateTime.UtcNow).TotalMilliseconds;
                if (remaingWaitMiliseconds == 0)
                    return _persistanceService.Get(topic, lastReceivedMsgDT);
                else if (remaingWaitMiliseconds < 0)
                    return new List<DTOs.PushMessage>();

                // Wait for any signal
                _waitHandle.WaitOne(remaingWaitMiliseconds);

                return Internal_HandleRequest(topic, endDateTime, lastReceivedMsgDT);
            }
        }

        #endregion
    }
}
