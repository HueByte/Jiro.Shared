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
	/// Gets or sets the type of command result.
	/// </summary>
	public CommandType CommandType { get; set; }

	/// <summary>
	/// Gets or sets the polymorphic command result data.
	/// </summary>
	public CommandResult? Result { get; set; }

	/// <summary>
	/// Gets or sets the text type when Result is a TextResult.
	/// This is a convenience property for backward compatibility.
	/// </summary>
	public TextType? TextType
	{
		get => (Result as TextResult)?.TextType;
		set
		{
			if (Result is TextResult textResult && value.HasValue)
			{
				textResult.TextType = value.Value;
			}
		}
	}
}
