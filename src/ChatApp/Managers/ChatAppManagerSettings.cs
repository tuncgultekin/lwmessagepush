using ChatApp.Interfaces;
using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Managers
{
    /// <summary>
    /// Contains the configuration info of ChatAppManager
    /// </summary>
    public class ChatAppManagerSettings
    {
        /// <summary>
        /// LWMessagePush service instance
        /// </summary>
        public IMessagingService MessagingServiceInstance { get; set; }

        /// <summary>
        /// UserPersistance provider type
        /// </summary>
        public UserPersistenceTypes UserPersistenceType { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ChatAppManagerSettings()
        {
            UserPersistenceType = UserPersistenceTypes.InMemory;
        }
    }

    /// <summary>
    /// User info persistance provider types
    /// </summary>
    public enum UserPersistenceTypes
    {
        InMemory
    }
}
