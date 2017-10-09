using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApp.DTOs;

namespace ChatApp.Interfaces
{
    /// <summary>
    /// An interface for the methods of user data persistance (services)
    /// </summary>
    public interface IUserPersistanceService
    {
        /// <summary>
        /// Retrieves the list of active users
        /// </summary>
        /// <returns>List of active users</returns>
        List<UserDTO> GetUserList();

        /// <summary>
        /// Appends the specified user name into active users pool
        /// </summary>
        /// <param name="userName">Username of the user</param>
        /// <returns>Append operation result</returns>
        bool TryAppendUser(string userName);

        /// <summary>
        /// Removes the specified user name from active users pool
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>Remove operation result</returns>
        bool RemoveUser(string userName);

        /// <summary>
        /// Updates the last activity date info of the specified user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="dtInfo">New utc datetime info</param>
        void UpdateLastActivityDt(string userName, DateTime dtInfo);

        /// <summary>
        /// Clears the user names whose last activities are older than specified datetime
        /// </summary>
        /// <param name="dtInfo">Utc datetime info</param>
        /// <returns>Returns true, at least one username is removed</returns>
        bool ClearUsersBefore(DateTime dtInfo);
    }
}
