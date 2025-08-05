
using System.Threading.Channels;

using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for the Jiro client implementation. Defines events received from the server and methods to send responses.
/// </summary>
public interface IJiroInstance
{
	#region Connection Lifecycle Events

	/// <summary>
	/// Event fired when connection is closed
	/// </summary>
	public event Func<Exception?, Task>? Closed;

	/// <summary>
	/// Event fired when connection is reconnecting
	/// </summary>
	public event Func<Exception?, Task>? Reconnecting;

	/// <summary>
	/// Event fired when connection is reconnected
	/// </summary>
	public event Func<string?, Task>? Reconnected;

	#endregion

	#region Command Events

	/// <summary>
	/// Event fired when a command is received from the server
	/// </summary>
	public event Func<CommandMessage, Task>? CommandReceived;

	#endregion

	#region Server Request Events

	/// <summary>
	/// Event fired when a logs request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task<LogsResponse>>? LogsRequested;

	/// <summary>
	/// Event fired when a logs stream request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task<ActionResult>>? LogsStreamRequested;

	/// <summary>
	/// Event fired when a session request is received from the server
	/// </summary>
	public event Func<GetSingleSessionRequest, Task<SessionResponse>>? SessionRequested;

	/// <summary>
	/// Event fired when a session messages stream request is received from the server
	/// </summary>
	public event Func<GetSingleSessionRequest, Task<ActionResult>>? SessionMessagesStreamRequested;

	/// <summary>
	/// Event fired when a sessions request is received from the server
	/// </summary>
	public event Func<GetSessionsRequest, Task<SessionsResponse>>? SessionsRequested;

	/// <summary>
	/// Event fired when a config request is received from the server
	/// </summary>
	public event Func<GetConfigRequest, Task<ConfigResponse>>? ConfigRequested;

	/// <summary>
	/// Event fired when a config update request is received from the server
	/// </summary>
	public event Func<UpdateConfigRequest, Task<ConfigResponse>>? ConfigUpdateRequested;

	/// <summary>
	/// Event fired when a custom themes request is received from the server
	/// </summary>
	public event Func<GetCustomThemesRequest, Task<ThemesResponse>>? CustomThemesRequested;

	/// <summary>
	/// Event fired when a commands metadata request is received from the server
	/// </summary>
	public event Func<GetCommandsMetadataRequest, Task<CommandsMetadataResponse>>? CommandsMetadataRequested;

	/// <summary>
	/// Event fired when a remove session request is received from the server
	/// </summary>
	public event Func<RemoveSessionRequest, Task<ActionResult>>? RemoveSessionRequested;

	/// <summary>
	/// Event fired when an update session request is received from the server
	/// </summary>
	public event Func<UpdateSessionRequest, Task<ActionResult>>? UpdateSessionRequested;

	/// <summary>
	/// Event fired when a machine info request is received from the server
	/// </summary>
	public event Func<MachineInfoRequest, Task<MachineInfoResponse>>? MachineInfoRequested;

	/// <summary>
	/// Sets up the events for the client connection
	/// </summary>
	void SetupEvents();

	/// <summary>
	/// Sends logs stream to the server
	/// </summary>
	/// <param name="requestId">The unique identifier for this stream request</param>
	/// <param name="stream">The channel reader containing log entries to send</param>
	Task ReceiveLogsStreamAsync(string requestId, ChannelReader<LogEntry> stream);

	/// <summary>
	/// Sends session messages stream to the server
	/// </summary>
	/// <param name="requestId">The unique identifier for this stream request</param>
	/// <param name="stream">The channel reader containing chat messages to send</param>
	Task ReceiveSessionMessagesStreamAsync(string requestId, ChannelReader<ChatMessage> stream);

	#endregion
}
