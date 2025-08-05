namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Represents a chat message in the session response.
/// </summary>
public class ChatMessage
{
	/// <summary>
	/// Gets or sets the message ID.
	/// </summary>
	public string MessageId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether the message is from a user.
	/// </summary>
	public bool IsUser { get; set; }

	/// <summary>
	/// Gets or sets the message type.
	/// </summary>
	public string Type { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the message content.
	/// </summary>
	public string Content { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the timestamp of the message creation.
	/// </summary>
	public DateTime CreatedAt { get; set; }
}
