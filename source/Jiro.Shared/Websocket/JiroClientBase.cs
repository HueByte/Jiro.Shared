
using System.Threading.Channels;

using Jiro.Shared.Extensions;
using Jiro.Shared.Websocket.Requests;
using Jiro.Shared.Websocket.Responses;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Jiro.Shared.Websocket;

/// <summary>
/// Base class for Jiro client implementations. Provides automatic event wiring for SignalR hub connections.
/// </summary>
public abstract class JiroClientBase : IJiroClient
{
	/// <summary>
	/// The SignalR hub connection used for communication
	/// </summary>
	protected HubConnection _hubConnection;

	/// <summary>
	/// Logger instance for logging events
	/// </summary>
	protected ILogger<JiroClientBase>? _logger;

	/// <summary>
	/// Semaphore for connection synchronization
	/// </summary>
	protected readonly SemaphoreSlim _connectionSemaphore = new(1, 1);

	#region Connection Lifecycle Events

	/// <summary>
	/// Event fired when connection is closed
	/// </summary>
	public event Func<Exception?, Task>? Closed;

	/// <summary>
	/// Event fired when connection is reconnecting
	/// </summary>
	public event Func<Exception?, Task>? Reconnecting;

	/// <summary>
	/// Event fired when connection is reconnected
	/// </summary>
	public event Func<string?, Task>? Reconnected;

	#endregion

	#region Command Events

	/// <summary>
	/// Event fired when a command is received from the server
	/// </summary>
	public event Func<CommandMessage, Task>? CommandReceived;

	#endregion

	#region Server Request Events

	/// <summary>
	/// Event fired when a logs request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task<LogsResponse>>? LogsRequested;

	/// <summary>
	/// Event fired when a logs stream request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task<ActionResult>>? LogsStreamRequested;

	/// <summary>
	/// Event fired when a session request is received from the server
	/// </summary>
	public event Func<GetSingleSessionRequest, Task<SessionResponse>>? SessionRequested;

	/// <summary>
	/// Event fired when a session messages stream request is received from the server
	/// </summary>
	public event Func<GetSingleSessionRequest, Task<ActionResult>>? SessionMessagesStreamRequested;

	/// <summary>
	/// Event fired when a sessions request is received from the server
	/// </summary>
	public event Func<GetSessionsRequest, Task<SessionsResponse>>? SessionsRequested;

	/// <summary>
	/// Event fired when a config request is received from the server
	/// </summary>
	public event Func<GetConfigRequest, Task<ConfigResponse>>? ConfigRequested;

	/// <summary>
	/// Event fired when a config update request is received from the server
	/// </summary>
	public event Func<UpdateConfigRequest, Task<ConfigResponse>>? ConfigUpdateRequested;

	/// <summary>
	/// Event fired when a custom themes request is received from the server
	/// </summary>
	public event Func<GetCustomThemesRequest, Task<ThemesResponse>>? CustomThemesRequested;

	/// <summary>
	/// Event fired when a commands metadata request is received from the server
	/// </summary>
	public event Func<GetCommandsMetadataRequest, Task<CommandsMetadataResponse>>? CommandsMetadataRequested;

	/// <summary>
	/// Event fired when a remove session request is received from the server
	/// </summary>
	public event Func<RemoveSessionRequest, Task<ActionResult>>? RemoveSessionRequested;

	/// <summary>
	/// Event fired when an update session request is received from the server
	/// </summary>
	public event Func<UpdateSessionRequest, Task<ActionResult>>? UpdateSessionRequested;

	/// <summary>
	/// Event fired when a machine info request is received from the server
	/// </summary>
	public event Func<MachineInfoRequest, Task<MachineInfoResponse>>? MachineInfoRequested;

	#endregion

	/// <summary>
	/// Creates a new instance of JiroClientBase
	/// </summary>
	/// <param name="hubConnection">The SignalR hub connection to use</param>
	/// <param name="logger">Optional logger instance</param>
	protected JiroClientBase(HubConnection? hubConnection = null, ILogger<JiroClientBase>? logger = null)
	{
		_hubConnection = hubConnection!;
		_logger = logger;
		if (_hubConnection != null)
			SetupEvents();
	}

