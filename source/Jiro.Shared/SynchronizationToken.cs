namespace Jiro.Shared;

/// <summary>
/// Represents a synchronization token containing instance and session identifiers
/// for tracking command execution across distributed systems.
/// </summary>
public class SynchronizationToken
{
    /// <summary>
    /// Gets or sets the unique identifier of the source instance.
    /// </summary>
    public string InstanceId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the session identifier for context.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the unique request identifier for tracking.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;
    
    /// <summary>
    /// Initializes a new instance of the SynchronizationToken class.
    /// </summary>
    public SynchronizationToken()
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the SynchronizationToken class with specified values.
    /// </summary>
    /// <param name="instanceId">The instance identifier.</param>
    /// <param name="sessionId">The session identifier.</param>
    /// <param name="requestId">The request identifier.</param>
    public SynchronizationToken(string instanceId, string sessionId, string requestId)
    {
        InstanceId = instanceId;
        SessionId = sessionId;
        RequestId = requestId;
    }
    
    /// <summary>
    /// Returns a string representation of the synchronization token.
    /// </summary>
    /// <returns>A string containing the instance, session, and request IDs.</returns>
    public override string ToString()
    {
        return $"Instance: {InstanceId}, Session: {SessionId}, Request: {RequestId}";
    }
}