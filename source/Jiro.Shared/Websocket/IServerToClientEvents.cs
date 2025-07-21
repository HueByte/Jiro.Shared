using Jiro.Shared.Websocket.Requests;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for server-to-client events. Used by the server to push events to connected clients.
/// </summary>
public interface IServerToClientEvents
{
	#region Server-to-Client Push Methods

	/// <summary>
	/// Requests logs from the client instance
	/// </summary>
	Task PushGetLogsAsync(GetLogsRequest request);

	/// <summary>
	/// Requests a specific session from the client instance
	/// </summary>
	Task PushGetSessionAsync(GetSessionRequest request);

	/// <summary>
	/// Requests all sessions from the client instance
	/// </summary>
	Task PushGetSessionsAsync(GetSessionsRequest request);

	/// <summary>
	/// Requests configuration from the client instance
	/// </summary>
	Task PushGetConfigAsync(GetConfigRequest request);

	/// <summary>
	/// Sends a configuration update to the client instance
	/// </summary>
	Task PushUpdateConfigAsync(UpdateConfigRequest request);

	/// <summary>
	/// Requests custom themes from the client instance
	/// </summary>
	Task PushGetCustomThemesAsync(GetCustomThemesRequest request);

	/// <summary>
	/// Requests commands metadata from the client instance
	/// </summary>
	Task PushGetCommandsMetadataAsync(GetCommandsMetadataRequest request);

	/// <summary>
	/// Sends a command to the client instance
	/// </summary>
	Task PushSendCommandAsync(CommandMessage commandMessage);

	/// <summary>
	/// Sends a keepalive acknowledgment to the client
	/// </summary>
	Task PushSendKeepaliveAckAsync();

	#endregion
}

