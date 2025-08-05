namespace Jiro.Shared.Tasks.Models;

public readonly struct InstanceManagedTask
{
	public readonly string InstanceId;
	public readonly TaskCompletionSource<TrackedObject> TaskCompletionSource;
	public readonly DateTime CreatedAt;

	public InstanceManagedTask(string instanceId, TaskCompletionSource<TrackedObject> taskCompletionSource)
	{
		InstanceId = instanceId;
		TaskCompletionSource = taskCompletionSource;
		CreatedAt = DateTime.UtcNow;
	}
}
