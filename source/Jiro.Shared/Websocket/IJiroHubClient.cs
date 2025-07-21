using Jiro.Shared.Websocket.Responses;

using Microsoft.AspNetCore.SignalR;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Interface for server hub methods that can be called to send commands to clients
/// </summary>
public interface IJiroHubClient
{
	#region Server Methods (Client-to-Server)

	/// <summary>
	/// Receives logs response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.GetLogs)]
	Task LogsResponse(LogsResponse response);

	/// <summary>
	/// Receives session response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.GetSession)]
	Task SessionResponse(SessionResponse response);

	/// <summary>
	/// Receives sessions response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.GetSessions)]
	Task SessionsResponse(SessionsResponse response);

	/// <summary>
	/// Receives config response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.GetConfig)]
	Task ConfigResponse(ConfigResponse response);

	/// <summary>
	/// Receives config update response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.UpdateConfig)]
	Task ConfigUpdateResponse(ConfigUpdateResponse response);

	/// <summary>
	/// Receives themes response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.GetCustomThemes)]
	Task ThemesResponse(ThemesResponse response);

	/// <summary>
	/// Receives commands metadata response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.GetCommandsMetadata)]
	Task CommandsMetadataResponse(CommandsMetadataResponse response);

	/// <summary>
	/// Receives keepalive response from the instance
	/// </summary>
	[HubMethodName(Endpoints.ClientHandled.KeepaliveAck)]
	Task KeepaliveResponse(KeepaliveResponse response);

	#endregion

	/// <summary>
	/// Sets up the events for the client hub
	/// </summary>
	void SetupEvents();
}

