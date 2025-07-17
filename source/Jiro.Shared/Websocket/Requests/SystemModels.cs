namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents configuration section information.
/// </summary>
public class ConfigurationSection
{
	/// <summary>
	/// Gets or sets the configuration values.
	/// </summary>
	public Dictionary<string, object> Values { get; set; } = new();
}

/// <summary>
/// Represents system information.
/// </summary>
public class SystemInfo
{
	/// <summary>
	/// Gets or sets the operating system.
	/// </summary>
	public string OperatingSystem { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the .NET runtime version.
	/// </summary>
	public string RuntimeVersion { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the machine name.
	/// </summary>
	public string MachineName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the processor count.
	/// </summary>
	public int ProcessorCount { get; set; }

	/// <summary>
	/// Gets or sets the total memory in bytes.
	/// </summary>
	public long TotalMemory { get; set; }
}
