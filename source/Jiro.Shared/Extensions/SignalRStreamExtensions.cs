using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Jiro.Shared.Extensions;

/// <summary>
/// Extension methods for SignalR stream handling to reduce code duplication.
/// </summary>
public static class SignalRStreamExtensions
{
	/// <summary>
	/// Registers a stream handler that returns an IAsyncEnumerable for the specified event.
	/// This is the preferred method for handling server-to-client streaming requests.
	/// </summary>
	/// <typeparam name="TRequest">The type of the request parameter</typeparam>
	/// <typeparam name="TResponse">The type of the response stream items</typeparam>
	/// <param name="hubConnection">The SignalR hub connection</param>
	/// <param name="eventName">The name of the event to handle</param>
	/// <param name="streamProvider">Function that provides the stream for a given request</param>
	/// <param name="logger">Optional logger for diagnostics</param>
	public static void OnStream<TRequest, TResponse>(
		this HubConnection hubConnection,
		string eventName,
		Func<TRequest, IAsyncEnumerable<TResponse>> streamProvider,
		ILogger? logger = null)
	{
		if (streamProvider == null)
		{
			logger?.LogWarning("Stream provider for {EventName} is null - stream requests will not be processed", eventName);
			return;
		}

		hubConnection.On<TRequest, IAsyncEnumerable<TResponse>>(
			eventName,
			request =>
			{
				logger?.LogInformation("{EventName} stream requested: {@Request}", eventName, request);

				try
				{
					var stream = streamProvider(request);
					logger?.LogInformation("{EventName} stream created successfully: {@Request}", eventName, request);
					return stream;
				}
				catch (Exception ex)
				{
					logger?.LogError(ex, "{EventName} stream provider failed: {@Request}", eventName, request);
					throw;
				}
			});

		logger?.LogDebug("Stream handler registered for {EventName}", eventName);
	}

	/// <summary>
	/// Registers a request-response handler with logging for the specified event.
	/// This method handles standard RPC-style calls where server expects a single response.
	/// </summary>
	/// <typeparam name="TRequest">The type of the request parameter</typeparam>
	/// <typeparam name="TResponse">The type of the response</typeparam>
	/// <param name="hubConnection">The SignalR hub connection</param>
	/// <param name="eventName">The name of the event to handle</param>
	/// <param name="handler">Function that handles the request and returns a response</param>
	/// <param name="logger">Optional logger for diagnostics</param>
	/// <param name="requestIdSelector">Optional function to extract request ID for logging</param>
	public static void OnRequest<TRequest, TResponse>(
		this HubConnection hubConnection,
		string eventName,
		Func<TRequest, Task<TResponse>> handler,
		ILogger? logger = null,
		Func<TRequest, object>? requestIdSelector = null)
	{
		if (handler == null)
		{
			logger?.LogWarning("Event handler for {EventName} is null - requests will not be processed", eventName);
			return;
		}

		hubConnection.On<TRequest, TResponse>(
			eventName,
			async request =>
			{
				var requestId = requestIdSelector?.Invoke(request);
				logger?.LogInformation("{EventName} received: {RequestId}, Request: {@Request}", eventName, requestId, request);

				try
				{
					var response = await handler(request);
					logger?.LogInformation("{EventName} handled successfully: {RequestId}, Response: {@Response}", eventName, requestId, response);
					return response;
				}
				catch (Exception ex)
				{
					logger?.LogError(ex, "{EventName} handler failed: {RequestId}", eventName, requestId);
					throw;
				}
			});

		logger?.LogDebug("Event handler registered for {EventName}", eventName);
	}

	/// <summary>
	/// Registers a fire-and-forget event handler with logging for the specified event.
	/// This method handles notifications where no response is expected.
	/// </summary>
	/// <typeparam name="T">The type of the event parameter</typeparam>
	/// <param name="hubConnection">The SignalR hub connection</param>
	/// <param name="eventName">The name of the event to handle</param>
	/// <param name="handler">Function that handles the event</param>
	/// <param name="logger">Optional logger for diagnostics</param>
	public static void OnNotification<T>(
		this HubConnection hubConnection,
		string eventName,
		Func<T, Task> handler,
		ILogger? logger = null)
	{
		if (handler == null)
		{
			logger?.LogWarning("Event handler for {EventName} is null - notifications will not be processed", eventName);
			return;
		}

		hubConnection.On<T>(eventName, async data =>
		{
			logger?.LogInformation("{EventName} received, Data: {@Data}", eventName, data);

			try
			{
				await handler(data);
				logger?.LogInformation("{EventName} executed successfully", eventName);
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, "{EventName} handler failed", eventName);
				throw;
			}
		});

		logger?.LogDebug("Event handler registered for {EventName}", eventName);
	}

	/// <summary>
	/// Registers a parameterless fire-and-forget event handler with logging for the specified event.
	/// This method handles simple notifications where no parameters or response are expected.
	/// </summary>
	/// <param name="hubConnection">The SignalR hub connection</param>
	/// <param name="eventName">The name of the event to handle</param>
	/// <param name="handler">Function that handles the event</param>
	/// <param name="logger">Optional logger for diagnostics</param>
	public static void OnNotification(
		this HubConnection hubConnection,
		string eventName,
		Func<Task> handler,
		ILogger? logger = null)
	{
		if (handler == null)
		{
			logger?.LogWarning("Event handler for {EventName} is null - notifications will not be processed", eventName);
			return;
		}

		hubConnection.On(eventName, async () =>
		{
			logger?.LogInformation("{EventName} received", eventName);

			try
			{
				await handler();
				logger?.LogInformation("{EventName} executed successfully", eventName);
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, "{EventName} handler failed", eventName);
				throw;
			}
		});

		logger?.LogDebug("Event handler registered for {EventName}", eventName);
	}
}
