namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get a specific session from the instance
/// </summary>
public class GetSessionRequest : TrackedObject
{
	/// <summary>
	/// Gets or sets the instance ID
	/// </summary>
	public string InstanceId { get; set; } = string.Empty;
}
