# Release Notes: v1.2.0 "Nagare"

**Release Date**: July 22, 2025  
**Code Name**: "Nagare" (流れ - Flow/Stream)  
**Type**: Minor Release  

## Key Features

### 🌊 Streaming Revolution
- **Real-Time Data Streaming**: Comprehensive streaming infrastructure with `StreamingExtensions` and `SignalRStreamExtensions`
- **Enhanced Communication**: New WebSocket events and improved gRPC services for seamless data flow
- **Memory Optimization**: Advanced `MemoryExtensions` for efficient handling of large data streams

### ⚡ Performance & Monitoring  
- **Performance Analysis**: New `PerformanceAnalyzer` utility for monitoring and optimization
- **Paginated Requests**: `GetLogsRequest` now supports offset parameter for efficient data retrieval
- **Session Broadcasting**: New `SendSessionMessages` gRPC call for enhanced session management

### 🔧 Infrastructure Improvements
- **Code Quality**: Global suppressions configuration for consistent analysis
- **Protocol Refinement**: Cleaned and optimized `jiroHub.proto` definitions
- **Memory Management**: Advanced utilities for handling streaming scenarios

## Breaking Changes

- **Model Rename**: `GetSessionRequest` → `GetSingleSessionRequest` (functionality unchanged)

## Technical Highlights

- Full .NET 9.0 compatibility
- Optimized streaming performance with reduced memory footprint
- Enhanced WebSocket event processing for real-time responsiveness
- Production-ready streaming capabilities with comprehensive testing

---

This release establishes Jiro.Shared as a powerful foundation for real-time, data-intensive applications with advanced streaming capabilities and performance monitoring tools.