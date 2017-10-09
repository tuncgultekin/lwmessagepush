[![NuGet version](https://badge.fury.io/nu/LWMessagePush.svg)](https://badge.fury.io/nu/LWMessagePush)
LW-MessagePush & Sample Chat Application
=========
LW-MessagePush is a highly customizable and light-weight, message pushing library for cross-platform .NET Core web applications. It supports both websocket and long polling connection methods with auto-fallback feature.

LW-MessagePush provides both font-end and back-end libraries. Through its front-end JavaScript library, any web UI framework technology that makes use of JavaScript, can adapt it easily.

As backend framework, .Net Core platform is preferred because of its cross-platform web framework features and dependency injection capabilities for customization features. 

To demonstrate functionality and usage of the LW-MessagePush, a simple test web application and Sample Chat Application is developed. Simple test application is a solid example of “Usage” instructions, which are described later sections in below. On the other hand, Sample Chat Application is a real-world usage example of LW-MessagePush. It includes both a single page application for front-end and a rest service back-end.

In this document; architecture components, customization, scalability features and usage details of LW-MessagePush and Sample Chat Application are explained.

Live Sample Chat Application: http://lwmessagepushchat.azurewebsites.net/

Sample Chat Application Demo Video: https://www.youtube.com/watch?v=J2U0j03enB4


Architecture
-----------
LW-MessagePush takes advantages of IoC pattern, each of the connection handlers and persistence services are injected by interfaces. So, it can be easily customized. Currently LW-MessagePush supports WebSocket and Long Polling connection methods and in-memory message persistence layer. 
LW-MessagePush Project Namespaces:
* DTOs:
  * PushMessage.cs :  Message data container class, that includes the properties of transferred messages.

* Embeded:
  * Client.js : Javascript client of LW-MessagePush, it provides functions for websocket and long polling connection schemas and auto-fallback features. Client.js maintained as embedded resource dynamically served from the library.

* Handlers:
  * LongPollingHander.cs : Handler class for long polling connection requests. It waits incoming requests until a signal is received (new message) or a configurable timeout is occurred.

  * WebSocketHandler.cs : Handler class for web socket connection requests. It keeps the track of all socket connections and parallelly sends new messages to them.

* Interfaces:
  * IConnectionHandler.cs : Interface of connection handler types such as WebSocketHandler and LongPollingHandler. By implementing this interface, a new connection handler can easily be added to system.
  * IConnectionHandlerFactoryService.cs : Interface for ConnectionHandlerFactory implementation. By implementing this interface, a new ConnectionHandlerFactory can be created.

  * IMessageService.cs : Interface of main message service class. MessageService manages broadcasting process of messages to topics and this process can be customized by creating the new implementation of this interface.

  * IPersistanceService.cs : Interface of message persistence methods. Currently only one implementation of this interface that performs in-memory persistence is implemented.

* Logging : 
  * LogLevel.cs : Enumeration that defines logging level of LW-MessagePush
  
  * LogListenerBase.cs : Abstract class that filters and redirects logs to host app's context wrt. active log level
  
  * DefaultLogListener.cs : Default instance of LogListenerBase, that simply ignores all log data.

* Middlewares : 
  * LWMessagePushMiddleware.cs : Request router class, that accepts requests (websocket, long polling, embedded javascript content) from clients and routes them to the appropriate handlers.

  * LWMessagePushMiddlewareExtensions.cs : Configurator class, that makes default service injections (InMemoryPersitanceService, ConnectionHandlerFactoryService, MessagingService).

* Options : 
  * LWMessagePushLongPollingOptions.cs : Settings for LongPollingConnectionHandler.

  * LWMessagePushWebSocketOptions.cs : Settings for WebSocketConnectionHandler. 

  * LWMessagePushMiddleOptions.cs : Settings for LWMessagePush

* Services :
  * ConnectionHandlerFactoryService.cs : Default implement of IConnectionHandlerFactory that is responsible for creation process of ConnectionHandlers.

  * InMemoryPersistanceService.cs : In-memory implementation of the IPersistanceService which is responsible for persistence operations of incoming messages.

  * MessagingService.cs : Default implementation of main message service class. It manages broadcasting process of messages to topics.


Security
-----------
Depending on the http security of host app, front-end library automatically selects “ws” and “wss” websocket protocols.


Scalability
-----------
Main scalability limitations of the applications that utilize LW-MessagePush are message persistence and inter-process signaling. Currently LW-MessagePush supports only in-memory message persistence and it is not appropriate for multi-processes environments. However, this limitation can be overcome by creating and injecting of centralized databased based implementation of IPersistanceService. Inter-process signaling is required to notify other processes for new messages. To do so; SendMesageToTopic in the implementation of IMessageService must be updated. Please refer to “Customization” section for further details.


Logging
-----------
LW-MessagePush supports both front-end back-end request logging. Front-end message logging is handled via “onLogMessageReceive”  JavaScript function callback. Back-end request logging is activated by extending LogListenerBase class and providing an instance of it to LogListener property of LWMessagePushMiddlewareOptions in LW-MessagePush initialization. Please take a look at to Startup.cs of Sample Chat Application for further details.


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
 That's all to run LWMessagePush, for further details, please check out SampleApp.

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


Sample Chat Application
=========
To demonstrate the usage and functionality of LW-MessagePush a Sample Chat application is developed.

Sample Chat Application is designed as a single page application.  Its front-end consist only three files; a single html file (index.html), a javascript file (app.js) and a css file (app.css). All front-end operations are handled by app.js file. As backend; Sample Chat Application includes a rest service (ApiController.cs) that makes use of LWMessagePush. This service is responsible for user login/logout operations, messaging facilities and ping operations.

Architecture
-----------
* Wwwroot : Contains static frontend files
  * Index.html : Main UI view
  * App.js : Main javascript file that coordinates UI operations
  * App.css : Css file for main UI view

* Controllers : 
  * ApiController.cs : Rest api for Chat backend operations. Please take a look at the summary parts of the methods for rest api operation details.

  * HomeControlle.cs : Auxiliary Asp.Net controller which is responsible for default page (Index.html) redirection.

* DTOs : 
  * CommandDTO.cs : Class that contains the information regarding the operational commands.

  * MessageDTO.cs : Class that contains the information about chat messages.

  * UserDTO.cs : Class which contains the information about an active user.

* Interfaces : 
  * IUserPersistanceService.cs : Interface of user data persistence service. By implementing this interface new persister types such that MongoDb user data persister can be implemented.

* Logging : 
  * ChatAppLWMessagePushLogListener.cs :  LW-MessagePush log listener implementation, that writes Verbose level log messages to console.
  
* Managers :
  * ChatAppManager.cs : Main backend class, it takes the advantages of singleton design pattern and manages all backend operations such  as, creation of appropriate UserPersistanceService, broadcasting of messages through LWMessagePush library and login/logout operations.

  * ChatAppManagerSettings : Contains the configuration info (user persistence type) of ChatAppManager

* Persistence : 
  * InMemoryUserPersistenceService.cs : Keeps the track of active users’ information in memory.

Security (Sample Chat App)
-----------
Sample chat app does not have a password protected login and session facility since it is just an example. Thus, anybody can start a chat session with an any actively unused username.

Scalability (Sample Chat App)
-----------
To make Sample Chat Application scalable, firstly LW-MessagePush scalability procedure must be performed. To do so, please refer to “Scalability” section of LW-MessagePush. After that for Sample Chat Application, centralized database based implementation of IUserPersistanceService must be created and utilized in ChatAppManager class.

Unit Tests & Integration Tests
-----------
Unit tests of InMemoryPersistenceService of LW-MessagePush and integration tests of Sample Chat App’s rest service are in UnitTests folder. 

