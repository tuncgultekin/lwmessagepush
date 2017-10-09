using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Logging
{
    /// <summary>
    ///  Base class for LW-MessagePush event logger 
    /// </summary>
    public abstract class LogListenerBase
    {

        #region Internals

        /// <summary>
        /// Filter method, that eliminates incoming logs wrt. active verbosity level
        /// </summary>
        /// <param name="log">Log data</param>
        /// <param name="verbosity">Log verbosity</param>
        internal void Log(string log, LogLevel verbosity)
        {
            try
            {
                if (verbosity >= GetLogLevel())
                    OnLogEventOccured(log, verbosity);
            }
            catch 
            {
                // Prevent from log listener's processing errors
            }

        }

        #endregion

        /// <summary>
        /// Virtual method for logger, which is implemented by host application 
        /// to capture and process LW-MessagePush logs
        /// </summary>
        /// <param name="log">Log data</param>
        /// <param name="verbosity">Log verbosity</param>
        public abstract void OnLogEventOccured(string log, LogLevel verbosity);

        /// <summary>
        /// Retrieves active log level from host application's context 
        /// </summary>
        /// <returns>Active log verbosity</returns>
        public abstract LogLevel GetLogLevel();

    }
}
