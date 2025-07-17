# Release Notes v1.0.0 "Hajimari"

**Release Date**: July 17, 2025  
**Package Version**: 1.0.0  
**Code Name**: Hajimari (始まり - Beginning)

## Summary

Initial release of Jiro.Shared - the foundational shared types and models library for the Jiro ecosystem. This package provides core communication contracts and data structures used across JiroCloud and Jiro instances.

## What's New

### WebSocket Communication

- Predefined endpoint constants for consistent communication
- Support for incoming, outgoing, and lifecycle events

### Request/Response Models

- Chat session management models
- Command system models
- Configuration management models
- Logging and theme models
- Error handling and keepalive responses

### gRPC Integration

- Protocol buffer definitions for efficient communication
- Strongly-typed gRPC services
- Compile-time validation for service contracts

### Package Features

- Full XML documentation for IntelliSense
- .NET 9.0 support with C# 13 features
- Professional NuGet package configuration

## Installation

```bash
dotnet add package Jiro.Shared --version 1.0.0
```

## Dependencies

- Grpc.AspNetCore v2.66.0
- Google.Protobuf v3.28.2

## Documentation

Full documentation available at: <https://huebyte.github.io/Jiro.Shared/>

## Next Steps

This release establishes the foundation for the Jiro ecosystem. Future releases will expand the model collection and add performance optimizations based on usage feedback.
