namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents the response for commands metadata.
/// </summary>
public class CommandsMetadataResponse : SyncResponse
{
	/// <summary>
	/// Gets or sets the list of command metadata.
	/// </summary>
	public List<CommandMetadata> Commands { get; set; } = new();
}
