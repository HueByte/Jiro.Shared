# Jiro.Shared

[![NuGet](https://img.shields.io/nuget/v/Jiro.Shared.svg)](https://www.nuget.org/packages/Jiro.Shared/)
[![Downloads](https://img.shields.io/nuget/dt/Jiro.Shared.svg)](https://www.nuget.org/packages/Jiro.Shared/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Shared types and models for the Jiro ecosystem applications.

## Overview

Jiro.Shared provides common types, models, and utilities used across the Jiro ecosystem, including JiroCloud and Jiro instances. This package contains shared WebSocket endpoints, gRPC service definitions, request/response models, and other foundational components that ensure consistent communication between different parts of the Jiro system.

## Features

- **WebSocket Endpoints**: Predefined endpoint constants for real-time communication
- **gRPC Definitions**: Protocol buffer definitions for efficient service communication
- **Shared Models**: Common request and response data transfer objects (DTOs)
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
<PackageReference Include="Jiro.Shared" Version="1.0.0" />
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

## Documentation

For comprehensive documentation, examples, and API reference, visit:
[https://huebyte.github.io/Jiro.Shared/](https://huebyte.github.io/Jiro.Shared/)

## Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Issues**: [GitHub Issues](https://github.com/HueByte/Jiro.Shared/issues)
- **Discussions**: [GitHub Discussions](https://github.com/HueByte/Jiro.Shared/discussions)
- **Documentation**: [Official Docs](https://huebyte.github.io/Jiro.Shared/)

## Related Projects

- [Jiro.Shared](https://github.com/HueByte/Jiro.Shared) - Shared models and infrastructure
- [JiroCloud](https://github.com/HueByte/JiroCloud) - Cloud service implementation
- [Jiro](https://github.com/HueByte/Jiro) - Main application instance
