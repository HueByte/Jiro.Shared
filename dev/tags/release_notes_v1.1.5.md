# Jiro.Shared v1.1.5 "Kiban" - Foundation Strengthening

## What's Changed

### ✨ Features Added

- **`JiroClientBase` Abstract Class**: New base class that automatically wires up SignalR hub events to IJiroClient interface events
- **Automatic Event Wiring**: Eliminates the need for manual SignalR event registration in client implementations
- **Built-in Logging Support**: Comprehensive logging for all WebSocket events with before/after execution tracking
- **Constructor Flexibility**: Optional logger parameter allows for custom logging implementations

### 🛠️ Technical Details

- Non-breaking change - existing IJiroClient implementations continue to work unchanged
- Structured logging format: `[INF] {EventName} received` and `[INF] {EventName} executed`
- Throws `NotImplementedException` for unhandled events, ensuring proper implementation
- Minimal performance overhead with optional logging (null-checked)

### 📦 Dependencies

- Microsoft.AspNetCore.SignalR.Client v9.0.7
- Microsoft.Extensions.Logging (via abstractions)

### 🚀 Getting Started

```csharp
// Simple client implementation with automatic event wiring
public class MyJiroClient : JiroClientBase
{
    public MyJiroClient(HubConnection connection, ILogger<MyJiroClient> logger) 
        : base(connection, logger)
    {
        // Events are automatically wired up!
        CommandReceived += async (command) => 
        {
            // Handle command
        };
    }
}
```

**Full Changelog**: https://github.com/HueByte/Jiro.Shared/compare/v1.1.4...v1.1.5