using LWMessagePush.DTOs;
using LWMessagePush.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class LWMessagePushUnitTests
    {
        private PushMessage GetTempMessage()
        {
            return new PushMessage()
            {
                MessageId = Guid.NewGuid(),
                Content = "Hello Test",
                Topic = "ChatUser"
            };
        }

        [Fact]
        public void SaveMessageTest()
        {
            var pers = new InMemoryPersistanceService();
            var message = GetTempMessage();
            pers.Save(message);
            var list = pers.Get(message.Topic, message.CreationUTC.AddSeconds(-1));
            Assert.Contains(message, list);
        }

        [Fact]
        public void RemoveMessageTest()
        {
            var pers = new InMemoryPersistanceService();
            var message = GetTempMessage();
            pers.Save(message);
            pers.Remove(message.Topic, message.CreationUTC.AddMinutes(-1));
            var list = pers.Get(message.Topic, message.CreationUTC.AddMinutes(-1));
            Assert.Contains(message, list);
            pers.Remove(message.Topic, message.CreationUTC.AddMinutes(1));
            list = pers.Get(message.Topic, message.CreationUTC);
            Assert.DoesNotContain(message, list);
        }

        [Fact]
        public void GetMessageTest()
        {
            var pers = new InMemoryPersistanceService();
            var message = GetTempMessage();
            pers.Save(message);
            var list = pers.Get(message.Topic, message.CreationUTC.AddSeconds(-1));
            Assert.Contains(message, list);
            list = pers.Get(message.Topic, message.CreationUTC.AddSeconds(1));
            Assert.DoesNotContain(message, list);
        }

    }

}
