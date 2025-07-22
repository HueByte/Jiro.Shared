using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Jiro.Shared.Extensions;

/// <summary>
/// Extension methods for streaming data using channels
/// </summary>
public static class StreamingExtensions
{
	/// <summary>
	/// Converts an IAsyncEnumerable to a ChannelReader for streaming data
	/// </summary>
	/// <typeparam name="T">The type of items to stream</typeparam>
	/// <param name="source">The async enumerable source</param>
	/// <param name="capacity">The channel capacity (unbounded if null)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A ChannelReader that can be used to read the streamed items</returns>
	public static ChannelReader<T> ToChannelReader<T>(
		this IAsyncEnumerable<T> source,
		int? capacity = null,
		CancellationToken cancellationToken = default)
	{
		var channel = capacity.HasValue
			? Channel.CreateBounded<T>(new BoundedChannelOptions(capacity.Value)
			{
				FullMode = BoundedChannelFullMode.Wait,
				SingleWriter = true,
				SingleReader = false
			})
			: Channel.CreateUnbounded<T>(new UnboundedChannelOptions
			{
				SingleWriter = true,
				SingleReader = false
			});

		_ = Task.Run(async () =>
		{
			try
			{
				await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
				{
					await channel.Writer.WriteAsync(item, cancellationToken).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected when cancellation is requested
			}
			catch (Exception ex)
			{
				channel.Writer.TryComplete(ex);
				return;
			}

			channel.Writer.TryComplete();
		}, cancellationToken);

		return channel.Reader;
	}

	/// <summary>
	/// Streams data from a ChannelReader to an IAsyncEnumerable
	/// </summary>
	/// <typeparam name="T">The type of items to stream</typeparam>
	/// <param name="reader">The channel reader source</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>An async enumerable that yields items from the channel</returns>
	public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
		this ChannelReader<T> reader,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		await foreach (var item in reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
		{
			yield return item;
		}
	}

	/// <summary>
	/// Creates a streaming pipeline that transforms items as they flow through
	/// </summary>
	/// <typeparam name="TSource">The source item type</typeparam>
	/// <typeparam name="TResult">The result item type</typeparam>
	/// <param name="source">The source async enumerable</param>
	/// <param name="transform">The transformation function</param>
	/// <param name="capacity">The channel capacity (unbounded if null)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A ChannelReader that yields transformed items</returns>
	public static ChannelReader<TResult> StreamTransform<TSource, TResult>(
		this IAsyncEnumerable<TSource> source,
		Func<TSource, Task<TResult>> transform,
		int? capacity = null,
		CancellationToken cancellationToken = default)
	{
		var channel = capacity.HasValue
			? Channel.CreateBounded<TResult>(new BoundedChannelOptions(capacity.Value)
			{
				FullMode = BoundedChannelFullMode.Wait,
				SingleWriter = true,
				SingleReader = false
			})
			: Channel.CreateUnbounded<TResult>(new UnboundedChannelOptions
			{
				SingleWriter = true,
				SingleReader = false
			});

		_ = Task.Run(async () =>
		{
			try
			{
				await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
				{
					var transformed = await transform(item).ConfigureAwait(false);
					await channel.Writer.WriteAsync(transformed, cancellationToken).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected when cancellation is requested
			}
			catch (Exception ex)
			{
				channel.Writer.TryComplete(ex);
				return;
			}

			channel.Writer.TryComplete();
		}, cancellationToken);

		return channel.Reader;
	}

	/// <summary>
	/// Merges multiple async enumerable streams into a single channel reader
	/// </summary>
	/// <typeparam name="T">The type of items to stream</typeparam>
	/// <param name="sources">The source async enumerables to merge</param>
	/// <param name="capacity">The channel capacity (unbounded if null)</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>A ChannelReader that yields items from all sources</returns>
	public static ChannelReader<T> MergeStreams<T>(
		this IEnumerable<IAsyncEnumerable<T>> sources,
		int? capacity = null,
		CancellationToken cancellationToken = default)
	{
		var channel = capacity.HasValue
			? Channel.CreateBounded<T>(new BoundedChannelOptions(capacity.Value)
			{
				FullMode = BoundedChannelFullMode.Wait,
				SingleWriter = false,
				SingleReader = false
			})
			: Channel.CreateUnbounded<T>(new UnboundedChannelOptions
			{
				SingleWriter = false,
				SingleReader = false
			});

		var tasks = sources.Select(source => Task.Run(async () =>
		{
			try
			{
				await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
				{
					await channel.Writer.WriteAsync(item, cancellationToken).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException)
			{
				// Expected when cancellation is requested
			}
		}, cancellationToken)).ToArray();

		_ = Task.Run(async () =>
		{
			try
			{
				await Task.WhenAll(tasks).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				channel.Writer.TryComplete(ex);
				return;
			}

			channel.Writer.TryComplete();
		}, cancellationToken);

		return channel.Reader;
	}
}