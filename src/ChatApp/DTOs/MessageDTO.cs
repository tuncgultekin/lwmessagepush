using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.DTOs
{
    /// <summary>
    /// Contains the information about chat messages
    /// </summary>
    public class MessageDTO
    {
        /// <summary>
        /// Message target
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Message sender
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        public string Content { get; set; }
    }
}
