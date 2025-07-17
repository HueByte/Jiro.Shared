namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get configuration from the instance
/// </summary>
public class GetConfigRequest
{
	/// <summary>
	/// Gets or sets the request ID for tracking purposes
	/// </summary>
	public string RequestId { get; set; } = string.Empty;
}
