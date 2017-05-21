using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LWMessagePush.Interfaces;

namespace SampleApp.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        private IMessagingService _messagingService;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagingService">LWMessagePush's IMessagingService instance</param>
        public HomeController(IMessagingService messagingService)
        {
            _messagingService = messagingService;
        }

        /// <summary>
        /// Default View
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Message sending endpoint
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="msg">Message content</param>
        /// <returns></returns>
        public async Task<ActionResult> SendMessage([FromQuery]string topic, [FromQuery]string msg)
        {
            var message = new LWMessagePush.DTOs.PushMessage()
            {
                MessageId = Guid.NewGuid(),
                Topic = topic,
                Content = msg
            };

            await _messagingService.SendMesageToTopic(topic, message);

            return new JsonResult(message);
        }
    }
}