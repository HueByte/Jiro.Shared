namespace Jiro.Shared.Utilities;

/// <summary>
/// Provides utility methods for application-level operations.
/// </summary>
public class ApplicationUtilities
{
	/// <summary>
	/// Checks if the application is running in debug mode.
	/// </summary>
	/// <returns></returns>
	public static bool IsDebug()
	{
#if DEBUG
		return true;
#else
        return false;
#endif
	}
}
