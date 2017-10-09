using ChatApp.DTOs;
using ChatApp.Interfaces;
using ChatApp.Persistance;
using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace ChatApp.Managers
{
    public class ChatAppManager
    {
        #region Fields

        /// <summary>
        /// Miliseconds period value for garbage collector timer
        /// </summary>
        private const int GARBAGE_COLLECTOR_PERIOD_MS = 10000;

        /// <summary>
        /// Miliseconds value for signing users as old
        /// </summary>
        private const int OLD_USER_ACTIVITY_LIMIT_MS = 25000;

        /// <summary>
        /// Static singleton instance
        /// </summary>
        private static ChatAppManager _instance = null;

        /// <summary>
        /// Timer task for garbage collection
        /// </summary>
        private Timer _garbageCollectionTimer;

        /// <summary>
        /// Settings insance
        /// </summary>
        private ChatAppManagerSettings _settings = null;

        /// <summary>
        /// User persistance service instance
        /// </summary>
        public IUserPersistanceService _userPersistanceServiceInstance { get; set; }

        #endregion

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private ChatAppManager() { }

        /// <summary>
        /// Singleton Method
        /// </summary>
        /// <returns>Instance of ChatAppManager</returns>
        public static ChatAppManager Instance()
        {
            if (_instance == null)
                _instance = new ChatAppManager();

            return _instance;
        }

        /// <summary>
        /// Initializes the manager
        /// </summary>
        /// <param name="settings">Settings instance</param>
        public void Init(ChatAppManagerSettings settings)
        {
            if (IsInitialized())
                return;

            if (settings.UserPersistenceType == UserPersistenceTypes.InMemory)
                _userPersistanceServiceInstance = new InMemoryUserPersistanceService();
            else
                throw new NotImplementedException("User persitance provider not found: " + settings.UserPersistenceType.ToString());

            _settings = settings;

            _garbageCollectionTimer = new Timer(new TimerCallback((state) =>
            {
                if (_userPersistanceServiceInstance.ClearUsersBefore(DateTime.UtcNow.AddMilliseconds(-1* OLD_USER_ACTIVITY_LIMIT_MS)))
                {
                    // Notify web users for updating their lists
                    NotifyUsersForListChange();
                }

            }), null, 0, GARBAGE_COLLECTOR_PERIOD_MS);
        }

        /// <summary>
        /// Inserts a new user into pool if the username is available
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Operation result</returns>
        public bool Login(string userName)
        {
            var result = _userPersistanceServiceInstance.TryAppendUser(userName);

            if (result)
            {
                // Notify web users for updating their lists
                NotifyUsersForListChange();
            }

            return result;
        }

        /// <summary>
        /// Updates the LastActivity datetime of the specified user
        /// </summary>
        /// <param name="userName">Username</param>
        public void Ping(string userName)
        {
            _userPersistanceServiceInstance.UpdateLastActivityDt(userName, DateTime.UtcNow);
        }

        /// <summary>
        /// Removes a user from pool
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Operation result</returns>
        public bool Logout(string userName)
        {
            var result = _userPersistanceServiceInstance.RemoveUser(userName);

            if (result)
            {
                // Notify web users for updating their lists
                NotifyUsersForListChange();
            }

            return result;
        }

        /// <summary>
        /// Sends a push message to user
        /// </summary>
        /// <param name="message">Message</param>
        public void SendMessageToUser(MessageDTO message)
        {
            IsInitialized(throwExp: true);
            
            _settings.MessagingServiceInstance.SendMesageToTopic(message.To, new LWMessagePush.DTOs.PushMessage()
            {
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(message),
                MessageId = Guid.NewGuid(),
                Topic = message.To
            });
        }

        /// <summary>
        /// Retrieves the list of active users
        /// </summary>
        /// <returns>List of UserDTO</returns>
        public List<UserDTO> GetUserList()
        {
            IsInitialized(throwExp: true);

            return _userPersistanceServiceInstance.GetUserList();
        }

        /// <summary>
        /// Checks the initialization status of ChatAppManager instance
        /// </summary>
        /// <param name="throwExp">Determins, whether an exception is going to be thrown in case of an uninitialized manager exists</param>
        /// <returns>Initialization result</returns>
        public bool IsInitialized(bool throwExp= false)
        {
            var result = (_settings != null);

            if (throwExp && !result)
                throw new Exception("ChatAppManager has not been initialized yet.");

            return result;
        }

        #region Private

        /// <summary>
        /// Sends a notification to all connected clients to update their user lists
        /// </summary>
        private void NotifyUsersForListChange()
        {
            _settings.MessagingServiceInstance.SendMesageToTopic("updateUsersReq", new LWMessagePush.DTOs.PushMessage()
            {
                MessageId = Guid.NewGuid(),
                Topic = "updateUsersReq",
                Content = Newtonsoft.Json.JsonConvert.SerializeObject(new CommandDTO(CommandType.UserListChanged, null))
            });
        }

        #endregion

    }
}
