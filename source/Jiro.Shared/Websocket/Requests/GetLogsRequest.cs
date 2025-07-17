namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to get logs from the instance
/// </summary>
public class GetLogsRequest
{
    /// <summary>
    /// Gets or sets the request ID for tracking purposes
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the log level filter (optional)
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of logs to retrieve (optional, defaults to 100)
    /// </summary>
    public int? Limit { get; set; }
}
