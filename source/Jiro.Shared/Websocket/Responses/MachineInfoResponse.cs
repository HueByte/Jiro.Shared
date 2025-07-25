namespace Jiro.Shared.Websocket.Responses;

public class MachineInfoResponse : TrackedObject
{
	/// <summary>
	/// Gets or sets the unique identifier for the machine.
	/// </summary>
	public string MachineId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the name of the machine.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the description of the machine.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the status of the machine.
	/// </summary>
	public string Status { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the metadata associated with the machine.
	/// This can include additional information such as hardware specifications, software versions, etc.
	/// </summary>
	public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}
