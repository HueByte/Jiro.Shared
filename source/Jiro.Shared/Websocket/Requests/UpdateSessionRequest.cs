namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to update a session.
/// </summary>
public class UpdateSessionRequest : TrackedObject
{
	/// <summary>
	/// Gets or sets the unique identifier for the session to be updated.
	/// </summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the updated name for the session.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Gets or sets the updated description for the session.
	/// </summary>
	public string? Description { get; set; }
}
