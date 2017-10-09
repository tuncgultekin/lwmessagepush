using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Logging
{
    /// <summary>
    ///  Default log listener for LW-MessagePush event logger 
    /// </summary>
    public class DefaultLogListener : LogListenerBase
    {
        /// <summary>
        /// Virtual method for logger, which is implemented by host application 
        /// to capture and process LW-MessagePush logs
        /// </summary>
        /// <param name="log">Log data</param>
        /// <param name="verbosity">Log verbosity</param>
        public override void OnLogEventOccured(string log, LogLevel verbosity)
        {
            // Do nothing as default
        }

        /// <summary>
        /// Return Error level as default
        /// </summary>
        /// <returns>Error verbosity</returns>
        public override LogLevel GetLogLevel()
        {
            return LogLevel.Error;
        }

    }
}
