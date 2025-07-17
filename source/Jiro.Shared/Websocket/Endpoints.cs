namespace Jiro.Shared.Websocket;

/// <summary>
/// Contains constants for WebSocket endpoint names used in Jiro hub communication
/// </summary>
public static class Endpoints
{
	/// <summary>
	/// Server to client events
	/// </summary>
	public static class Incoming
	{
		public const string ReceiveCommand = "ReceiveCommand";
		public const string KeepaliveAck = "KeepaliveAck";
		public const string GetLogs = "GetLogs";
		public const string GetSession = "GetSession";
		public const string GetSessions = "GetSessions";
		public const string GetConfig = "GetConfig";
		public const string UpdateConfig = "UpdateConfig";
		public const string GetCustomThemes = "GetCustomThemes";
		public const string GetCommandsMetadata = "GetCommandsMetadata";

		public static string[] AllIncomingEvents => new[]
		{
			ReceiveCommand,
			KeepaliveAck,
			GetLogs,
			GetSession,
			GetSessions,
			GetConfig,
			UpdateConfig,
			GetCustomThemes,
			GetCommandsMetadata
		};
	}

	/// <summary>
	/// Client to server events
	/// </summary>
	public static class Outgoing
	{
		public const string KeepaliveResponse = "KeepaliveResponse";
		public const string LogsResponse = "LogsResponse";
		public const string ErrorResponse = "ErrorResponse";
		public const string SessionResponse = "SessionResponse";
		public const string SessionsResponse = "SessionsResponse";
		public const string ConfigResponse = "ConfigResponse";
		public const string ConfigUpdateResponse = "ConfigUpdateResponse";
		public const string ThemesResponse = "ThemesResponse";
		public const string CommandsMetadataResponse = "CommandsMetadataResponse";

		public static string[] AllOutgoingEvents => new[]
		{
			KeepaliveResponse,
			LogsResponse,
			ErrorResponse,
			SessionResponse,
			SessionsResponse,
			ConfigResponse,
			ConfigUpdateResponse,
			ThemesResponse,
			CommandsMetadataResponse
		};
	}


	/// <summary>
	/// Connection lifecycle events
	/// </summary>
	public static class Lifecycle
	{
		public const string Closed = "Closed";
		public const string Reconnecting = "Reconnecting";
		public const string Reconnected = "Reconnected";

		public static string[] AllLifecycleEvents => new[]
		{
			Closed,
			Reconnecting,
			Reconnected
		};
	}
}
