namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to update configuration on the instance
/// </summary>
public class UpdateConfigRequest
{
    /// <summary>
    /// Gets or sets the request ID for tracking purposes
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the configuration data to update
    /// </summary>
    public object ConfigData { get; set; } = default!;
}
