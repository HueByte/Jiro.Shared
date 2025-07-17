namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get a specific session from the instance
/// </summary>
public class GetSessionRequest
{
	/// <summary>
	/// Gets or sets the request ID for tracking purposes
	/// </summary>
	public string RequestId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the instance ID
	/// </summary>
	public string InstanceId { get; set; } = string.Empty;
}
