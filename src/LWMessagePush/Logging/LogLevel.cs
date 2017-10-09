using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Logging
{
    /// <summary>
    /// Specifies the verbosity level of logs
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///  Allows processing of all log types
        /// </summary>
        Verbose = 0,

        /// <summary>
        ///  Allows processing of Info and Error log types
        /// </summary>
        Info = 1,

        /// <summary>
        ///  Allows processing of Error typed logs
        /// </summary>
        Error = 2
    }
}
