namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Represents a word update in a streaming message.
/// </summary>
public class WordUpdate
{
	/// <summary>
	/// Gets or sets the message ID that this word belongs to.
	/// </summary>
	public string MessageId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the word or token being streamed.
	/// </summary>
	public string Word { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the position of this word in the message (0-based).
	/// </summary>
	public int Position { get; set; }

	/// <summary>
	/// Gets or sets whether this is the final word in the message.
	/// </summary>
	public bool IsComplete { get; set; }

	/// <summary>
	/// Gets or sets the timestamp when this word was generated.
	/// </summary>
	public DateTime Timestamp { get; set; }
}