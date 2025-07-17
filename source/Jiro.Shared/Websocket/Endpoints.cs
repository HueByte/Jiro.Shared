namespace Jiro.Shared.Websocket;


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
	}


	/// <summary>
	/// Connection lifecycle events
	/// </summary>
	public static class Lifecycle
	{
		public const string Closed = "Closed";
		public const string Reconnecting = "Reconnecting";
		public const string Reconnected = "Reconnected";
	}
}
