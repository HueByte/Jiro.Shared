
using System.Threading.Channels;

namespace Jiro.Shared.Tasks;

/// <summary>
/// Defines methods for managing asynchronous tasks and their responses.
/// </summary>
public interface ITaskManager
{
	/// <summary>
	/// Cancels a task with the specified request ID.
	/// </summary>
	/// <param name="requestId">The request ID of the task to cancel.</param>
	/// <exception cref="ArgumentException">Thrown if the request ID is null or empty.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the request ID does not exist.</exception>
	void CancelTask(string requestId);

	/// <summary>
	/// Executes a task asynchronously and waits for a response.
	/// </summary>
	/// <typeparam name="TResponse">The type of the response expected.</typeparam>
	/// <param name="instanceId">The ID of the instance to execute the task on.</param>
	/// <param name="requestId">The unique request ID for the task.</param>
	/// <param name="task">The asynchronous task to execute.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous operation, with a response of type <typeparamref name="TResponse"/>.</returns>
	/// <exception cref="ArgumentException">Thrown if the instance ID or request ID is null or empty.</exception>
	Task<TResponse> ExternalExecuteAsync<TResponse>(string instanceId, string requestId, Func<Task> task, CancellationToken cancellationToken = default) where TResponse : TrackedObject;

	/// <summary>
	/// Executes a dispatch that immediately returns a value and optionally waits for an external response.
	/// </summary>
	/// <typeparam name="TResponse">The type of the response expected.</typeparam>
	/// <param name="instanceId">The ID of the instance to execute the task on.</param>
	/// <param name="requestId">The unique request ID for the task.</param>
	/// <param name="dispatchAsync">The asynchronous function to execute.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous operation, with a response of type <typeparamref name="TResponse"/>.</returns>
	public Task<TResponse> ExecuteAsync<TResponse>(string instanceId, string requestId, Func<Task<TResponse>> dispatchAsync, CancellationToken cancellationToken = default) where TResponse : TrackedObject;

	/// <summary>
	/// Sets the result for a pending task with the specified request ID.
	/// </summary>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	/// <param name="requestId">The request ID of the task.</param>
	/// <param name="result">The result to set for the task.</param>
	void SetTaskResult<TResponse>(string requestId, TResponse result) where TResponse : TrackedObject;



	/// <summary>
	/// Gets a channel-based stream response asynchronously for the specified request.
	/// </summary>
	/// <typeparam name="TResult">The type of items in the stream.</typeparam>
	/// <param name="instanceId">The ID of the instance to execute the stream request on.</param>
	/// <param name="connectionId">The ID of the connection for the stream.</param>
	/// <param name="requestId">The unique request ID for the stream.</param>
	/// <param name="streamInvoker">The function to invoke to initiate the stream.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous operation, returning a channel reader.</returns>
	/// <exception cref="ArgumentException">Thrown if the instance ID or request ID is null or empty.</exception>
	/// <exception cref="InvalidOperationException">Thrown if the request ID is already pending.</exception>
	Task<ChannelReader<TResult>> GetChannelStreamResponseAsync<TResult>(
		string instanceId,
		string connectionId,
		string requestId,
		Func<Task> streamInvoker,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Sets the channel stream result from a ChannelReader.
	/// </summary>
	/// <typeparam name="TResult">The type of items in the stream.</typeparam>
	/// <param name="requestId">The request ID of the stream.</param>
	/// <param name="reader">The channel reader to set as the result.</param>
	Task SetChannelStreamResultAsync<TResult>(string requestId, ChannelReader<TResult> reader);
}
