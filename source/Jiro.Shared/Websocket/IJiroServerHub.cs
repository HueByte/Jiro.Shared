using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

using Microsoft.AspNetCore.SignalR;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for server hub methods that can be called to send commands to clients
/// </summary>
public interface IJiroServerHub
{
	#region Response Handlers

	/// <summary>
	/// Receives logs response from the instance
	/// </summary>
	[HubMethodName("LogsResponse")]
	Task ReceiveLogsResponse(LogsResponse response);

	/// <summary>
	/// Receives session response from the instance
	/// </summary>
	[HubMethodName("SessionResponse")]
	Task ReceiveSessionResponse(SessionResponse response);

	/// <summary>
	/// Receives sessions response from the instance
	/// </summary>
	[HubMethodName("SessionsResponse")]
	Task ReceiveSessionsResponse(SessionsResponse response);

	/// <summary>
	/// Receives config response from the instance
	/// </summary>
	[HubMethodName("ConfigResponse")]
	Task ReceiveGetConfigResponse(ConfigResponse response);

	/// <summary>
	/// Receives config update response from the instance
	/// </summary>
	[HubMethodName("ConfigUpdateResponse")]
	Task ReceiveConfigUpdateResponse(ConfigUpdateResponse response);

	/// <summary>
	/// Receives themes response from the instance
	/// </summary>
	[HubMethodName("ThemesResponse")]
	Task ReceiveThemesResponse(ThemesResponse response);

	/// <summary>
	/// Receives commands metadata response from the instance
	/// </summary>
	[HubMethodName("CommandsMetadataResponse")]
	Task ReceiveCommandsMetadataResponse(CommandsMetadataResponse response);

	/// <summary>
	/// Receives keepalive response from the instance
	/// </summary>
	[HubMethodName("KeepaliveResponse")]
	Task ReceiveKeepaliveResponse(KeepaliveResponse response);

	/// <summary>
	/// Receives error response from the instance
	/// </summary>
	[HubMethodName("ErrorResponse")]
	Task ReceiveErrorResponse(ErrorResponse response);

	#endregion

	#region Command Methods

	/// <summary>
	/// Sends a command to the client instance
	/// </summary>
	/// <param name="commandMessage">The command message to send</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendCommandAsync(CommandMessage commandMessage, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a keepalive acknowledgment to the client
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task SendKeepaliveAckAsync(CancellationToken cancellationToken = default);

	#endregion

	#region Request Methods

	/// <summary>
	/// Requests logs from the client instance
	/// </summary>
	/// <param name="request">Parameters for the logs request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task GetLogsAsync(GetLogsRequest request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests a specific session from the client instance
	/// </summary>
	/// <param name="request">Parameters for the session request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task GetSessionAsync(GetSessionRequest request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests all sessions from the client instance
	/// </summary>
	/// <param name="request">Parameters for the sessions request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task GetSessionsAsync(GetSessionsRequest request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests configuration from the client instance
	/// </summary>
	/// <param name="request">Parameters for the config request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task GetConfigAsync(GetConfigRequest request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a configuration update to the client instance
	/// </summary>
	/// <param name="request">Parameters for the config update request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task UpdateConfigAsync(UpdateConfigRequest request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests custom themes from the client instance
	/// </summary>
	/// <param name="request">Parameters for the themes request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task GetCustomThemesAsync(GetCustomThemesRequest request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests commands metadata from the client instance
	/// </summary>
	/// <param name="request">Parameters for the commands metadata request</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Task representing the operation</returns>
	Task GetCommandsMetadataAsync(GetCommandsMetadataRequest request, CancellationToken cancellationToken = default);

	#endregion
}
