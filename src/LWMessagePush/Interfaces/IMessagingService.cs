using LWMessagePush.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LWMessagePush.Interfaces
{
    /// <summary>
    /// Messaging service interface
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// Sends specified message to the subscribers of the given topic via all connection types
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message content</param>
        /// <returns></returns>
        Task SendMesageToTopic(string topic, PushMessage message);
    }
}
