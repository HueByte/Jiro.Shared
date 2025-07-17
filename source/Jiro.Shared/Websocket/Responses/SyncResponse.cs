namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Base class for synchronous WebSocket responses.
/// </summary>
public class SyncResponse
{
	/// <summary>
	/// Gets or sets the request identifier for correlation.
	/// </summary>
	public string RequestId { get; set; } = string.Empty;
}
