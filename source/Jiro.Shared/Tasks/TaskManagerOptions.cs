namespace Jiro.Shared.Tasks;

/// <summary>
/// Configuration options for the TaskManager service.
/// </summary>
public class TaskManagerOptions
{
	/// <summary>
	/// Frequency of health check timer in seconds. Default is 300 seconds (5 minutes).
	/// </summary>
	public int HealthCheckIntervalSeconds { get; set; } = 300;

	/// <summary>
	/// Maximum number of pending tasks allowed before queueing new requests. Default is 1000.
	/// </summary>
	public int MaxPendingTasks { get; set; } = 1000;

	/// <summary>
	/// Maximum number of pending streams allowed before queueing new requests. Default is 500.
	/// </summary>
	public int MaxPendingStreams { get; set; } = 500;

	/// <summary>
	/// Maximum number of timeout monitors allowed. This should match MaxPendingStreams. Default is 500.
	/// </summary>
	public int MaxTimeoutMonitors { get; set; } = 500;

	/// <summary>
	/// Default timeout for tasks and streams in seconds. Default is 300 seconds (5 minutes).
	/// </summary>
	public int DefaultTimeoutSeconds { get; set; } = 300;

	/// <summary>
	/// Maximum time to wait in the queue before timing out in seconds. Default is 120 seconds (2 minutes).
	/// </summary>
	public int MaxQueueTimeoutSeconds { get; set; } = 120;

	/// <summary>
	/// Validates the configuration values and ensures they are within acceptable ranges.
	/// </summary>
	public void Validate()
	{
		if (HealthCheckIntervalSeconds <= 0)
			throw new ArgumentException("HealthCheckIntervalSeconds must be greater than 0", nameof(HealthCheckIntervalSeconds));

		if (MaxPendingTasks <= 0)
			throw new ArgumentException("MaxPendingTasks must be greater than 0", nameof(MaxPendingTasks));

		if (MaxPendingStreams <= 0)
			throw new ArgumentException("MaxPendingStreams must be greater than 0", nameof(MaxPendingStreams));

		if (MaxTimeoutMonitors <= 0)
			throw new ArgumentException("MaxTimeoutMonitors must be greater than 0", nameof(MaxTimeoutMonitors));

		if (DefaultTimeoutSeconds <= 0)
			throw new ArgumentException("DefaultTimeoutSeconds must be greater than 0", nameof(DefaultTimeoutSeconds));

		if (MaxQueueTimeoutSeconds <= 0)
			throw new ArgumentException("MaxQueueTimeoutSeconds must be greater than 0", nameof(MaxQueueTimeoutSeconds));

		// Ensure reasonable upper bounds
		if (HealthCheckIntervalSeconds > 3600) // 1 hour max
			throw new ArgumentException("HealthCheckIntervalSeconds cannot exceed 3600 seconds (1 hour)", nameof(HealthCheckIntervalSeconds));

		if (DefaultTimeoutSeconds > 3600) // 1 hour max
			throw new ArgumentException("DefaultTimeoutSeconds cannot exceed 3600 seconds (1 hour)", nameof(DefaultTimeoutSeconds));

		if (MaxQueueTimeoutSeconds > 1800) // 30 minutes max
			throw new ArgumentException("MaxQueueTimeoutSeconds cannot exceed 1800 seconds (30 minutes)", nameof(MaxQueueTimeoutSeconds));
	}
}
