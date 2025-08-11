using System.Collections.Concurrent;
using System.Threading.Channels;

using Jiro.Shared.Tasks.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jiro.Shared.Tasks;

/// <summary>
/// Manages asynchronous tasks and streaming operations with timeout monitoring, queueing, and resource limits.
/// Provides thread-safe execution of fire-and-forget dispatches with external result completion.
/// </summary>
/// <remarks>
/// <para>The TaskManager supports two primary operation modes:</para>
/// <list type="bullet">
/// <item><description>Regular Tasks: Fire-and-forget execution with external result completion via <see cref="SetTaskResult{TResponse}"/></description></item>
/// <item><description>Stream Tasks: Channel-based streaming with real-time data flow via <see cref="SetChannelStreamResultAsync{TResult}"/></description></item>
/// </list>
/// <para>Example usage for regular tasks:</para>
/// <code>
/// // Execute a command and wait for external result
/// var result = await taskManager.ExternalExecuteAsync&lt;CommandResult&gt;(
///     instanceId: "instance-1",
///     requestId: "req-123",
///     dispatchAsync: () => hubContext.Clients.Group(instanceId).SendAsync("ExecuteCommand", command),
///     cancellationToken: cancellationToken);
///
/// // Later, when the external service receives the result:
/// taskManager.SetTaskResult("req-123", commandResult);
/// </code>
/// <para>Example usage for streaming:</para>
/// <code>
/// // Start a streaming operation
/// var reader = await taskManager.GetChannelStreamResponseAsync&lt;StreamItem&gt;(
///     instanceId: "instance-1",
///     connectionId: "conn-456",
///     requestId: "stream-789",
///     streamInvoker: () => hubContext.Clients.Client(connectionId).SendAsync("StartStream", parameters));
///
/// // Consume the stream
/// await foreach (var item in reader.ReadAllAsync())
/// {
///     ProcessStreamItem(item);
/// }
/// </code>
/// </remarks>
public class TaskManager : IInstanceTaskManager, IDisposable
{
	#region Constants

	private const int MaxProcessingBatchSize = 10;
	private const double ResourceWarningThreshold = 0.8;
	private const int QueueProcessingIntervalSeconds = 5;
	private const int DisposalTimeoutSeconds = 5;

	#endregion

	#region Fields

	private readonly ILogger<TaskManager> _logger;
	private readonly TaskManagerOptions _options;
	private readonly ConcurrentDictionary<string, InstanceManagedTask> _pendingTasks = new();
	private readonly ConcurrentDictionary<string, object> _pendingStreams = new();
	private readonly ConcurrentDictionary<string, Task> _timeoutMonitors = new();
	private readonly ConcurrentQueue<QueuedRequest> _queuedTasks = new();
	private readonly ConcurrentQueue<QueuedStreamRequest> _queuedStreams = new();
	private readonly TimeSpan _defaultTimeout;
	private readonly Timer _healthCheckTimer;
	private readonly Timer _queueProcessorTimer;
	private readonly SemaphoreSlim _cleanupLock = new(1, 1);
	private readonly SemaphoreSlim _queueLock = new(1, 1);
	private volatile bool _disposed;

	#endregion

	#region Constructor

	/// <summary>
	/// Initializes a new instance of the TaskManager with the specified configuration.
	/// </summary>
	/// <param name="logger">Logger instance for diagnostics and monitoring.</param>
	/// <param name="options">Configuration options for task management behavior.</param>
	/// <exception cref="ArgumentNullException">Thrown when logger or options is null.</exception>
	/// <exception cref="ArgumentException">Thrown when options validation fails.</exception>
	public TaskManager(ILogger<TaskManager> logger, IOptions<TaskManagerOptions> options)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		_options.Validate();

		_defaultTimeout = TimeSpan.FromSeconds(_options.DefaultTimeoutSeconds);

		var healthCheckInterval = TimeSpan.FromSeconds(_options.HealthCheckIntervalSeconds);
		_healthCheckTimer = new Timer(PerformHealthCheckSafely, null, healthCheckInterval, healthCheckInterval);
		_queueProcessorTimer = new Timer(ProcessQueueSafely, null,
			TimeSpan.FromSeconds(QueueProcessingIntervalSeconds),
			TimeSpan.FromSeconds(QueueProcessingIntervalSeconds));

