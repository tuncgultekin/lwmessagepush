[![NuGet version](https://badge.fury.io/nu/VBLock.svg)](https://badge.fury.io/nu/VBLock) 
LW-MessagePush
=========
LW-MessagePush is a highly customizable and light-weight, message pushing library for .NET Core web applications.
It supports both websocket and long polling connection methods with auto-fallback feature.

Usage
-----------
Modify your Startup.cs:
1) Add services.AddLWMessagePushDefaultServices() to the beginning of the ConfigureServices method
```sh
public void ConfigureServices(IServiceCollection services)
{
    // Add LWMessagePush services
    services.AddLWMessagePushDefaultServices();

    ...
}
```
2) Add app.UseLWMessagePush() to Configure method
```sh
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    ...
    
    // Add LWMessagePush Middleware to the pipeline
    app.UseLWMessagePush();
    
    ...
}
```

3) To send a message to subscribers of a topic:
```sh
...

var message = new LWMessagePush.DTOs.PushMessage()
{
	MessageId = Guid.NewGuid(),
	Topic = "TopicThatYouWantToSendMessage",
	Content = "MessageContent"
};

await _messagingService.SendMesageToTopic(topic, message);

...
```

For client side code (any html page or mvc view):
1) Add LWMessagePush client script to your page right after the JQuery script. Client script is automatically served from 
http://{your_application_path}/**__lwpush/client**
```sh
<script src="https://code.jquery.com/jquery-3.2.1.min.js"></script>
<script src="http://{your_application_path}/__lwpush/client"></script>
```
2) Configure LWMessagePush client and connect:
```sh
<script>
	var client;
	$(function () {            
		client = new LWMessagePushClient(
			function (msg) {
				// onMessageReceive callback
			},
			"http://{your_application_path}",
			"TopicThatYouWantToListen",
			LWMessagePushConnectionMethod.Automatic, // Also you may choose WebSocket or LongPolling
			function (log) {
				// onLogMessageReceive callback
			});
			
		client.connect();
	});
</script>
```
3) To disconnect:
```sh
<script>
	...
	client.disconnect();
	...
</script>
```
 That's all to run LWMessagePush, for further details, please checkout SampleApp.

Customization
-----------
LW-MessagePush injects 3 default services as:

```sh
public static void AddLWMessagePushDefaultServices(this IServiceCollection services)
{
    services.AddSingleton<IPersistanceService, InMemoryPersistanceService>();
    services.AddSingleton<IConnectionHandlerFactoryService, ConnectionHandlerFactoryService>();
    services.AddSingleton<IMessagingService, MessagingService>();
}
```
**IPersistanceService** is responsible for message persistance and all messages are kept in the memory (with InMemoryPersistanceService). You may want to change this behaviour and persist messages to a db. To do so you can create your custom persistance service which implements IPersistanceService and inject it as Singleton.

**IConnectionHandlerFactoryService** is used for to retrieve connection handler types such as WebSocketConnectionHandler, LongPollingConnectionHandler. If you need to use your own connection method, you may customize it. 

**IMessagingService** is used for to send a message to all opened connections. 
