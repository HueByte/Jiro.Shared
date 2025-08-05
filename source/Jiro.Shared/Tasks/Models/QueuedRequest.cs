namespace Jiro.Shared.Tasks.Models;

/// <summary>
/// Represents a queued task request waiting to be processed.
/// </summary>
public class QueuedRequest
{
	public string InstanceId { get; }
	public string RequestId { get; }
	public Func<Task> DispatchAsync { get; }
	public CancellationToken CancellationToken { get; }
	public TaskCompletionSource<object> TaskCompletionSource { get; }
	public DateTime QueuedAt { get; }
	public Type ResponseType { get; }

	public QueuedRequest(
		string instanceId,
		string requestId,
		Func<Task> dispatchAsync,
		CancellationToken cancellationToken,
		TaskCompletionSource<object> taskCompletionSource,
		Type responseType)
	{
		InstanceId = instanceId;
		RequestId = requestId;
		DispatchAsync = dispatchAsync;
		CancellationToken = cancellationToken;
		TaskCompletionSource = taskCompletionSource;
		QueuedAt = DateTime.UtcNow;
		ResponseType = responseType;
	}
}

/// <summary>
/// Represents a queued channel stream request waiting to be processed.
/// </summary>
public class QueuedStreamRequest
{
	public string InstanceId { get; }
	public string ConnectionId { get; }
	public string RequestId { get; }
	public Func<Task> StreamInvoker { get; }
	public CancellationToken CancellationToken { get; }
	public TaskCompletionSource<object> TaskCompletionSource { get; }
	public DateTime QueuedAt { get; }
	public Type ResultType { get; }

	public QueuedStreamRequest(
		string instanceId,
		string connectionId,
		string requestId,
		Func<Task> streamInvoker,
		CancellationToken cancellationToken,
		TaskCompletionSource<object> taskCompletionSource,
		Type resultType)
	{
		InstanceId = instanceId;
		ConnectionId = connectionId;
		RequestId = requestId;
		StreamInvoker = streamInvoker;
		CancellationToken = cancellationToken;
		TaskCompletionSource = taskCompletionSource;
		QueuedAt = DateTime.UtcNow;
		ResultType = resultType;
	}
}
