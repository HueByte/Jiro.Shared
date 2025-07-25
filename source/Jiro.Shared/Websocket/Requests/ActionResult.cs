namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents the result of an action performed by a client, providing acknowledgment
/// of whether the operation succeeded or failed along with relevant details.
/// </summary>
public class ActionResult : TrackedObject
{
	/// <summary>
	/// Gets or sets a value indicating whether the action was successful.
	/// </summary>
	public bool IsSuccess { get; set; }

	/// <summary>
	/// Gets or sets a message describing the result of the action.
	/// This can contain success confirmation or error details.
	/// </summary>
	public string Message { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets an array of error messages if the action failed.
	/// This property is empty when the action succeeds.
	/// </summary>
	public string[] Errors { get; set; } = Array.Empty<string>();
}
