using ChatApp.DTOs;
using ChatApp.Managers;
using LWMessagePush.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Controllers
{
    public class ApiController
    {
        #region Fields
        
        /// <summary>
        /// Instance of LWMessagePush's messaging service
        /// </summary>
        private IMessagingService _messagingService;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagingService">LWMessagePush's IMessagingService instance</param>
        public ApiController(IMessagingService messagingService)
        {
            _messagingService = messagingService;

            // Initialize chat app manager
            ChatAppManager.Instance().Init(new ChatAppManagerSettings()
            {
                MessagingServiceInstance = _messagingService,
                UserPersistenceType = UserPersistenceTypes.InMemory
            });
        }

        /// <summary>
        /// Retrieves the user list
        /// </summary>
        /// <returns>List of UserDTO</returns>
        public List<UserDTO> GetUserList()
        {
            return ChatAppManager.Instance().GetUserList();
        }

        /// <summary>
        /// Sends a message to user
        /// </summary>
        /// <param name="message">Message to be sent</param>
        [HttpPost]
        public void SendMessageToUser(MessageDTO message)
        {
            ChatAppManager.Instance().SendMessageToUser(message);            
        }

        /// <summary>
        /// Appends specified username into pool if the username is available
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Append result</returns>
        [HttpPost]
        public bool Login(string userName)
        {
            return ChatAppManager.Instance().Login(userName);
        }

        /// <summary>
        /// Removes specified username from pool
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Remove result</returns>
        [HttpPost]
        public bool Logout(string userName)
        {
            return ChatAppManager.Instance().Logout(userName);
        }

        /// <summary>
        /// Removes specified username from pool
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Remove result</returns>
        [HttpPost]
        public void Ping(string userName)
        {
            ChatAppManager.Instance().Ping(userName);
        }
    }
}
