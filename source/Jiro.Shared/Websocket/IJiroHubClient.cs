
using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

using Microsoft.AspNetCore.SignalR;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for server-to-client events. Used by the server to push events to connected clients.
/// </summary>
public interface IJiroHubClient
{
	/// <summary>
	/// Requests logs from the client instance
	/// </summary>
	[HubMethodName(Events.LogsRequested)]
	Task<LogsResponse> RequestLogsAsync(GetLogsRequest request);

	/// <summary>
	/// Requests logs stream from the client instance
	/// </summary>
	[HubMethodName(Events.LogsStreamRequested)]
	Task<ActionResult> RequestLogsStreamAsync(GetLogsRequest request);

	/// <summary>
	/// Requests a specific session from the client instance
	/// </summary>
	[HubMethodName(Events.SingleSessionRequested)]
	Task<SessionResponse> RequestSingleSessionAsync(GetSingleSessionRequest request);

	/// <summary>
	/// Requests a stream of messages for a specific session from the client instance
	/// </summary>
	[HubMethodName(Events.SessionMessagesStreamRequested)]
	Task<ActionResult> RequestSessionMessagesStreamAsync(GetSingleSessionRequest request);

	/// <summary>
	/// Requests all sessions from the client instance
	/// </summary>
	[HubMethodName(Events.SessionsRequested)]
	Task<SessionsResponse> RequestSessionsAsync(GetSessionsRequest request);

	/// <summary>
	/// Requests configuration from the client instance
	/// </summary>
	[HubMethodName(Events.ConfigRequested)]
	Task<ConfigResponse> RequestConfigAsync(GetConfigRequest request);

	/// <summary>
	/// Sends a configuration update to the client instance
	/// </summary>
	[HubMethodName(Events.ConfigUpdated)]
	Task<ConfigUpdateResponse> UpdateConfigAsync(UpdateConfigRequest request);

	/// <summary>
	/// Requests custom themes from the client instance
	/// </summary>
	[HubMethodName(Events.CustomThemesRequested)]
	Task<ThemesResponse> RequestCustomThemesAsync(GetCustomThemesRequest request);

	/// <summary>
	/// Requests commands metadata from the client instance
	/// </summary>
	[HubMethodName(Events.CommandsMetadataRequested)]
	Task<CommandsMetadataResponse> RequestCommandsMetadataAsync(GetCommandsMetadataRequest request);

	/// <summary>
	/// Sends a command to the client instance
	/// </summary>
	[HubMethodName(Events.CommandReceived)]
	Task<ActionResult> SendCommandAsync(CommandMessage commandMessage);

	/// <summary>
	/// Removes a session from the client instance
	/// </summary>
	[HubMethodName(Events.RemoveSession)]
	Task<ActionResult> RemoveSessionAsync(string sessionId);

	/// <summary>
	/// Updates a session with the provided session update message
	/// </summary>
	[HubMethodName(Events.UpdateSession)]
	Task<ActionResult> UpdateSessionAsync(UpdateSessionRequest sessionUpdateMessage);
}

