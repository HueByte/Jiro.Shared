using System.Threading.Channels;

namespace Jiro.Shared.Tasks.Models;

/// <summary>
/// Represents a pending channel-based stream task that waits for a client to provide a ChannelReader.
/// This struct is used internally by TaskManager to track channel stream requests and their completion sources.
/// </summary>
/// <typeparam name="TResult">The type of items expected in the channel stream.</typeparam>
public readonly struct ChannelStreamTask<TResult>
{
	/// <summary>
	/// The unique identifier of the instance handling this stream request.
	/// </summary>
	public readonly string InstanceId;

	/// <summary>
	/// The connection identifier associated with this stream request.
	/// </summary>
	public readonly string ConnectionId;

	/// <summary>
	/// The task completion source that will be resolved when the client provides a ChannelReader.
	/// </summary>
	public readonly TaskCompletionSource<ChannelReader<TResult>> TaskCompletionSource;

	/// <summary>
	/// The channel writer used to buffer stream data from the client.
	/// Data from the client's ChannelReader is copied to this writer to ensure availability.
	/// </summary>
	public readonly ChannelWriter<TResult>? Writer;

	/// <summary>
	/// Initializes a new instance of the ChannelStreamTask struct.
	/// </summary>
	/// <param name="instanceId">The unique identifier of the instance handling this stream request.</param>
	/// <param name="connectionId">The connection identifier associated with this stream request.</param>
	/// <param name="taskCompletionSource">The task completion source that will be resolved when the client provides a ChannelReader.</param>
	/// <param name="writer">The channel writer for legacy compatibility (can be null for direct channel passing).</param>
	public ChannelStreamTask(string instanceId, string connectionId, TaskCompletionSource<ChannelReader<TResult>> taskCompletionSource, ChannelWriter<TResult>? writer)
	{
		InstanceId = instanceId;
		ConnectionId = connectionId;
		TaskCompletionSource = taskCompletionSource;
		Writer = writer;
	}
}
