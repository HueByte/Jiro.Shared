using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a request to remove a session.
/// </summary>
public class RemoveSessionRequest : TrackedObject
{
	/// <summary>
	/// Gets or sets the unique identifier for the session to be removed.
	/// </summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the reason for removing the session.
	/// </summary>
	public string? Reason { get; set; }
}
