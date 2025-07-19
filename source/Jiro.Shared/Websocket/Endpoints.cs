using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Contains constants for WebSocket endpoint names used in Jiro hub communication
/// </summary>
/// <summary>
/// Provides strongly-typed constants and event type mappings for WebSocket endpoint names used in Jiro hub communication.
/// </summary>
public static class Endpoints
{
	/// <summary>
	/// Server to client events
	/// </summary>
	/// <summary>
	/// Contains constants and event type mappings for server-to-client WebSocket events handled by the client.
	/// </summary>
	public static class ClientHandled
	{
		/// <summary>
		/// Maps event names to their expected payload types for server-to-client events.
		/// </summary>
		public static Dictionary<string, Type> EventTypes { get; } = new()
		{
			// Server-to-client events: payloads are requests or command messages
			{ ReceiveCommand, typeof(CommandMessage) },
			{ KeepaliveAck, typeof(object) },
			{ GetLogs, typeof(GetLogsRequest) },
			{ GetSession, typeof(GetSessionRequest) },
			{ GetSessions, typeof(GetSessionsRequest) },
			{ GetConfig, typeof(GetConfigRequest) },
			{ UpdateConfig, typeof(UpdateConfigRequest) },
			{ GetCustomThemes, typeof(GetCustomThemesRequest) },
			{ GetCommandsMetadata, typeof(GetCommandsMetadataRequest) }
		};

		/// <summary>Event name for receiving a command from the server.</summary>
		public const string ReceiveCommand = "ReceiveCommand";
		/// <summary>Event name for acknowledging a keepalive signal from the server.</summary>
		public const string KeepaliveAck = "KeepaliveAck";
		/// <summary>Event name for requesting logs from the server.</summary>
		public const string GetLogs = "GetLogs";
		/// <summary>Event name for requesting a single session from the server.</summary>
		public const string GetSession = "GetSession";
		/// <summary>Event name for requesting all sessions from the server.</summary>
		public const string GetSessions = "GetSessions";
		/// <summary>Event name for requesting configuration from the server.</summary>
		public const string GetConfig = "GetConfig";
		/// <summary>Event name for updating configuration on the server.</summary>
		public const string UpdateConfig = "UpdateConfig";
		/// <summary>Event name for requesting custom themes from the server.</summary>
		public const string GetCustomThemes = "GetCustomThemes";
		/// <summary>Event name for requesting command metadata from the server.</summary>
		public const string GetCommandsMetadata = "GetCommandsMetadata";
	}

	/// <summary>
	/// Client to server events
	/// </summary>
	/// <summary>
	/// Contains constants and event type mappings for client-to-server WebSocket events handled by the server.
	/// </summary>
	public static class ServerHandled
	{
		/// <summary>
		/// Maps event names to their expected payload types for client-to-server events.
		/// </summary>
		public static Dictionary<string, Type> EventTypes { get; } = new()
		{
			// Client-to-server events: payloads are responses
			{ KeepaliveResponse, typeof(KeepaliveResponse) },
			{ LogsResponse, typeof(LogsResponse) },
			{ ErrorResponse, typeof(ErrorResponse) },
			{ SessionResponse, typeof(SessionResponse) },
			{ SessionsResponse, typeof(SessionsResponse) },
			{ ConfigResponse, typeof(ConfigResponse) },
			{ ConfigUpdateResponse, typeof(ConfigUpdateResponse) },
			{ ThemesResponse, typeof(ThemesResponse) },
			{ CommandsMetadataResponse, typeof(CommandsMetadataResponse) }
		};

		/// <summary>Event name for sending a keepalive response to the server.</summary>
		public const string KeepaliveResponse = "KeepaliveResponse";
		/// <summary>Event name for sending logs to the server.</summary>
		public const string LogsResponse = "LogsResponse";
		/// <summary>Event name for sending an error response to the server.</summary>
		public const string ErrorResponse = "ErrorResponse";
		/// <summary>Event name for sending a single session response to the server.</summary>
		public const string SessionResponse = "SessionResponse";
		/// <summary>Event name for sending all sessions response to the server.</summary>
		public const string SessionsResponse = "SessionsResponse";
		/// <summary>Event name for sending configuration data to the server.</summary>
		public const string ConfigResponse = "ConfigResponse";
		/// <summary>Event name for sending a configuration update response to the server.</summary>
		public const string ConfigUpdateResponse = "ConfigUpdateResponse";
		/// <summary>Event name for sending themes data to the server.</summary>
		public const string ThemesResponse = "ThemesResponse";
		/// <summary>Event name for sending command metadata to the server.</summary>
		public const string CommandsMetadataResponse = "CommandsMetadataResponse";
	}


	/// <summary>
	/// Connection lifecycle events
	/// </summary>
	/// <summary>
	/// Contains constants for WebSocket connection lifecycle events.
	/// </summary>
	public static class Lifecycle
	{
		/// <summary>Event name for when the WebSocket connection is closed.</summary>
		public const string Closed = "Closed";
		/// <summary>Event name for when the WebSocket connection is attempting to reconnect.</summary>
		public const string Reconnecting = "Reconnecting";
		/// <summary>Event name for when the WebSocket connection has successfully reconnected.</summary>
		public const string Reconnected = "Reconnected";

		/// <summary>
		/// Gets all connection lifecycle event names.
		/// </summary>
		public static string[] AllLifecycleEvents => new[]
		{
			Closed,
			Reconnecting,
			Reconnected
		};
	}
}
