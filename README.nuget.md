# Jiro.Shared

Shared types and models for the Jiro ecosystem applications.

## Overview

Jiro.Shared provides common types, models, and utilities used across the Jiro ecosystem, including JiroCloud and Jiro instances. This package contains shared WebSocket endpoints, gRPC service definitions, request/response models, and other foundational components that ensure consistent communication between different parts of the Jiro system.

## Features

- **WebSocket Endpoints**: Predefined endpoint constants for real-time communication
- **gRPC Definitions**: Protocol buffer definitions for efficient service communication
- **TaskManager Infrastructure**: Unified task management for distributed systems
- **Shared Models**: Common request and response data transfer objects (DTOs)
- **Proto File Distribution**: jiroHub.proto included for gRPC service generation
- **Type Safety**: Strongly-typed contracts for reliable inter-service communication
- **Documentation**: Full XML documentation for excellent IntelliSense support

## Installation

### Package Manager

```powershell
Install-Package Jiro.Shared
```

### .NET CLI

```bash
dotnet add package Jiro.Shared
```

### Package Reference

```xml
<PackageReference Include="Jiro.Shared" Version="1.4.5" />
```

## Usage

### WebSocket Endpoints

```csharp
using Jiro.Shared.Websocket;

// Server to client events
var commandEndpoint = Endpoints.Incoming.ReceiveCommand;
var logsEndpoint = Endpoints.Incoming.GetLogs;

// Client to server events  
var responseEndpoint = Endpoints.Outgoing.LogsResponse;
var errorEndpoint = Endpoints.Outgoing.ErrorResponse;

// Connection lifecycle
var closedEvent = Endpoints.Lifecycle.Closed;
```

### Request/Response Models

```csharp
using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

// Command message
var command = new CommandMessage
{
    Command = "help",
    SessionId = "session-123"
};

// Session response
var sessionResponse = new SessionResponse
{
    SessionId = "session-123",
    IsActive = true
};
```

### TaskManager Infrastructure (v1.4.0+)

```csharp
using Jiro.Shared.Tasks;
using Microsoft.Extensions.DependencyInjection;

// Register TaskManager
services.Configure<TaskManagerOptions>(options => 
{
    options.DefaultTimeoutSeconds = 30;
    options.MaxPendingTasks = 1000;
});
services.AddSingleton<ITaskManager, TaskManager>();

// Use TaskManager
var taskManager = serviceProvider.GetService<ITaskManager>();
var result = await taskManager.ExternalExecuteAsync<MyResponse>(
    instanceId: "instance-1",
    requestId: "req-123", 
    action: () => SendCommandAsync()
);
```

### SynchronizationToken Model (v1.4.0+)

```csharp
using Jiro.Shared;

// Enhanced command tracking
var token = new SynchronizationToken
{
    InstanceId = "instance-1",
    SessionId = "session-123", 
    RequestId = "req-456"
};

// Use with command responses
var response = new SessionCommandResponse
{
    SynchronizationToken = token,
    CommandType = CommandType.Text,
    Result = new TextResult { Response = "Hello World" }
};
```

### gRPC Proto File Usage (v1.4.0+)

The jiroHub.proto file is automatically included in your project:

```xml
<ItemGroup>
  <Protobuf Include="Grpc\jiroHub.proto" GrpcServices="Client" />
</ItemGroup>
```

Generate gRPC services:

```csharp
using Jiro.Shared.Grpc;

// Use generated gRPC client
var client = new JiroHubProto.JiroHubProtoClient(channel);
var response = await client.SendCommandResultAsync(clientMessage);
```

## Documentation

For comprehensive documentation, examples, and API reference, visit:
<https://huebyte.github.io/Jiro.Shared/>

## Contributing

Contributions are welcome! Please see our Contributing Guidelines for details.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

- **Issues**: <https://github.com/HueByte/Jiro.Shared/issues>
- **Discussions**: <https://github.com/HueByte/Jiro.Shared/discussions>
- **Documentation**: <https://huebyte.github.io/Jiro.Shared/>

## Related Projects

- **Jiro.Shared**: <https://github.com/HueByte/Jiro.Shared> - Shared models and infrastructure
- **JiroCloud**: <https://github.com/HueByte/JiroCloud> - Cloud service implementation
- **Jiro**: <https://github.com/HueByte/Jiro> - Main application instance
