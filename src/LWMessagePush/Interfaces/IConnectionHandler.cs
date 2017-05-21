using LWMessagePush.DTOs;
using LWMessagePush.Options;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LWMessagePush.Interfaces
{
    /// <summary>
    /// Connection handler interface
    /// </summary>
    public interface IConnectionHandler
    {
        /// <summary>
        /// ConnectionHandler identifier
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Process request
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="options">Middleware options</param>
        /// <returns></returns>
        Task HandleRequest(HttpContext context, LWMessagePushMiddlewareOptions options);

        /// <summary>
        /// Sends specified message to the subscribers of the given topic      
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message content</param>
        /// <returns></returns>
        Task PushMessage(string topic, PushMessage message);

    }
}
