using System.Text.Json;

using Xunit;

namespace Jiro.Shared.Tests.Serialization;

/// <summary>
/// Tests for JSON serialization of SessionCommandResponse.
/// Note: ICommandResult types are now handled by consuming applications (Jiro.Commands).
/// </summary>
public class SessionCommandResponseSerializationTests
{
	private readonly JsonSerializerOptions _options = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true
	};

	[Fact]
	public void SessionCommandResponse_WithStringResult_SerializesCorrectly()
	{
		// Arrange
		var response = new Websocket.Responses.SessionCommandResponse
		{
			CommandName = "test-command",
			CommandType = "text",
			IsSuccess = true,
			Result = "Simple text result"
		};

		// Act
		var json = JsonSerializer.Serialize(response, _options);
		var deserialized = JsonSerializer.Deserialize<Websocket.Responses.SessionCommandResponse>(json, _options);

		// Assert
		Assert.NotNull(deserialized);
		Assert.Equal("test-command", deserialized.CommandName);
		Assert.Equal("text", deserialized.CommandType);
		Assert.True(deserialized.IsSuccess);
		// Result is deserialized as JsonElement for object types
		var resultElement = (JsonElement)deserialized.Result;
		Assert.Equal("Simple text result", resultElement.GetString());
	}

	[Fact]
	public void SessionCommandResponse_WithObjectResult_SerializesCorrectly()
	{
		// Arrange
		var complexResult = new { message = "Success", data = new[] { 1, 2, 3 } };
		var response = new Websocket.Responses.SessionCommandResponse
		{
			CommandName = "complex-command",
			CommandType = "graph",
			IsSuccess = true,
			Result = complexResult
		};

		// Act
		var json = JsonSerializer.Serialize(response, _options);
		var deserialized = JsonSerializer.Deserialize<Websocket.Responses.SessionCommandResponse>(json, _options);

		// Assert
		Assert.NotNull(deserialized);
		Assert.Equal("complex-command", deserialized.CommandName);
		Assert.Equal("graph", deserialized.CommandType);
		Assert.True(deserialized.IsSuccess);
		Assert.NotNull(deserialized.Result);
		
		// Result is deserialized as JsonElement, verify structure
		var resultElement = (JsonElement)deserialized.Result;
		Assert.True(resultElement.TryGetProperty("message", out var messageProperty));
		Assert.Equal("Success", messageProperty.GetString());
	}

	[Fact]
	public void SessionCommandResponse_WithNullResult_SerializesCorrectly()
	{
		// Arrange
		var response = new Websocket.Responses.SessionCommandResponse
		{
			CommandName = "null-result-command",
			CommandType = "error",
			IsSuccess = false,
			Result = null
		};

		// Act
		var json = JsonSerializer.Serialize(response, _options);
		var deserialized = JsonSerializer.Deserialize<Websocket.Responses.SessionCommandResponse>(json, _options);

		// Assert
		Assert.NotNull(deserialized);
		Assert.Equal("null-result-command", deserialized.CommandName);
		Assert.Equal("error", deserialized.CommandType);
		Assert.False(deserialized.IsSuccess);
		Assert.Null(deserialized.Result);
	}

	[Fact]
	public void SessionCommandResponse_WithEmptyCommandType_SerializesCorrectly()
	{
		// Arrange
		var response = new Websocket.Responses.SessionCommandResponse
		{
			CommandName = "test-command",
			CommandType = "",
			IsSuccess = true,
			Result = "Result data"
		};

		// Act
		var json = JsonSerializer.Serialize(response, _options);
		var deserialized = JsonSerializer.Deserialize<Websocket.Responses.SessionCommandResponse>(json, _options);

		// Assert
		Assert.NotNull(deserialized);
		Assert.Equal("test-command", deserialized.CommandName);
		Assert.Equal("", deserialized.CommandType);
		Assert.True(deserialized.IsSuccess);
		// Result is deserialized as JsonElement for object types  
		var resultElement = (JsonElement)deserialized.Result;
		Assert.Equal("Result data", resultElement.GetString());
	}
}