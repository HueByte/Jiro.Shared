namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get commands metadata from the instance
/// </summary>
public class GetCommandsMetadataRequest
{
	/// <summary>
	/// Gets or sets the request ID for tracking purposes
	/// </summary>
	public string RequestId { get; set; } = string.Empty;
}
