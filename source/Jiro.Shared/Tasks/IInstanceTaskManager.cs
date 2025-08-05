namespace Jiro.Shared.Tasks;

/// <summary>
/// Provides methods for managing tasks associated with specific instances.
/// </summary>
public interface IInstanceTaskManager : ITaskManager
{
	/// <summary>
	/// Gets the instance ID associated with the specified request ID.
	/// </summary>
	/// <param name="requestId">The request ID to look up.</param>
	/// <returns>The instance ID associated with the request ID.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the request ID does not have an associated instance ID.</exception>
	string GetRequestInstanceId(string requestId);
}
