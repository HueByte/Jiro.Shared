using Jiro.Shared.Websocket.Requests;

namespace Jiro.Shared.Websocket.Responses;

/// <summary>
/// Represents the response for configuration.
/// </summary>
public class ConfigResponse : TrackedObject
{
	/// <summary>
	/// Gets or sets the application name.
	/// </summary>
	public string ApplicationName { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the version.
	/// </summary>
	public string? Version { get; set; }

	/// <summary>
	/// Gets or sets the environment.
	/// </summary>
	public string Environment { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the instance ID.
	/// </summary>
	public string InstanceId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the configuration section.
	/// </summary>
	public ConfigurationSection Configuration { get; set; } = new();

	/// <summary>
	/// Gets or sets the system information.
	/// </summary>
	public SystemInfo SystemInfo { get; set; } = new();

	/// <summary>
	/// Gets or sets the uptime in seconds.
	/// </summary>
	public double UptimeSeconds { get; set; }
}
