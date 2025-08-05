namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Request to start streaming word-by-word message content from client to server.
/// </summary>
public class StreamWordRequest
{
	/// <summary>
	/// Gets or sets the unique identifier for this stream request.
	/// </summary>
	public string RequestId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the session ID.
	/// </summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the message ID to stream.
	/// </summary>
	public string MessageId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the buffer size for the stream.
	/// </summary>
	public int BufferSize { get; set; } = 100;
}