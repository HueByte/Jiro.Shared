<p align="center">
    <img src="dev/assets/JiroBanner.png" style="border-radius: 15px;" alt="Jiro AI Assistant Banner"/>
</p>

<p align="center">
    <a href="https://github.com/HueByte/Jiro.Shared/actions/workflows/create-release.yml">
        <img src="https://img.shields.io/github/actions/workflow/status/HueByte/Jiro.Shared/create-release.yml?branch=main&style=for-the-badge&label=build" alt="Build Status"/>
    </a>
    <a href="https://github.com/HueByte/Jiro.Shared/releases/latest">
        <img src="https://img.shields.io/github/v/release/HueByte/Jiro.Shared?style=for-the-badge&color=blue" alt="Latest Release"/>
    </a>
    <a href="https://github.com/HueByte/Jiro.Shared/commits/main">
        <img src="https://img.shields.io/github/last-commit/HueByte/Jiro.Shared?style=for-the-badge&color=orange" alt="Last Commit"/>
    </a>
    <a href="https://github.com/HueByte/Jiro.Shared/stargazers">
        <img src="https://img.shields.io/github/stars/HueByte/Jiro.Shared?style=for-the-badge&color=yellow" alt="GitHub Stars"/>
    </a>
    <a href="https://github.com/HueByte/Jiro.Shared/issues">
        <img src="https://img.shields.io/github/issues/HueByte/Jiro.Shared?style=for-the-badge&color=red" alt="GitHub Issues"/>
    </a>
    <a href="https://github.com/HueByte/Jiro.Shared/blob/main/LICENSE">
        <img src="https://img.shields.io/github/license/HueByte/Jiro.Shared?style=for-the-badge&color=green" alt="License"/>
    </a>
    <a href="https://dotnet.microsoft.com/download">
        <img src="https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge" alt=".NET 9.0"/>
    </a>
    <a href="https://github.com/HueByte/Jiro.Shared">
        <img src="https://img.shields.io/github/languages/code-size/HueByte/Jiro.Shared?style=for-the-badge&color=purple" alt="Code Size"/>
    </a>
    <a href="https://www.nuget.org/packages/Jiro.Shared/">
        <img src="https://img.shields.io/nuget/v/Jiro.Shared.svg?style=for-the-badge&color=blue" alt="NuGet"/>
    </a>
    <a href="https://www.nuget.org/packages/Jiro.Shared/">
        <img src="https://img.shields.io/nuget/dt/Jiro.Shared.svg?style=for-the-badge&color=blue" alt="Downloads"/>
    </a>
    <a href="LICENSE">
        <img src="https://img.shields.io/badge/license-MIT-blue.svg?style=for-the-badge&color=blue" alt="License"/>
    </a>
</p>

# Jiro.Shared

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
<PackageReference Include="Jiro.Shared" Version="1.1.0" />
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

## Releases

- **[v1.1.0 "Kaizen"](https://github.com/HueByte/Jiro.Shared/releases/tag/v1.1.0)** - Enhancement release with system monitoring and database utilities
- **[v1.0.1](https://github.com/HueByte/Jiro.Shared/releases/tag/v1.0.1)** - NuGet package display fixes
- **[v1.0.0 "Hajimari"](https://github.com/HueByte/Jiro.Shared/releases/tag/v1.0.0)** - Initial release

Each major release includes a Japanese-inspired code name that reflects its theme. See our [changelog](dev/docs/changelog/) for detailed release notes and [code names documentation](dev/docs/changelog/code-names.md) for the naming convention.

## Support

- **Issues**: [GitHub Issues](https://github.com/HueByte/Jiro.Shared/issues)
- **Discussions**: [GitHub Discussions](https://github.com/HueByte/Jiro.Shared/discussions)
- **Documentation**: [Official Docs](https://huebyte.github.io/Jiro.Shared/)

## Related Projects

- [Jiro.Shared](https://github.com/HueByte/Jiro.Shared) - Shared models and infrastructure
- [JiroCloud](https://github.com/HueByte/JiroCloud) - Cloud service implementation
- [Jiro](https://github.com/HueByte/Jiro) - Main application instance