		_logger.LogInformation("TaskManager initialized with: HealthCheck={HealthCheck}s, MaxTasks={MaxTasks}, MaxStreams={MaxStreams}, DefaultTimeout={DefaultTimeout}s",
			_options.HealthCheckIntervalSeconds, _options.MaxPendingTasks, _options.MaxPendingStreams, _options.DefaultTimeoutSeconds);
	}

	#endregion

	#region Health Check & Self-Healing

	/// <summary>
	/// Timer callback wrapper that safely executes health check operations.
	/// Prevents exceptions from terminating the timer.
	/// </summary>
	private void PerformHealthCheckSafely(object? state)
	{
		if (_disposed) return;

		_ = Task.Run(async () =>
		{
			try
			{
				await PerformHealthCheckAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception in health check background task");
			}
		});
	}

	/// <summary>
	/// Performs comprehensive health check with proper resource management.
	/// </summary>
	private async Task PerformHealthCheckAsync()
	{
		if (_disposed) return;

		await _cleanupLock.WaitAsync();
		try
		{
			var cleanupStats = await CleanupOrphanedResourcesAsync();
			LogResourceUsage();

			if (cleanupStats.OrphanedTasks > 0 || cleanupStats.CompletedMonitors > 0)
			{
				_logger.LogInformation("Health check completed: cleaned {Orphans} orphaned tasks, {Monitors} completed monitors",
					cleanupStats.OrphanedTasks, cleanupStats.CompletedMonitors);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during health check");
		}
		finally
		{
			_cleanupLock.Release();
		}
	}

	/// <summary>
	/// Cleans up orphaned tasks and completed monitors, returning cleanup statistics.
	/// </summary>
	private async Task<(int OrphanedTasks, int CompletedMonitors)> CleanupOrphanedResourcesAsync()
	{
		var now = DateTime.UtcNow;
		var orphanedTasks = 0;
		var completedMonitors = 0;

		completedMonitors += await CleanupCompletedMonitorsAsync();
		orphanedTasks += await CleanupOrphanedTasksAsync(now);
		orphanedTasks += await CleanupOrphanedStreamsAsync();

		return (orphanedTasks, completedMonitors);
	}

	/// <summary>
	/// Removes completed timeout monitors from tracking.
	/// </summary>
	private Task<int> CleanupCompletedMonitorsAsync()
	{
		var completedCount = 0;
		var keysToCheck = new List<string>(_timeoutMonitors.Keys);

		foreach (var key in keysToCheck)
		{
			if (_timeoutMonitors.TryGetValue(key, out var monitor) &&
				monitor.IsCompleted &&
				_timeoutMonitors.TryRemove(key, out _))
			{
				completedCount++;
			}
		}

		return Task.FromResult(completedCount);
	}

	/// <summary>
	/// Removes tasks that have exceeded the orphan timeout threshold.
	/// </summary>
	private Task<int> CleanupOrphanedTasksAsync(DateTime now)
	{
		var orphanedCount = 0;
		var orphanTimeout = _defaultTimeout.Add(_defaultTimeout);
		var keysToCheck = new List<string>(_pendingTasks.Keys);

		foreach (var key in keysToCheck)
		{
			if (!_pendingTasks.TryGetValue(key, out var task)) continue;

			var taskAge = now - task.CreatedAt;
			if (taskAge <= orphanTimeout) continue;

			if (_pendingTasks.TryRemove(key, out _))
			{
				orphanedCount++;
				task.TaskCompletionSource.TrySetException(
					new TimeoutException($"Task '{key}' was orphaned and removed after {taskAge.TotalSeconds:F1}s"));
				_logger.LogWarning("Removed orphaned task [{Request}] after {Seconds:F1}s", key, taskAge.TotalSeconds);
			}
		}

		return Task.FromResult(orphanedCount);
	}

	/// <summary>
	/// Removes streams that exist without corresponding timeout monitors.
	/// </summary>
	private Task<int> CleanupOrphanedStreamsAsync()
	{
		var orphanedCount = 0;
		var keysToCheck = new List<string>(_pendingStreams.Keys);

		foreach (var key in keysToCheck)
		{
			if (_timeoutMonitors.ContainsKey(key)) continue;

			if (_pendingStreams.TryRemove(key, out var stream))
			{
				orphanedCount++;
				_logger.LogWarning("Removed orphaned stream [{Request}] without monitor", key);

				if (stream is IDisposable disposable)
				{
					try
					{
						disposable.Dispose();
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Error disposing orphaned stream [{Request}]", key);
					}
				}
			}
		}

		return Task.FromResult(orphanedCount);
	}

	/// <summary>
	/// Logs current resource usage and warns if thresholds are exceeded.
	/// </summary>
	private void LogResourceUsage()
	{
		var taskCount = _pendingTasks.Count;
		var streamCount = _pendingStreams.Count;
		var monitorCount = _timeoutMonitors.Count;
		var queuedTaskCount = _queuedTasks.Count;
		var queuedStreamCount = _queuedStreams.Count;

		var taskThreshold = _options.MaxPendingTasks * ResourceWarningThreshold;
		var streamThreshold = _options.MaxPendingStreams * ResourceWarningThreshold;
		var monitorThreshold = _options.MaxTimeoutMonitors * ResourceWarningThreshold;

		if (taskCount > taskThreshold || streamCount > streamThreshold || monitorCount > monitorThreshold)
		{
			_logger.LogWarning("High resource usage detected - Tasks: {Tasks}/{MaxTasks}, Streams: {Streams}/{MaxStreams}, Monitors: {Monitors}/{MaxMonitors}, QueuedTasks: {QueuedTasks}, QueuedStreams: {QueuedStreams}",
				taskCount, _options.MaxPendingTasks, streamCount, _options.MaxPendingStreams,
				monitorCount, _options.MaxTimeoutMonitors, queuedTaskCount, queuedStreamCount);
		}
	}

	#endregion

	#region Queue Processing

	/// <summary>
	/// Timer callback wrapper that safely processes queued requests.
	/// Prevents exceptions from terminating the timer.
	/// </summary>
	private void ProcessQueueSafely(object? state)
	{
		if (_disposed) return;

		_ = Task.Run(async () =>
		{
			try
			{
				await ProcessQueueAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception in queue processing background task");
			}
		});
	}

	/// <summary>
	/// Processes queued tasks and streams when capacity becomes available.
	/// </summary>
	private async Task ProcessQueueAsync()
	{
		if (_disposed) return;

		await _queueLock.WaitAsync();
		try
		{
			await ProcessQueuedTasks();
			await ProcessQueuedStreams();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during queue processing");
		}
		finally
		{
			_queueLock.Release();
		}
	}

	/// <summary>
	/// Checks if there is available capacity for processing more tasks.
	/// Thread-safe capacity check with consistent state.
	/// </summary>
	private bool HasTaskCapacity() => _pendingTasks.Count < _options.MaxPendingTasks;

	/// <summary>
	/// Checks if there is available capacity for processing more streams.
	/// Thread-safe capacity check with consistent state.
	/// </summary>
	private bool HasStreamCapacity() => _pendingStreams.Count < _options.MaxPendingStreams;

	/// <summary>
	/// Processes queued task requests up to the configured batch size per cycle.
	/// </summary>
	private async Task ProcessQueuedTasks()
	{
		var processed = 0;
		var maxQueueTimeout = TimeSpan.FromSeconds(_options.MaxQueueTimeoutSeconds);

		while (_queuedTasks.TryDequeue(out var queuedRequest) && processed < MaxProcessingBatchSize)
		{
			processed++;

			if (IsQueuedRequestExpired(queuedRequest, maxQueueTimeout))
			{
				HandleExpiredQueuedRequest(queuedRequest, maxQueueTimeout);
				continue;
			}

			if (!HasTaskCapacity())
			{
				_queuedTasks.Enqueue(queuedRequest);
				break;
			}

			try
			{
				await ExecuteQueuedTaskInternal(queuedRequest);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing queued task [{Request}]", queuedRequest.RequestId);
				queuedRequest.TaskCompletionSource.TrySetException(ex);
			}
		}
	}

	/// <summary>
	/// Checks if a queued request has exceeded the maximum wait time.
	/// </summary>
	private bool IsQueuedRequestExpired(QueuedRequest request, TimeSpan maxTimeout)
	{
		return DateTime.UtcNow - request.QueuedAt > maxTimeout;
	}

	/// <summary>
	/// Handles a queued request that has expired by setting a timeout exception.
	/// </summary>
	private void HandleExpiredQueuedRequest(QueuedRequest request, TimeSpan maxTimeout)
	{
		var waitTime = DateTime.UtcNow - request.QueuedAt;
		_logger.LogWarning("Queued task [{Request}] timed out after {Seconds:F1}s in queue",
			request.RequestId, waitTime.TotalSeconds);
		request.TaskCompletionSource.TrySetException(
			new TimeoutException($"Request '{request.RequestId}' timed out in queue after {maxTimeout.TotalSeconds}s"));
	}

	/// <summary>
	/// Processes queued stream requests up to the configured batch size per cycle.
	/// </summary>
	private async Task ProcessQueuedStreams()
	{
		var processed = 0;
		var maxQueueTimeout = TimeSpan.FromSeconds(_options.MaxQueueTimeoutSeconds);

		while (_queuedStreams.TryDequeue(out var queuedRequest) && processed < MaxProcessingBatchSize)
		{
			processed++;

			if (IsQueuedStreamRequestExpired(queuedRequest, maxQueueTimeout))
			{
				HandleExpiredQueuedStreamRequest(queuedRequest, maxQueueTimeout);
				continue;
			}

			if (!HasStreamCapacity())
			{
				_queuedStreams.Enqueue(queuedRequest);
				break;
			}

			try
			{
				await ExecuteQueuedStreamInternal(queuedRequest);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing queued stream [{Request}]", queuedRequest.RequestId);
				queuedRequest.TaskCompletionSource.TrySetException(ex);
			}
		}
	}

	/// <summary>
	/// Checks if a queued stream request has exceeded the maximum wait time.
	/// </summary>
	private bool IsQueuedStreamRequestExpired(QueuedStreamRequest request, TimeSpan maxTimeout)
	{
		return DateTime.UtcNow - request.QueuedAt > maxTimeout;
	}

	/// <summary>
	/// Handles a queued stream request that has expired by setting a timeout exception.
	/// </summary>
	private void HandleExpiredQueuedStreamRequest(QueuedStreamRequest request, TimeSpan maxTimeout)
	{
		var waitTime = DateTime.UtcNow - request.QueuedAt;
		_logger.LogWarning("Queued stream [{Request}] timed out after {Seconds:F1}s in queue",
			request.RequestId, waitTime.TotalSeconds);
		request.TaskCompletionSource.TrySetException(
			new TimeoutException($"Stream request '{request.RequestId}' timed out in queue after {maxTimeout.TotalSeconds}s"));
	}

	private async Task ExecuteQueuedTaskInternal(QueuedRequest queuedRequest)
	{
		var tcs = new TaskCompletionSource<TrackedObject>(TaskCreationOptions.RunContinuationsAsynchronously);
		var managed = new InstanceManagedTask(queuedRequest.InstanceId, tcs);

		if (!_pendingTasks.TryAdd(queuedRequest.RequestId, managed))
		{
			queuedRequest.TaskCompletionSource.TrySetException(
				new InvalidOperationException($"Request '{queuedRequest.RequestId}' already pending"));
			return;
		}

		_logger.LogInformation("Processing queued request [{Request}] on instance [{Instance}]",
			queuedRequest.RequestId, queuedRequest.InstanceId);

		try
		{
			_ = queuedRequest.DispatchAsync().ContinueWith(t => HandleDispatchCompletion(t, queuedRequest.RequestId, tcs),
				TaskContinuationOptions.ExecuteSynchronously);
			var result = await WaitForCompletion<TrackedObject>(queuedRequest.RequestId, tcs.Task, queuedRequest.CancellationToken);
			queuedRequest.TaskCompletionSource.TrySetResult(result);
		}
		catch (Exception ex)
		{
			queuedRequest.TaskCompletionSource.TrySetException(ex);
		}
		finally
		{
			_pendingTasks.TryRemove(queuedRequest.RequestId, out _);
		}
	}

	private async Task ExecuteQueuedStreamInternal(QueuedStreamRequest queuedRequest)
	{
		await ExecuteQueuedChannelStreamInternal(queuedRequest);
	}

	private async Task ExecuteQueuedChannelStreamInternal(QueuedStreamRequest queuedRequest)
	{
		var channel = Channel.CreateUnbounded<object>();
		var tcs = new TaskCompletionSource<ChannelReader<object>>(TaskCreationOptions.RunContinuationsAsynchronously);
		var channelStreamTask = new ChannelStreamTask<object>(queuedRequest.InstanceId, queuedRequest.ConnectionId, tcs, channel.Writer);

		if (!_pendingStreams.TryAdd(queuedRequest.RequestId, channelStreamTask))
		{
			queuedRequest.TaskCompletionSource.TrySetException(
				new InvalidOperationException($"Channel stream request '{queuedRequest.RequestId}' already pending"));
			return;
		}

		var monitorTask = MonitorStreamTimeout(queuedRequest.RequestId, channel.Writer);
		if (!_timeoutMonitors.TryAdd(queuedRequest.RequestId, monitorTask))
		{
			_logger.LogWarning("Failed to add timeout monitor for queued request [{Request}]", queuedRequest.RequestId);
		}

		try
		{
			await queuedRequest.StreamInvoker();
			tcs.TrySetResult(channel.Reader);
			var result = await tcs.Task;
			queuedRequest.TaskCompletionSource.TrySetResult(result);
		}
		catch (Exception ex)
		{
			channel.Writer.TryComplete(ex);
			tcs.TrySetException(ex);
			queuedRequest.TaskCompletionSource.TrySetException(ex);
			await CleanupStreamRequest(queuedRequest.RequestId);
		}
	}


	#endregion

	#region Task Execution

	/// <summary>
	/// Executes a fire-and-forget dispatch and waits for an external response.
	/// </summary>
	public Task<TResponse> ExternalExecuteAsync<TResponse>(
		string instanceId,
		string requestId,
		Func<Task> dispatchAsync,
		CancellationToken cancellationToken = default)
		where TResponse : TrackedObject
	{
		return InternalExecuteAsync<TResponse>(instanceId, requestId, dispatchAsync, cancellationToken);
	}

	/// <summary>
	/// Executes a dispatch that immediately returns a value and optionally waits for an external response.
	/// </summary>
	public Task<TResponse> ExecuteAsync<TResponse>(
		string instanceId,
		string requestId,
		Func<Task<TResponse>> dispatchAsync,
		CancellationToken cancellationToken = default)
		where TResponse : TrackedObject
	{
		async Task wrapper()
		{
			var local = await dispatchAsync().ConfigureAwait(false);
			SetTaskResult(requestId, local);
		}

		return InternalExecuteAsync<TResponse>(instanceId, requestId, wrapper, cancellationToken);
	}

	private async Task<TResponse> InternalExecuteAsync<TResponse>(
		string instanceId,
		string requestId,
		Func<Task> dispatchAsync,
		CancellationToken cancellationToken)
		where TResponse : TrackedObject
	{
		ThrowIfDisposed();
		Validate(instanceId, requestId);

		_logger.LogInformation("Executing request [{Request}] on instance [{Instance}]", requestId, instanceId);

		if (!HasTaskCapacity())
		{
			_logger.LogInformation("Task capacity reached ({Count}/{Max}), queueing request [{Request}]",
				_pendingTasks.Count, _options.MaxPendingTasks, requestId);

			var queueTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
			var queuedRequest = new QueuedRequest(instanceId, requestId, dispatchAsync, cancellationToken, queueTcs, typeof(TResponse));
			_queuedTasks.Enqueue(queuedRequest);

			var queuedResult = await queueTcs.Task.ConfigureAwait(false);
			return (TResponse)queuedResult;
		}

		var tcs = new TaskCompletionSource<TrackedObject>(TaskCreationOptions.RunContinuationsAsynchronously);
		var managed = new InstanceManagedTask(instanceId, tcs);
		if (!_pendingTasks.TryAdd(requestId, managed))
			throw new InvalidOperationException($"Request '{requestId}' already pending");

		_logger.LogInformation("[{Instance}] Dispatch {Request}", instanceId, requestId);
		try
		{
			_ = dispatchAsync().ContinueWith(t => HandleDispatchCompletion(t, requestId, tcs),
				TaskContinuationOptions.ExecuteSynchronously);
			var response = await WaitForCompletion<TResponse>(requestId, tcs.Task, cancellationToken).ConfigureAwait(false);
			return response;
		}
		finally
		{
			_logger.LogInformation("Completed request [{Request}] on instance [{Instance}]", requestId, instanceId);
			_pendingTasks.TryRemove(requestId, out _);
		}
	}

	private void HandleDispatchCompletion(
		Task dispatchTask,
		string requestId,
		TaskCompletionSource<TrackedObject> tcs)
	{
		if (dispatchTask.IsFaulted)
		{
			_logger.LogError(dispatchTask.Exception, "Dispatch failed for {Request}", requestId);
			tcs.TrySetException(dispatchTask.Exception!);
		}
		else if (dispatchTask.IsCanceled)
		{
			_logger.LogWarning("Dispatch canceled for {Request}", requestId);
			tcs.TrySetCanceled();
		}
	}

	/// <summary>
	/// Sets the result for a pending task, typically called by external systems (e.g., external services, SignalR hubs).
	/// </summary>
	/// <typeparam name="TResponse">The type of the response object, must inherit from TrackedObject.</typeparam>
	/// <param name="requestId">The unique identifier of the pending request.</param>
	/// <param name="result">The result object to set for the pending task.</param>
	/// <remarks>
	/// This method assigns the requestId to the result object and completes the associated TaskCompletionSource.
	/// If no pending task exists for the requestId, a warning is logged but no exception is thrown.
	/// This is used for regular request-response patterns, not for stream operations.
	/// </remarks>
	public void SetTaskResult<TResponse>(string requestId, TResponse result)
		where TResponse : TrackedObject
	{
		ThrowIfDisposed();
		result.RequestId = requestId;
		if (_pendingTasks.TryGetValue(requestId, out var managed))
		{
			_logger.LogDebug("Setting result for {Request}", requestId);
			managed.TaskCompletionSource.TrySetResult(result);
		}
		else
		{
			_logger.LogWarning("No pending for {Request}", requestId);
		}
	}

	#endregion

	#region Stream Operations



	/// <summary>
	/// Gets a channel-based stream response asynchronously for the specified request.
	/// This method creates a buffered channel that will be populated by SetChannelStreamResultAsync.
	/// </summary>
	public async Task<ChannelReader<TResult>> GetChannelStreamResponseAsync<TResult>(
		string instanceId,
		string connectionId,
		string requestId,
		Func<Task> streamInvoker,
		CancellationToken cancellationToken = default)
	{
		ThrowIfDisposed();
		Validate(instanceId, requestId);

		_logger.LogInformation("Executing channel stream request [{Request}] on instance [{Instance}] for connection [{Connection}]", requestId, instanceId, connectionId);

		if (!HasStreamCapacity())
		{
			_logger.LogInformation("Stream capacity reached ({Count}/{Max}), queueing channel stream [{Request}]",
				_pendingStreams.Count, _options.MaxPendingStreams, requestId);

			var queueTcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
			var queuedRequest = new QueuedStreamRequest(instanceId, connectionId, requestId, streamInvoker, cancellationToken, queueTcs, typeof(TResult));
			_queuedStreams.Enqueue(queuedRequest);

			var queuedResult = await queueTcs.Task.ConfigureAwait(false);
			return (ChannelReader<TResult>)queuedResult;
		}

		var channel = Channel.CreateUnbounded<TResult>();
		var tcs = new TaskCompletionSource<ChannelReader<TResult>>(TaskCreationOptions.RunContinuationsAsynchronously);
		var channelStreamTask = new ChannelStreamTask<TResult>(instanceId, connectionId, tcs, channel.Writer);

		if (!_pendingStreams.TryAdd(requestId, channelStreamTask))
			throw new InvalidOperationException($"Channel stream request '{requestId}' already pending");

		var monitorTask = MonitorStreamTimeout(requestId, channel.Writer);
		if (!_timeoutMonitors.TryAdd(requestId, monitorTask))
		{
			_logger.LogWarning("Failed to add timeout monitor for request [{Request}]", requestId);
		}

		try
		{
			await streamInvoker().ConfigureAwait(false);

			tcs.TrySetResult(channel.Reader);
			return await tcs.Task.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error executing channel stream request [{Request}] for connection [{Connection}]", requestId, connectionId);
			channel.Writer.TryComplete(ex);
			tcs.TrySetException(ex);

			await CleanupStreamRequest(requestId);
			throw;
		}
	}

	/// <summary>
	/// Monitors a stream request for timeout and cleans it up if necessary.
	/// </summary>
	private async Task MonitorStreamTimeout<TResult>(string requestId, ChannelWriter<TResult> writer)
	{
		try
		{
			await Task.Delay(_defaultTimeout).ConfigureAwait(false);

			// Use lock-free coordination to prevent race conditions
			if (_pendingStreams.TryGetValue(requestId, out var streamTask) &&
				_pendingStreams.TryRemove(requestId, out _))
			{
				_logger.LogWarning("Channel stream request [{Request}] timed out after {Timeout}s", requestId, _defaultTimeout.TotalSeconds);

				// Complete the channel writer with timeout exception
				writer.TryComplete(new TimeoutException($"Stream request '{requestId}' timed out after {_defaultTimeout.TotalSeconds}s"));

				// Dispose the stream task if it implements IDisposable
				if (streamTask is IDisposable disposable)
				{
					try
					{
						disposable.Dispose();
					}
					catch (Exception disposeEx)
					{
						_logger.LogWarning(disposeEx, "Error disposing timed out stream [{Request}]", requestId);
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error monitoring stream timeout for request [{Request}]", requestId);
		}
		finally
		{
			_timeoutMonitors.TryRemove(requestId, out _);
		}
	}

	/// <summary>
	/// Cleans up all resources associated with a stream request.
	/// Coordinates with timeout monitor to prevent race conditions.
	/// </summary>
	private async Task CleanupStreamRequest(string requestId)
	{
		try
		{
			// Remove stream first, then handle monitor cleanup
			if (_pendingStreams.TryRemove(requestId, out var removedStream))
			{
				// Dispose the stream if it implements IDisposable
				if (removedStream is IDisposable disposable)
				{
					try
					{
						disposable.Dispose();
					}
					catch (Exception disposeEx)
					{
						_logger.LogWarning(disposeEx, "Error disposing stream during cleanup [{Request}]", requestId);
					}
				}
			}

			// Clean up timeout monitor if it exists
			if (_timeoutMonitors.TryRemove(requestId, out var monitor))
			{
				try
				{
					// Give monitor a chance to complete, but don't wait indefinitely
					var timeout = Task.Delay(TimeSpan.FromSeconds(2));
					var completed = await Task.WhenAny(monitor, timeout);

					if (completed == timeout)
					{
						_logger.LogWarning("Timeout monitor for [{Request}] did not complete within cleanup timeout", requestId);
					}
				}
				catch (Exception monitorEx)
				{
					_logger.LogWarning(monitorEx, "Error waiting for timeout monitor during cleanup [{Request}]", requestId);
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error cleaning up stream request [{Request}]", requestId);
		}
	}

	/// <summary>
	/// Sets the channel stream result from a ChannelReader provided by the client.
	/// This method copies data from the provided ChannelReader to the internal buffered channel
	/// to ensure data is available when the consumer reads it.
	/// </summary>
	public async Task SetChannelStreamResultAsync<TResult>(string requestId, ChannelReader<TResult> reader)
	{
		ThrowIfDisposed();
		_logger.LogInformation("Attempting to set channel stream result for request {RequestId}", requestId);

		if (!_pendingStreams.TryGetValue(requestId, out var task))
		{
			_logger.LogWarning("No pending channel stream found for request {RequestId}. Available streams: {AvailableStreams}",
				requestId, string.Join(", ", _pendingStreams.Keys));
			return;
		}

		if (task is not ChannelStreamTask<TResult> channelStreamTask)
		{
			_logger.LogWarning("Type mismatch for channel stream request {Request}. Expected {Expected}, got {Actual}",
				requestId, typeof(ChannelStreamTask<TResult>).Name, task.GetType().Name);
			return;
		}

		_logger.LogInformation("Successfully setting channel stream result for request {RequestId}", requestId);

		if (channelStreamTask.Writer == null)
		{
			_logger.LogError("Channel writer is null for request {RequestId}", requestId);
			return;
		}

		try
		{
			await foreach (var item in reader.ReadAllAsync())
			{
				if (!channelStreamTask.Writer.TryWrite(item))
				{
					_logger.LogWarning("Failed to write item to channel for request {RequestId}", requestId);
					break;
				}
			}

			// Complete successfully after all data is written
			channelStreamTask.Writer.TryComplete();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error copying channel stream for request {RequestId}", requestId);
			channelStreamTask.Writer.TryComplete(ex);
			throw;
		}
	}

	#endregion

	#region Task Management

	/// <summary>
	/// Cancels a pending task with the specified request identifier.
	/// </summary>
	/// <param name="requestId">The unique identifier of the request to cancel.</param>
	/// <remarks>
	/// If the request exists, its TaskCompletionSource will be set to canceled state.
	/// If no pending request is found, a warning is logged but no exception is thrown.
	/// </remarks>
	public void CancelTask(string requestId)
	{
		if (_pendingTasks.TryRemove(requestId, out var managed))
		{
			_logger.LogInformation("Cancel {Request}", requestId);
			managed.TaskCompletionSource.TrySetCanceled();
		}
		else
		{
			_logger.LogWarning("No pending for {Request}", requestId);
		}
	}

	/// <summary>
	/// Retrieves the instance identifier associated with a pending request.
	/// </summary>
	/// <param name="requestId">The unique identifier of the request.</param>
	/// <returns>The instance identifier associated with the specified request.</returns>
	/// <exception cref="InvalidOperationException">Thrown when no pending request exists for the specified requestId.</exception>
	/// <remarks>
	/// This method only works for regular tasks managed via ExternalExecuteAsync or ExecuteAsync.
	/// Stream requests are managed separately and will not be found by this method.
	/// </remarks>
	public string GetRequestInstanceId(string requestId)
	{
		ThrowIfDisposed();
		if (!_pendingTasks.TryGetValue(requestId, out var managed))
			throw new InvalidOperationException($"No pending for {requestId}");
		return managed.InstanceId;
	}

	#endregion

	#region Private Utility Methods

	/// <summary>
	/// Validates that required parameters are not null or whitespace.
	/// </summary>
	/// <param name="instanceId">The instance identifier to validate.</param>
	/// <param name="requestId">The request identifier to validate.</param>
	/// <exception cref="ArgumentException">Thrown when instanceId or requestId is null or whitespace.</exception>
	private void Validate(string instanceId, string requestId)
	{
		if (string.IsNullOrWhiteSpace(instanceId))
			throw new ArgumentException("Instance ID is required", nameof(instanceId));
		if (string.IsNullOrWhiteSpace(requestId))
			throw new ArgumentException("Request ID is required", nameof(requestId));
	}

	private void ThrowIfDisposed()
	{
		if (_disposed)
			throw new ObjectDisposedException(nameof(TaskManager));
	}

	private async Task<TResponse> WaitForCompletion<TResponse>(
		string requestId,
		Task<TrackedObject> responseTask,
		CancellationToken cancellationToken)
		where TResponse : TrackedObject
	{
		if (cancellationToken == default)
		{
			using var cts = new CancellationTokenSource(_defaultTimeout);
			var delay = Task.Delay(_defaultTimeout, cts.Token);
			var completed = await Task.WhenAny(responseTask, delay).ConfigureAwait(false);
			if (completed != responseTask)
				throw new TimeoutException($"'{requestId}' timed out after {_defaultTimeout.TotalSeconds}s");

			cts.Cancel();
			return (TResponse)await responseTask.ConfigureAwait(false);
		}
		else
		{
			using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			cts.CancelAfter(_defaultTimeout);

			var delay = Task.Delay(_defaultTimeout, cts.Token);
			var completed = await Task.WhenAny(responseTask, delay).ConfigureAwait(false);
			if (completed != responseTask)
				throw new TimeoutException($"'{requestId}' timed out after {_defaultTimeout.TotalSeconds}s");

			return (TResponse)await responseTask.ConfigureAwait(false);
		}
	}

	#endregion

	#region IDisposable

	public void Dispose()
	{
		if (_disposed) return;

		_disposed = true;

		try
		{
			_healthCheckTimer?.Dispose();
			_queueProcessorTimer?.Dispose();

			foreach (var kvp in _pendingTasks)
			{
				kvp.Value.TaskCompletionSource.TrySetCanceled();
			}
			_pendingTasks.Clear();

			foreach (var kvp in _pendingStreams)
			{
				if (kvp.Value is IDisposable disposable)
				{
					try
					{
						disposable.Dispose();
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error disposing stream {RequestId}", kvp.Key);
					}
				}
			}
			_pendingStreams.Clear();

			var allMonitors = _timeoutMonitors.Values.ToArray();
			try
			{
				var waitResult = Task.WaitAll(allMonitors, TimeSpan.FromSeconds(DisposalTimeoutSeconds));
				if (!waitResult)
				{
					_logger.LogWarning("Some timeout monitors did not complete within {Timeout} seconds during disposal", DisposalTimeoutSeconds);
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Error waiting for timeout monitors during disposal");
			}
			_timeoutMonitors.Clear();

			_cleanupLock?.Dispose();
			_queueLock?.Dispose();

			_logger.LogInformation("TaskManager disposed successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error during TaskManager disposal");
		}
	}

	#endregion
}
