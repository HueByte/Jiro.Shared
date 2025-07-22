using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for the Jiro client implementation. Defines events received from the server and methods to send responses.
/// </summary>
public interface IJiroClient
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

	/// <summary>
	/// Event fired when a keepalive acknowledgment is received
	/// </summary>
	public event Func<Task>? KeepaliveAckReceived;

	#endregion

	#region Server Request Events

	/// <summary>
	/// Event fired when a logs request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task<LogsResponse>>? LogsRequested;

	/// <summary>
	/// Event fired when a logs stream request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, IAsyncEnumerable<LogEntry>>? LogsStreamRequested;

	/// <summary>
	/// Event fired when a session request is received from the server
	/// </summary>
	public event Func<GetSingleSessionRequest, Task<SessionResponse>>? SessionRequested;

	/// <summary>
	/// Event fired when a session messages stream request is received from the server
	/// </summary>
	public event Func<GetSingleSessionRequest, IAsyncEnumerable<ChatMessage>>? SessionMessagesStreamRequested;

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
	/// Sets up the events for the client connection
	/// </summary>
	void SetupEvents();

	#endregion
}