	#region SetupEvents
	/// <summary>
	/// Sets up the events for the WebSocket connection.
	/// This method is called after the connection is established to ensure all events are ready to be invoked.
	/// It should be called after the connection is started to ensure the events are properly registered
	/// and can be invoked when the corresponding events occur.
	/// </summary>
	public virtual void SetupEvents()
	{
		if (_hubConnection == null)
			return;

		// Connection lifecycle events
		_hubConnection.Closed += async (error) =>
		{
			if (Closed != null)
				await Closed(error);
		};

		_hubConnection.Reconnecting += async (error) =>
		{
			if (Reconnecting != null)
				await Reconnecting(error);
		};

		_hubConnection.Reconnected += async (connectionId) =>
		{
			if (Reconnected != null)
				await Reconnected(connectionId);
		};

		// Fire-and-forget notifications
		_hubConnection.OnNotification<CommandMessage>(Events.CommandReceived, async command =>
		{
			if (CommandReceived != null)
				await CommandReceived(command);
		}, _logger);

		// RPC-style calls (server expects return value)
		_hubConnection.OnRequest<GetLogsRequest, LogsResponse>(
			Events.LogsRequested,
			async request => await LogsRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetLogsRequest, ActionResult>(Events.LogsStreamRequested,
			async request => await LogsStreamRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetSessionsRequest, SessionsResponse>(
			Events.SessionsRequested,
			async request => await SessionsRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetSingleSessionRequest, SessionResponse>(
			Events.SingleSessionRequested,
			async request => await SessionRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetSingleSessionRequest, ActionResult>(Events.SessionMessagesStreamRequested,
			async request => await SessionMessagesStreamRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetConfigRequest, ConfigResponse>(
			Events.ConfigRequested,
			async request => await ConfigRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<UpdateConfigRequest, ConfigResponse>(
			Events.ConfigUpdated,
			async request => await ConfigUpdateRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetCustomThemesRequest, ThemesResponse>(
			Events.CustomThemesRequested,
			async request => await CustomThemesRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<GetCommandsMetadataRequest, CommandsMetadataResponse>(
			Events.CommandsMetadataRequested,
			async request => await CommandsMetadataRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<RemoveSessionRequest, ActionResult>(
			Events.RemoveSessionRequested,
			async request => await RemoveSessionRequested!(request),
			_logger);

		_hubConnection.OnRequest<UpdateSessionRequest, ActionResult>(
			Events.UpdateSessionRequested,
			async request => await UpdateSessionRequested!(request),
			_logger,
			request => request.RequestId);

		_hubConnection.OnRequest<MachineInfoRequest, MachineInfoResponse>(
			Events.MachineInfoRequested,
			async request => await MachineInfoRequested!(request),
			_logger,
			request => request.RequestId);
	}
	#endregion

	/// <summary>
	/// Initializes the WebSocket connection with proper setup and error handling
	/// </summary>
	/// <param name="hubUrl">The URL of the hub to connect to</param>
	/// <param name="apiKey">The API key for authentication</param>
	/// <param name="exceptionHandler">Optional exception handler for connection errors</param>
	/// <param name="cancellationToken">Cancellation token for the operation</param>
	public virtual async Task InitializeAsync(string? hubUrl = null, string? apiKey = null, Action<Exception, string>? exceptionHandler = null, CancellationToken cancellationToken = default)
	{
		_logger?.LogInformation("Initializing WebSocket connection to {Url}", hubUrl);

		if (_hubConnection == null)
		{
			throw new InvalidOperationException("HubConnection is not initialized. Ensure the connection is properly configured.");
		}

		await _connectionSemaphore.WaitAsync(cancellationToken);
		try
		{
			if (_hubConnection is null)
			{
				throw new InvalidOperationException("HubConnection is not initialized. Ensure the connection is properly configured.");
			}

			if (_hubConnection.State == HubConnectionState.Connected)
			{
				_logger?.LogInformation("Already connected to hub at {Url}", hubUrl);
				_logger?.LogInformation("Stopping existing connection before re-initializing");
				await _hubConnection.StopAsync(cancellationToken);
			}

			_logger?.LogInformation("Connecting to hub at {Url}", hubUrl);

			// Ensure API key is provided for authentication
			if (string.IsNullOrEmpty(apiKey))
			{
				throw new InvalidOperationException("API key is required for WebSocket authentication. Please configure 'WebSocket:ApiKey' or 'API_KEY' in your settings.");
			}

			// Connect
			SetupHandlers();

			await _hubConnection.StartAsync(cancellationToken);

			_logger?.LogInformation("Successfully connected to hub");
		}
		catch (Exception ex)
		{
			exceptionHandler?.Invoke(ex, "Connect");
			throw;
		}
		finally
		{
			_connectionSemaphore.Release();
		}
	}

	/// <summary>
	/// Override this method to implement custom event handlers for the client.
	/// This method is called after the WebSocket connection is established to set up
	/// application-specific event handling logic.
	/// </summary>
	protected abstract void SetupHandlers();

	/// <summary>
	/// Override this method to implement custom cleanup logic.
	/// This method is called when the client is being disposed or disconnected
	/// to perform any necessary cleanup operations.
	/// </summary>
	/// <returns>A task representing the asynchronous cleanup operation</returns>
	protected virtual Task CleanupAsync()
	{
		return Task.CompletedTask;
	}

	/// <summary>
	/// Sends logs stream to the server
	/// </summary>
	/// <param name="requestId">The unique identifier for this stream request</param>
	/// <param name="stream">The channel reader containing log entries to send</param>
	public virtual async Task ReceiveLogsStreamAsync(string requestId, ChannelReader<LogEntry> stream)
	{
		if (_hubConnection?.State == HubConnectionState.Connected)
		{
			await _hubConnection.InvokeAsync(Events.ReceiveLogsStream, requestId, stream);
		}
	}

	/// <summary>
	/// Sends session messages stream to the server
	/// </summary>
	/// <param name="requestId">The unique identifier for this stream request</param>
	/// <param name="stream">The channel reader containing chat messages to send</param>
	public virtual async Task ReceiveSessionMessagesStreamAsync(string requestId, ChannelReader<ChatMessage> stream)
	{
		if (_hubConnection?.State == HubConnectionState.Connected)
		{
			await _hubConnection.InvokeAsync(Events.ReceiveSessionMessagesStream, requestId, stream);
		}
	}
}
