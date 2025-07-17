namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get all sessions from the instance
/// </summary>
public class GetSessionsRequest
{
	/// <summary>
	/// Gets or sets the request ID for tracking purposes
	/// </summary>
	public string RequestId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the instance ID to retrieve sessions for
	/// </summary>
	public string InstanceId { get; set; } = string.Empty;
}
