using LWMessagePush.Handlers;
using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LWMessagePush.Services
{
    /// <summary>
    /// Connection handler factory implementation
    /// </summary>
    public class ConnectionHandlerFactoryService : IConnectionHandlerFactoryService
    {
        #region Fields

        private readonly IPersistanceService _persistanceService;
        private readonly List<IConnectionHandler> _connectionHandlers;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="persistanceService">IPersistanceService implementation</param>
        public ConnectionHandlerFactoryService(IPersistanceService persistanceService)
        {
            _persistanceService = persistanceService;
            _connectionHandlers = new List<IConnectionHandler>();

            _connectionHandlers.Add(new LongPollingHandler(persistanceService));
            _connectionHandlers.Add(new WebSocketHandler(persistanceService));
        }

        /// <summary>
        /// Retrieves a specific connection handler by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>IConnectionHandler implementation</returns>s
        public IConnectionHandler GetConnectionHandlerByName(string name)
        {
            return _connectionHandlers.Where(t => t.Name == name).FirstOrDefault();
        }


        /// <summary>
        /// Retrieves the list of all available connection handlers
        /// </summary>
        /// <returns>List of IConnectionHandler implementations</returns>
        public List<IConnectionHandler> GetConnectionHandlers()
        {
            return _connectionHandlers;
        }
    }
}
