using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Options
{
    /// <summary>
    /// LongPolling options
    /// </summary>
    public class LWMessagePushLongPollingOptions
    {
        /// <summary>
        /// Long polling timeout
        /// </summary>
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public LWMessagePushLongPollingOptions()
        {
            TimeoutSeconds = 10;
        }
    }
}
