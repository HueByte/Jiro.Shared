namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get a specific session from the instance
/// </summary>
public class GetSingleSessionRequest : TrackedObject
{
	/// <summary>
	/// Gets or sets the instance ID
	/// </summary>
	public string InstanceId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the session ID
	/// </summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether to include messages in the session response
	/// </summary>
	public bool IncludeMessages { get; set; } = false;
}
