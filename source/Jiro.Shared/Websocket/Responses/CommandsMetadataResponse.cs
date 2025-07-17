using Jiro.Shared.Websocket.Requests;

namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Represents the response for commands metadata.
/// </summary>
public class CommandsMetadataResponse : TrackedObject
{
	/// <summary>
	/// Gets or sets the list of command metadata.
	/// </summary>
	public List<CommandMetadata> Commands { get; set; } = new();
}
