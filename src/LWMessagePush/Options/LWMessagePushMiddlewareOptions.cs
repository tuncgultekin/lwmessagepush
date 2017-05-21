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
        /// <summary>
        /// Long polling handler options
        /// </summary>
        public LWMessagePushLongPollingOptions LongPollingConfiguration { get; set; }

        /// <summary>
        /// Web socket handler options
        /// </summary>
        public LWMessagePushWebSocketOptions WebSocketConfigurations { get; set; }
    }
}
