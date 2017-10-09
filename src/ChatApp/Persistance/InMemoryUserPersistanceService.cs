using ChatApp.DTOs;
using ChatApp.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Persistance
{
    /// <summary>
    /// In-memory user info persistance provider/service
    /// </summary>
    public class InMemoryUserPersistanceService : IUserPersistanceService
    {
        #region Fields

        /// <summary>
        /// Inmemory user dictionary repository
        /// </summary>
        private ConcurrentDictionary<string, UserDTO> _repository = new ConcurrentDictionary<string, UserDTO>();

        #endregion

        /// <summary>
        /// Clears the user names whose last activities are older than specified datetime
        /// </summary>
        /// <param name="dtVal">Utc datetime info</param>
        /// <returns>Returns true, at least one username is removed</returns>
        public bool ClearUsersBefore(DateTime dtVal)
        {
            List<string> deleteList = new List<string>();

            foreach (var item in _repository)
            {
                if (item.Value.LastActivityDt < dtVal)
                    deleteList.Add(item.Key);
            }

            foreach (var item in deleteList)
            {
                UserDTO tmp;
                _repository.TryRemove(item, out tmp);
            }

            return (deleteList.Count > 0);
        }

        /// <summary>
        /// Retrieves the list of active users
        /// </summary>
        /// <returns>List of active users</returns>
        public List<UserDTO> GetUserList()
        {
            return _repository.Values.ToList<UserDTO>();
        }

        /// <summary>
        /// Removes the specified user name from active users pool
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Remove operation result</returns>
        public bool RemoveUser(string userName)
        {
            UserDTO userDTO;
            return _repository.TryRemove(userName, out userDTO);
        }

        /// <summary>
        /// Appends the specified user name into active users pool
        /// </summary>
        /// <param name="userName">Username of the user</param>
        /// <returns>Append operation result</returns>
        public bool TryAppendUser(string userName)
        {
            if (_repository.ContainsKey(userName))
                return false;
            else
            {
                _repository.TryAdd(userName, new UserDTO() { Username = userName, LastActivityDt = DateTime.UtcNow });
                return true;
            }
        }

        /// <summary>
        /// Updates the last activity date info of the specified user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="newDt">New utc datetime info</param>
        public void UpdateLastActivityDt(string userName, DateTime newDt)
        {
            UserDTO currentUser;
            if (_repository.TryGetValue(userName, out currentUser))
                currentUser.LastActivityDt = newDt;            
        }
    }
}
