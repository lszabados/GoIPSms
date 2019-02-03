# Server

## Initialization

### Dependency injection

Use in the Program.cs ConfigureServices section 

````c#
services
	.Configure<GoIPSmsServerOptions>(option => {
		option.AuthPassword = "XXXXXX";
		option.Port = 44444;
		option.ServerId = "XXXX";
	})
	.AddLogging(configure => configure
				.AddConsole()
				.AddDebug()
				.AddFilter("Voxo.GoIpSmsServer.GoIPSmsServer", LogLevel.Information)
	)
	.AddSingleton<GoIPSmsServer>();
````

Create instance:

````c#
var server = serviceProvider.GetService<GoIPSmsServer>();
````

### Not Dependency injection

Direct create

````c#
var server = new GoIPSmsServer(new GoIPSmsServerOptions()
				{ AuthPassword = "hhh", Port = 44444, ServerId = "lkljkl" }, 
				new NullLogger<GoIPSmsServer>());
````

## Start server

Use `StartServer()` to start the server.

## Stop server

Use `StopServer()` to stop the server.

## Server State

The status of the server is included in the ``Status`` field

````c#
public enum ServerStatus : byte { Stopped, Started, Stopping, Starting }
````


## Events

Raise `OnRegistration` when receive req data stream from GoIP device. GOIP will send a keepalive packet to server every 30s

Raise `OnStateChange` when receive STATE data stream from GoIP device. When status of channel of goip changed, goip send the status to server.

Raise `OnMessage` when receive RECEIVE data stream from GoIP device. When received SMS, Goip will relay the SMS to Server.

Raise `OnDeliveryReport` when receive DELIVER data stream from GoIP device. If carrier report delivery of sms sending, goip will send delivery report to sms server

Raise `OnRecord` when receive RECORD data stream from GoIP device. When Goip in a call, goip send status of call to server

Raise `OnRemain` when receive REMAIN data from GoIP device. After each call ,goip send remain time to server

Raise `OnCellListChanged` when receive changed cell list from GoIP device.
