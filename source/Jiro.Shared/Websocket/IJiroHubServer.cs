using System.Threading.Channels;

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
	/// <param name="requestId">The unique identifier for this stream request</param>
	/// <param name="stream">The channel reader containing log entries from the client</param>
	/// <returns>An ActionResult indicating the success or failure of the operation</returns>
	[HubMethodName(Events.ReceiveLogsStream)]
	Task<ActionResult> ReceiveLogsStreamAsync(string requestId, ChannelReader<LogEntry> stream);

	/// <summary>
	/// Receives session messages stream from the client instance
	/// </summary>
	/// <param name="requestId">The unique identifier for this stream request</param>
	/// <param name="stream">The channel reader containing chat messages from the client</param>
	/// <returns>An ActionResult indicating the success or failure of the operation</returns>
	[HubMethodName(Events.ReceiveSessionMessagesStream)]
	Task<ActionResult> ReceiveSessionMessagesStreamAsync(string requestId, ChannelReader<ChatMessage> stream);
}
