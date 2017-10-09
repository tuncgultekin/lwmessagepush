using LWMessagePush.Logging;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Options
{
    /// <summary>
    /// LWMessagePush options
    /// </summary>
    public class LWMessagePushMiddlewareOptions
    {
        private LogListenerBase _logListener = null;

        /// <summary>
        /// Long polling handler options
        /// </summary>
        public LWMessagePushLongPollingOptions LongPollingConfiguration { get; set; }

        /// <summary>
        /// Web socket handler options
        /// </summary>
        public LWMessagePushWebSocketOptions WebSocketConfigurations { get; set; }

        /// <summary>
        /// Logging options, uses DefaultLogLister if not assigned
        /// </summary>
        public LogListenerBase LogListener
        {
            get
            {
                if (_logListener == null)
                    _logListener = new DefaultLogListener();

                return _logListener;

            }
            set { _logListener = value; }
        }
    }
}
