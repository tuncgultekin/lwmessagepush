using System;
using Microsoft.AspNetCore.Builder;
using LWMessagePush.Options;
using Microsoft.Extensions.DependencyInjection;
using LWMessagePush.Interfaces;
using LWMessagePush.Services;

namespace LWMessagePush.Middlewares
{
    /// <summary>
    /// LWMessagePush .net core middleware extensions
    /// </summary>
    public static class LWMessagePushMiddlewareExtensions
    {
        /// <summary>
        /// Adds LWMessagePush middleware to the pipeline
        /// </summary>
        /// <param name="builder">IApplicationBuilder instance</param>
        /// <param name="options">LWMessagePush middleware options</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLWMessagePush(this IApplicationBuilder builder, LWMessagePushMiddlewareOptions options = null)
        {
            // Default configuration
            if (options == null)
                options = new LWMessagePush.Options.LWMessagePushMiddlewareOptions()
                {
                    LongPollingConfiguration = new LWMessagePush.Options.LWMessagePushLongPollingOptions() { TimeoutSeconds = 10 },
                    WebSocketConfigurations = new LWMessagePush.Options.LWMessagePushWebSocketOptions()
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(120),
                        ReceiveBufferSize = 4 * 1024
                    }
                };

            if (options.WebSocketConfigurations == null)
                throw new ArgumentNullException(nameof(options.WebSocketConfigurations));

            if (options.LongPollingConfiguration == null)
                throw new ArgumentNullException(nameof(options.LongPollingConfiguration));

            builder.UseWebSockets(options.WebSocketConfigurations);

            return builder.UseMiddleware<LWMessagePushMiddleware>(builder, options);
        }

        /// <summary>
        /// Adds LWMessagePush services
        /// </summary>
        /// <param name="services">The services.</param>
        public static void AddLWMessagePushDefaultServices(this IServiceCollection services)
        {
            services.AddSingleton<IPersistanceService, InMemoryPersistanceService>();
            services.AddSingleton<IConnectionHandlerFactoryService, ConnectionHandlerFactoryService>();
            services.AddSingleton<IMessagingService, MessagingService>();
        }

    }
}
