namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get custom themes from the instance
/// </summary>
public class GetCustomThemesRequest
{
    /// <summary>
    /// Gets or sets the request ID for tracking purposes
    /// </summary>
    public string RequestId { get; set; } = string.Empty;
}
