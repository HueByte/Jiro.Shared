using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

using Microsoft.AspNetCore.SignalR;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for client-to-server events. Used by server to handle events from connected clients.
/// </summary>
public interface IJiroHubServer
{
	/// <summary>
	/// Receives logs stream from the client instance
	/// </summary>
	[HubMethodName(Events.ReceiveLogsStream)]
	Task ReceiveLogsStreamAsync(string requestId, IAsyncEnumerable<LogEntry> stream);

	/// <summary>
	/// Receives session messages stream from the client instance
	/// </summary>
	[HubMethodName(Events.ReceiveSessionMessagesStream)]
	Task ReceiveSessionMessagesStreamAsync(string requestId, IAsyncEnumerable<ChatMessage> stream);
}
