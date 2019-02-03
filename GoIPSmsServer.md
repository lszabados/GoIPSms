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

	);
````

## Start server

Use ˙˙StartServer()˙˙ to start the server.

## Stop server

Use ˙˙StopServer()˙˙ to stop the server.

## Server State

The status of the server is included in the ``Status`` field

````c#
public enum ServerStatus : byte { Stopped, Started, Stopping, Starting }
````

