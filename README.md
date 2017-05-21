[![NuGet version](https://badge.fury.io/nu/VBLock.svg)](https://badge.fury.io/nu/VBLock) 
Lw-MessagePush
=========
Lw-MessagePush is a highly customizable and light-weight, message pushing library for .NET Core web applications.
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
 That's all to run LWMessagePush, for further details, please checkout SampleApp...
