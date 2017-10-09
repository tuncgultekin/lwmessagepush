using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.DTOs
{
    /// <summary>
    /// Contains the information about user
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Last ping activity utc date time
        /// </summary>
        public DateTime LastActivityDt { get; set; }
    }
}
