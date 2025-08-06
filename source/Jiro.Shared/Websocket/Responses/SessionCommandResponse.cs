namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Represents a command execution response with tracking information.
/// </summary>
public class SessionCommandResponse : TrackedObject
{
	/// <summary>
	/// Gets or sets the synchronization token containing instance and session information.
	/// </summary>
	public SynchronizationToken SynchronizationToken { get; set; } = new();

	/// <summary>
	/// Gets or sets the name of the executed command.
	/// </summary>
	public string CommandName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether the command executed successfully.
	/// </summary>
	public bool IsSuccess { get; set; }

	/// <summary>
	/// Gets or sets the type of command result as a string.
	/// Applications should use their own CommandType enum for type safety.
	/// </summary>
	public string CommandType { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the command result data.
	/// Applications should cast this to their specific ICommandResult implementation.
	/// </summary>
	public object? Result { get; set; }
}
