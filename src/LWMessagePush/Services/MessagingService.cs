using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using LWMessagePush.DTOs;
using System.Threading.Tasks;
using System.Threading;

namespace LWMessagePush.Services
{
    /// <summary>
    /// IMessagingService implementation
    /// </summary>
    public class MessagingService : IMessagingService
    {
        #region Fields

        IConnectionHandlerFactoryService _connectionHandlerFactory;
        IPersistanceService _persistanceService;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="persistanceService">IPersistanceService instance</param>
        /// <param name="connectionHandlerFactory">IConnectionHandlerFactoryService instance</param>
        public MessagingService(IPersistanceService persistanceService, IConnectionHandlerFactoryService connectionHandlerFactory)
        {
            _persistanceService = persistanceService;
            _connectionHandlerFactory = connectionHandlerFactory;
        }

        #region IMessagingService Members

        /// <summary>
        /// Sends specified message to the subscribers of the given topic via all connection types
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message content</param>
        /// <returns></returns>
        public async Task SendMesageToTopic(string topic, PushMessage message)
        {
            _persistanceService.Save(message);

            Parallel.ForEach(_connectionHandlerFactory.GetConnectionHandlers(), (connHandler) => {

                connHandler.PushMessage(topic, message);

            });

            await Task.CompletedTask; 
        }

        #endregion
    }
}
