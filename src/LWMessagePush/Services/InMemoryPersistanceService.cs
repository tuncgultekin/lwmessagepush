using LWMessagePush.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using LWMessagePush.DTOs;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace LWMessagePush.Services
{
    /// <summary>
    /// In memory implementation of the IPersistanceService
    /// </summary>
    public class InMemoryPersistanceService : IPersistanceService
    {
        #region Fields

        private static Dictionary<string, List<PushMessage>> _storage = new Dictionary<string, List<PushMessage>>();
        private static ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        #endregion

        /// <summary>
        /// Retrieves the list of PushMessages which are newer than specified creationUtcDt
        /// </summary>
        /// <param name="topic">Message topic filter</param>
        /// <param name="creationUtcDt">CreationUtcDt filter</param>
        /// <returns>List of PushMessages</returns>
        public List<PushMessage> Get(string topic, DateTime? creationUtcDt)
        {
            try
            {
                _lock.EnterReadLock();

                List<PushMessage> messages;
                _storage.TryGetValue(topic, out messages);

                if (messages != null)
                    if (creationUtcDt.HasValue)
                        return messages.Where(t => t.CreationUTC > creationUtcDt.Value.ToUniversalTime()).ToList<PushMessage>();
                    else
                        return messages;
                else
                    return new List<PushMessage>();
            }
            finally
            {
                if (_lock.IsReadLockHeld)
                    _lock.ExitReadLock();                
            }
            
        }

        /// <summary>
        /// Removes messages whose creationDt are older than specified createdBeforeUtcDt
        /// </summary>
        /// <param name="topic">Message topic filter</param>
        /// <param name="createdBeforeUtcDt">CreatedBeforeUtcDt filter</param>
        public void Remove(string topic, DateTime? createdBeforeUtcDt)
        {
            try
            {
                _lock.EnterWriteLock();

                List<PushMessage> messages;
                _storage.TryGetValue(topic, out messages);

                if (messages != null)
                    if (createdBeforeUtcDt.HasValue)
                        messages.RemoveAll(t => t.CreationUTC <= createdBeforeUtcDt.Value.ToUniversalTime());
                    else
                        messages.Clear();

                if (messages == null || messages.Count == 0)
                    _storage.Remove(topic);


            }
            finally 
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Stores specified message item
        /// </summary>
        /// <param name="item">PushMessage instance</param>
        public void Save(PushMessage item)
        {
            try
            {
                _lock.EnterWriteLock();

                List<PushMessage> messages;
                _storage.TryGetValue(item.Topic, out messages);

                if (messages != null)
                    messages.Add(item);
                else
                    _storage.Add(item.Topic, new List<PushMessage>() { item });
            }
            finally
            {
                if (_lock.IsWriteLockHeld)
                    _lock.ExitWriteLock();
            }
        }
    }
}
