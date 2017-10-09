using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LWMessagePush.Options;
using System.Net.WebSockets;
using System.Threading;
using System.Collections.Concurrent;
using LWMessagePush.Handlers;
using LWMessagePush.Interfaces;
using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Reflection;

namespace LWMessagePush.Middlewares
{
    /// <summary>
    /// LWMessagePushMiddleware main middleware
    /// </summary>
    public class LWMessagePushMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;
        private readonly LWMessagePushMiddlewareOptions _options;        
        private readonly IConnectionHandlerFactoryService _connectionHandlerFactory;
        private static string _clientCode;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        public LWMessagePushMiddleware(RequestDelegate next, IApplicationBuilder builder, LWMessagePushMiddlewareOptions options)
        {
            _next = next;
            _options = options;            
            _connectionHandlerFactory = (IConnectionHandlerFactoryService)builder.ApplicationServices.GetService(typeof(IConnectionHandlerFactoryService));
            _clientCode = null;
        }

        /// <summary>
        /// Middleware request handler, routes requests to the related connection handlers
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {           
            if (context.Request.Path.Value.EndsWith("/__lwpush/ws"))
            {                
                await _connectionHandlerFactory.GetConnectionHandlerByName("WebSocketHandler").HandleRequest(context, _options);
            }
            else if (context.Request.Path.Value.EndsWith("/__lwpush/poll"))
            {
                await _connectionHandlerFactory.GetConnectionHandlerByName("LongPollingHandler").HandleRequest(context, _options);
            }
            else if (context.Request.Path.Value.EndsWith("/__lwpush/client"))
            {
                if (_clientCode == null)
                {
                    var assembly = typeof(LWMessagePushMiddleware).GetTypeInfo().Assembly;
                    Stream resource = assembly.GetManifestResourceStream("LWMessagePush.Embedded.client.js");
                    StreamReader reader = new StreamReader(resource);
                    _clientCode = reader.ReadToEnd();                    
                }
                context.Response.ContentType = "text/javascript";
                await context.Response.WriteAsync(_clientCode);
            }
            else
                await _next(context);
        }
    }
}
