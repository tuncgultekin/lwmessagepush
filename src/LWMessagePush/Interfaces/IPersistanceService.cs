using LWMessagePush.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LWMessagePush.Interfaces
{
    /// <summary>
    /// Message persistance service interface
    /// </summary>
    public interface IPersistanceService
    {
        /// <summary>
        /// Stores specified message item
        /// </summary>
        /// <param name="item">PushMessage instance</param>
        void Save(PushMessage item);

        /// <summary>
        /// Retrieves the list of PushMessages which are newer than specified creationUtcDt
        /// </summary>
        /// <param name="topic">Message topic filter</param>
        /// <param name="creationUtcDt">CreationUtcDt filter</param>
        /// <returns>List of PushMessages</returns>
        List<PushMessage> Get(string topic, DateTime? creationUtcDt);

        /// <summary>
        /// Removes messages whose creationDt are older than specified createdBeforeUtcDt
        /// </summary>
        /// <param name="topic">Message topic filter</param>
        /// <param name="createdBeforeUtcDt">CreatedBeforeUtcDt filter</param>
        void Remove(string topic, DateTime? createdBeforeUtcDt);
    }
}
