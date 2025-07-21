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
	protected JiroClientBase(HubConnection hubConnection, ILogger<JiroClientBase>? logger = null)
	{
		_hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
		_logger = logger;
		SetupEvents();
	}

	/// <summary>
	/// Sets up the events for the client connection
	/// </summary>
	public virtual void SetupEvents()
	{
		// Connection lifecycle events
		_hubConnection.Closed += async (error) =>
		{
			if (Closed != null)
				await Closed.Invoke(error);
		};

		_hubConnection.Reconnecting += async (error) =>
		{
			if (Reconnecting != null)
				await Reconnecting.Invoke(error);
		};

		_hubConnection.Reconnected += async (connectionId) =>
		{
			if (Reconnected != null)
				await Reconnected.Invoke(connectionId);
		};

		// Command events
		_hubConnection.On<CommandMessage>(Events.CommandReceived, async (command) =>
		{
			_logger?.LogInformation("{EventName} received", Events.CommandReceived);
			if (CommandReceived != null)
				await CommandReceived.Invoke(command);
			_logger?.LogInformation("{EventName} executed", Events.CommandReceived);
		});

		_hubConnection.On(Events.KeepaliveAckReceived, async () =>
		{
			_logger?.LogInformation("{EventName} received", Events.KeepaliveAckReceived);
			if (KeepaliveAckReceived != null)
				await KeepaliveAckReceived.Invoke();
			_logger?.LogInformation("{EventName} executed", Events.KeepaliveAckReceived);
		});

		// Server request events with responses
		_hubConnection.On<GetLogsRequest, Task<LogsResponse>>(Events.LogsRequested, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.LogsRequested);
			if (LogsRequested != null)
			{
				var result = await LogsRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.LogsRequested);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.LogsRequested} not implemented");
		});

		_hubConnection.On<GetSessionRequest, Task<SessionResponse>>(Events.SessionRequested, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.SessionRequested);
			if (SessionRequested != null)
			{
				var result = await SessionRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.SessionRequested);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.SessionRequested} not implemented");
		});

		_hubConnection.On<GetSessionsRequest, Task<SessionsResponse>>(Events.SessionsRequested, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.SessionsRequested);
			if (SessionsRequested != null)
			{
				var result = await SessionsRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.SessionsRequested);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.SessionsRequested} not implemented");
		});

		_hubConnection.On<GetConfigRequest, Task<ConfigResponse>>(Events.ConfigRequested, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.ConfigRequested);
			if (ConfigRequested != null)
			{
				var result = await ConfigRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.ConfigRequested);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.ConfigRequested} not implemented");
		});

		_hubConnection.On<UpdateConfigRequest, Task<ConfigResponse>>(Events.ConfigUpdated, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.ConfigUpdated);
			if (ConfigUpdateRequested != null)
			{
				var result = await ConfigUpdateRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.ConfigUpdated);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.ConfigUpdated} not implemented");
		});

		_hubConnection.On<GetCustomThemesRequest, Task<ThemesResponse>>(Events.CustomThemesRequested, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.CustomThemesRequested);
			if (CustomThemesRequested != null)
			{
				var result = await CustomThemesRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.CustomThemesRequested);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.CustomThemesRequested} not implemented");
		});

		_hubConnection.On<GetCommandsMetadataRequest, Task<CommandsMetadataResponse>>(Events.CommandsMetadataRequested, async (request) =>
		{
			_logger?.LogInformation("{EventName} received", Events.CommandsMetadataRequested);
			if (CommandsMetadataRequested != null)
			{
				var result = await CommandsMetadataRequested.Invoke(request);
				_logger?.LogInformation("{EventName} executed", Events.CommandsMetadataRequested);
				return result;
			}

			throw new NotImplementedException($"Handler for {Events.CommandsMetadataRequested} not implemented");
		});
	}

	/// <summary>
	/// Override this method to implement custom initialization logic
	/// </summary>
	protected virtual Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	/// <summary>
	/// Override this method to implement custom cleanup logic
	/// </summary>
	protected virtual Task CleanupAsync()
	{
		return Task.CompletedTask;
	}
}
