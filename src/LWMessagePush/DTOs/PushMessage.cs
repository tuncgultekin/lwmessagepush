using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.DTOs
{
    /// <summary>
    /// Message container class
    /// </summary>
    public class PushMessage 
    {
        /// <summary>
        /// Id of the message
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// Message payload
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Message topic
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Message creation utc date time
        /// </summary>
        public DateTime CreationUTC { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public PushMessage()
        {
            CreationUTC = DateTime.UtcNow;
        }

    }
}
