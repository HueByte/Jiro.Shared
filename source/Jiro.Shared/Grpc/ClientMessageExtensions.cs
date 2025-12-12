using Jiro.Shared.Websocket.Responses;

namespace Jiro.Shared.Grpc;

/// <summary>
/// Extension methods for converting gRPC ClientMessage to response objects
/// </summary>
public static class ClientMessageExtensions
{
	/// <summary>
	/// Converts a gRPC ClientMessage to the appropriate response type
	/// </summary>
	/// <param name="clientMessage">The gRPC ClientMessage to convert</param>
	/// <returns>A response object that can be used with the TaskManager</returns>
	public static SessionCommandResponse ToTrackedObject(this ClientMessage clientMessage)
	{
		if (clientMessage == null)
			throw new ArgumentNullException(nameof(clientMessage));

		return clientMessage.DataType switch
		{
			DataType.DataText => ConvertToTextResponse(clientMessage),
			DataType.DataGraph => ConvertToGraphResponse(clientMessage),
			_ => throw new ArgumentException($"Unsupported data type: {clientMessage.DataType}", nameof(clientMessage))
		};
	}

	/// <summary>
	/// Converts a gRPC ClientMessage to the appropriate response type with instance ID
	/// </summary>
	/// <param name="clientMessage">The gRPC ClientMessage to convert</param>
	/// <param name="instanceId">The instance ID to include in the synchronization token</param>
	/// <returns>A response object that can be used with the TaskManager</returns>
	public static SessionCommandResponse ToTrackedObject(this ClientMessage clientMessage, string instanceId)
	{
		var response = clientMessage.ToTrackedObject();
		response.SynchronizationToken.InstanceId = instanceId;

		return response;
	}

	private static SessionCommandResponse ConvertToTextResponse(ClientMessage clientMessage, string instanceId = "")
	{
		var textResult = clientMessage.TextResult;
		if (textResult == null)
			throw new ArgumentException("TextResult is null for text data type", nameof(clientMessage));

		// Create response object with generic result - consuming applications will handle typed conversion
		return new SessionCommandResponse
		{
			RequestId = clientMessage.RequestId,
			SynchronizationToken = new SynchronizationToken
			{
				RequestId = clientMessage.RequestId,
				SessionId = clientMessage.SessionId,
				InstanceId = instanceId
			},
			CommandName = clientMessage.CommandName,
			IsSuccess = clientMessage.IsSuccess,
			CommandType = "text",
			Result = new
			{
				Response = textResult.Response,
				TextType = textResult.TextType.ToString().ToLower()
			}
		};
	}

	private static SessionCommandResponse ConvertToGraphResponse(ClientMessage clientMessage, string instanceId = "")
	{
		var graphResult = clientMessage.GraphResult;
		if (graphResult == null)
			throw new ArgumentException("GraphResult is null for graph data type", nameof(clientMessage));

		// Create response object with generic result - consuming applications will handle typed conversion
		return new SessionCommandResponse
		{
			RequestId = clientMessage.RequestId,
			SynchronizationToken = new SynchronizationToken
			{
				RequestId = clientMessage.RequestId,
				SessionId = clientMessage.SessionId,
				InstanceId = instanceId
			},
			CommandName = clientMessage.CommandName,
			IsSuccess = clientMessage.IsSuccess,
			CommandType = "graph",
			Result = new
			{
				Message = graphResult.Message,
				Data = graphResult.GraphData?.ToByteArray() != null
					? System.Text.Encoding.UTF8.GetString(graphResult.GraphData.ToByteArray())
					: null,
				Units = graphResult.Units?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>(),
				XAxis = graphResult.XAxis,
				YAxis = graphResult.YAxis,
				Note = graphResult.Note
			}
		};
	}

}
