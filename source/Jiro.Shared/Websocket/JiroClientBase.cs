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

	/// <summary>
	/// Event fired when a keepalive acknowledgment is received
	/// </summary>
	public event Func<Task>? KeepaliveAckReceived;

	#endregion

	#region Server Request Events

	/// <summary>
	/// Event fired when a logs request is received from the server
	/// </summary>
	public event Func<GetLogsRequest, Task<LogsResponse>>? LogsRequested;

	/// <summary>
	/// Event fired when a session request is received from the server
	/// </summary>
	public event Func<GetSessionRequest, Task<SessionResponse>>? SessionRequested;

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
		_hubConnection.On<CommandMessage>(Events.CommandReceived, async command =>
		{
			_logger?.LogInformation("{EventName} received", Events.CommandReceived);
			if (CommandReceived != null)
				await CommandReceived(command);
			_logger?.LogInformation("{EventName} executed", Events.CommandReceived);
		});

		_hubConnection.On(Events.KeepaliveAckReceived, async () =>
		{
			_logger?.LogInformation("{EventName} received", Events.KeepaliveAckReceived);
			if (KeepaliveAckReceived != null)
				await KeepaliveAckReceived();
			_logger?.LogInformation("{EventName} executed", Events.KeepaliveAckReceived);
		});

		// RPC-style calls (server expects return value)

		_hubConnection.On<GetLogsRequest, LogsResponse>(
			Events.LogsRequested,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.LogsRequested, request.RequestId);
				var response = await LogsRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.LogsRequested, response.RequestId);
				return response;
			});

		_hubConnection.On<GetSessionsRequest, SessionsResponse>(
			Events.SessionsRequested,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.SessionsRequested, request.RequestId);
				var response = await SessionsRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.SessionsRequested, response.RequestId);
				return response;
			});

		_hubConnection.On<GetSessionRequest, SessionResponse>(
			Events.SessionRequested,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.SessionRequested, request.RequestId);
				var response = await SessionRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.SessionRequested, response.RequestId);
				return response;
			});

		_hubConnection.On<GetConfigRequest, ConfigResponse>(
			Events.ConfigRequested,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.ConfigRequested, request.RequestId);
				var response = await ConfigRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.ConfigRequested, response.RequestId);
				return response;
			});

		_hubConnection.On<UpdateConfigRequest, ConfigResponse>(
			Events.ConfigUpdated,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.ConfigUpdated, request.RequestId);
				var response = await ConfigUpdateRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.ConfigUpdated, response.RequestId);
				return response;
			});

		_hubConnection.On<GetCustomThemesRequest, ThemesResponse>(
			Events.CustomThemesRequested,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.CustomThemesRequested, request.RequestId);
				var response = await CustomThemesRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.CustomThemesRequested, response.RequestId);
				return response;
			});

		_hubConnection.On<GetCommandsMetadataRequest, CommandsMetadataResponse>(
			Events.CommandsMetadataRequested,
			async request =>
			{
				_logger?.LogInformation("{EventName} received: {RequestId}", Events.CommandsMetadataRequested, request.RequestId);
				var response = await CommandsMetadataRequested!(request);
				_logger?.LogInformation("{EventName} handled: {RequestId}", Events.CommandsMetadataRequested, response.RequestId);
				return response;
			});
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
			await _hubConnection.StartAsync(cancellationToken);

			SetupHandlers();

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
	/// Override this method to implement custom event handlers for the client
	/// </summary>
	protected abstract void SetupHandlers();

	/// <summary>
	/// Override this method to implement custom cleanup logic
	/// </summary>
	protected virtual Task CleanupAsync()
	{
		return Task.CompletedTask;
	}
}
