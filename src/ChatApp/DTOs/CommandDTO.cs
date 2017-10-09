using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.DTOs
{
    /// <summary>
    /// Contains information regarding the operational commands
    /// </summary>
    public class CommandDTO
    {
        /// <summary>
        /// Type of command
        /// </summary>
        public CommandType Command { get; private set; }

        /// <summary>
        /// Command data
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="command">Command type</param>
        /// <param name="data">Command data</param>
        public CommandDTO(CommandType command, string data)
        {
            Command = command;
            Data = data;
        }
    }

    /// <summary>
    /// Command type enumeration, only user list changed available
    /// </summary>
    public enum CommandType
    {
        UserListChanged = 1
    }
}
