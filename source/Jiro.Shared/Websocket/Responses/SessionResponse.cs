namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Represents the response for a single session with messages.
/// </summary>
public class SessionResponse : SyncResponse
{
    /// <summary>
    /// Gets or sets the instance ID.
    /// </summary>
    public string InstanceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session ID.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the session name.
    /// </summary>
    public string SessionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp of the session creation.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last update.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the total number of messages in the session.
    /// </summary>
    public int TotalMessages { get; set; }

    /// <summary>
    /// Gets or sets the list of messages in the session.
    /// </summary>
    public List<ChatMessage> Messages { get; set; } = new();
}

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
