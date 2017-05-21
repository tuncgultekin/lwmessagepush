using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Interfaces
{
    /// <summary>
    /// Connection handler factory interface
    /// </summary>
    public interface IConnectionHandlerFactoryService
    {
        /// <summary>
        /// Retrieves the list of all available connection handlers
        /// </summary>
        /// <returns></returns>
        List<IConnectionHandler> GetConnectionHandlers();

        /// <summary>
        /// Retrieves a specific connection handler by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IConnectionHandler GetConnectionHandlerByName(string name);
    }
}
