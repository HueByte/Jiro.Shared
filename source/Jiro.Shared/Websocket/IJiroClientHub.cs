using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for client events that can be received from the server
/// </summary>
public interface IJiroClientHub
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

	#region Request Events

	/// <summary>
	/// Event fired when a logs request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task>? LogsRequested;

	/// <summary>
	/// Event fired when a session request is received from the server
	/// </summary>
	public event Func<GetSessionRequest, Task>? SessionRequested;

	/// <summary>
	/// Event fired when a sessions request is received from the server
	/// </summary>
	public event Func<GetSessionsRequest, Task>? SessionsRequested;

	/// <summary>
	/// Event fired when a config request is received from the server
	/// </summary>
	public event Func<GetConfigRequest, Task>? ConfigRequested;

	/// <summary>
	/// Event fired when a config update request is received from the server
	/// </summary>
	public event Func<UpdateConfigRequest, Task>? ConfigUpdateRequested;

	/// <summary>
	/// Event fired when a custom themes request is received from the server
	/// </summary>
	public event Func<GetCustomThemesRequest, Task>? CustomThemesRequested;

	/// <summary>
	/// Event fired when a commands metadata request is received from the server
	/// </summary>
	public event Func<GetCommandsMetadataRequest, Task>? CommandsMetadataRequested;

	#endregion

	#region Response Methods

	/// <summary>
	/// Sends a logs response to the server
	/// </summary>
	/// <param name="response">The logs response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendLogsResponseAsync(LogsResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a session response to the server
	/// </summary>
	/// <param name="response">The session response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendSessionResponseAsync(SessionResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a sessions response to the server
	/// </summary>
	/// <param name="response">The sessions response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendSessionsResponseAsync(SessionsResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a config response to the server
	/// </summary>
	/// <param name="response">The config response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendConfigResponseAsync(ConfigResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a config update response to the server
	/// </summary>
	/// <param name="response">The config update response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendConfigUpdateResponseAsync(ConfigUpdateResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a themes response to the server
	/// </summary>
	/// <param name="response">The themes response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendThemesResponseAsync(ThemesResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a commands metadata response to the server
	/// </summary>
	/// <param name="response">The commands metadata response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendCommandsMetadataResponseAsync(CommandsMetadataResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a keepalive response to the server
	/// </summary>
	/// <param name="response">The keepalive response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendKeepaliveResponseAsync(KeepaliveResponse response, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends an error response to the server
	/// </summary>
	/// <param name="response">The error response</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendErrorResponseAsync(ErrorResponse response, CancellationToken cancellationToken = default);

	#endregion

	/// <summary>
	/// Sets up the events for the client hub
	/// </summary>
	void SetupEvents();
}
