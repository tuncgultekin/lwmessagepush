using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Options
{
    /// <summary>
    /// Web socket middleware options, current there is not any extra configuration,
    /// Inherited from WebSocketOptions for further configuration additions
    /// </summary>
    public class LWMessagePushWebSocketOptions:WebSocketOptions
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public LWMessagePushWebSocketOptions():base()
        {
            KeepAliveInterval = TimeSpan.FromSeconds(120);
            ReceiveBufferSize = 4 * 1024;
        }
    }
}
