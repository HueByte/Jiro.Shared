namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get all sessions from the instance
/// </summary>
public class GetSessionsRequest : TrackedObject
{
	/// <summary>
	/// Gets or sets the instance ID to retrieve sessions for
	/// </summary>
	public string InstanceId { get; set; } = string.Empty;
}
