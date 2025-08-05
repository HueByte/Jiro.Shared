namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Request to start streaming chat messages.
/// </summary>
public class StreamChatRequest
{
	/// <summary>
	/// Gets or sets the unique identifier for this stream request.
	/// </summary>
	public string RequestId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the session ID for the chat stream.
	/// </summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the buffer size for the stream.
	/// </summary>
	public int BufferSize { get; set; } = 100;
}
