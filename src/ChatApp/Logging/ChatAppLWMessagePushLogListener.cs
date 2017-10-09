using LWMessagePush.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Logging
{
    /// <summary>
    /// ChatApp specific log listener class for LWMessagePush
    /// </summary>
    public class ChatAppLWMessagePushLogListener : LogListenerBase
    {
        /// <summary>
        /// Current log level is set as Verbose
        /// </summary>
        /// <returns>LogLevel.Verbose</returns>
        public override LWMessagePush.Logging.LogLevel GetLogLevel()
        {
            return LWMessagePush.Logging.LogLevel.Verbose;
        }

        /// <summary>
        /// Custom log event processor
        /// </summary>
        /// <param name="log">Log data</param>
        /// <param name="verbosity">Log level</param>
        public override void OnLogEventOccured(string log, LWMessagePush.Logging.LogLevel verbosity)
        {
            Console.WriteLine(string.Format("{0}: {1}", verbosity.ToString(), log));
        }
    }
}
