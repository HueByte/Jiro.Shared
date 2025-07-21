namespace Jiro.Shared.Websocket;

/// <summary>
/// Defines constant event names used for WebSocket communication between Jiro clients and servers.
/// These events represent client-to-server messages where the client is responding to or acknowledging server requests.
/// </summary>
public class Events
{
	/// <summary>
	/// Event fired when a client sends a command execution result back to the server.
	/// </summary>
	public const string CommandReceived = "CommandReceived";
	
	/// <summary>
	/// Event fired when a client acknowledges a keepalive ping from the server.
	/// </summary>
	public const string KeepaliveAckReceived = "KeepaliveAckReceived";
	
	/// <summary>
	/// Event fired when a client responds to a server's request for logs.
	/// </summary>
	public const string LogsRequested = "LogsRequested";
	
	/// <summary>
	/// Event fired when a client responds with session details for a specific session.
	/// </summary>
	public const string SessionRequested = "SessionRequested";
	
	/// <summary>
	/// Event fired when a client responds with a list of all available sessions.
	/// </summary>
	public const string SessionsRequested = "SessionsRequested";
	
	/// <summary>
	/// Event fired when a client responds with configuration details.
	/// </summary>
	public const string ConfigRequested = "ConfigRequested";
	
	/// <summary>
	/// Event fired when a client confirms a configuration update.
	/// </summary>
	public const string ConfigUpdated = "ConfigUpdated";
	
	/// <summary>
	/// Event fired when a client responds with custom theme information.
	/// </summary>
	public const string CustomThemesRequested = "CustomThemesRequested";
	
	/// <summary>
	/// Event fired when a client responds with command metadata information.
	/// </summary>
	public const string CommandsMetadataRequested = "CommandsMetadataRequested";
	
	/// <summary>
	/// Event fired when a client successfully reconnects to the server.
	/// </summary>
	public const string Reconnected = "Reconnected";

}
