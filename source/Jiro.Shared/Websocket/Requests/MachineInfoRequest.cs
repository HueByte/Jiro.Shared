namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get machine information from the instance
/// </summary>
public class MachineInfoRequest : TrackedObject
{
	/// <summary>
	/// Gets or sets the filters to apply when retrieving machine information.
	/// </summary>
	public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
}
