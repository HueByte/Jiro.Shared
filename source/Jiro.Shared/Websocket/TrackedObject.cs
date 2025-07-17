namespace Jiro.Shared.Websocket;

/// <summary>
/// Base class for objects that require request ID tracking
/// </summary>
public abstract class TrackedObject
{
	/// <summary>
	/// Gets or sets the request ID for tracking purposes
	/// </summary>
	public string RequestId { get; set; } = string.Empty;
}
