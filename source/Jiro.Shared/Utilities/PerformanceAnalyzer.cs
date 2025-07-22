using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Jiro.Shared.Utilities;

/// <summary>
/// Defines a contract for system performance monitoring operations.
/// </summary>
public interface IPerformanceAnalyzer
{
	/// <summary>
	/// Gets the available system memory in megabytes.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the available memory in megabytes.</returns>
	Task<float> GetAvailableMemoryMBAsync();

	/// <summary>
	/// Gets the current CPU usage percentage.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the CPU usage as a percentage (0-100).</returns>
	Task<float> GetCpuUsageAsync();

	/// <summary>
	/// Gets the amount of memory currently used by the system in megabytes.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the used memory in megabytes.</returns>
	Task<float> GetApplicationMemoryUsedMBAsync();

	/// <summary>
	/// Gets the system memory usage as a percentage of total available memory.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the memory usage as a percentage (0-100).</returns>
	Task<float> GetApplicationMemoryUsagePercentageAsync();
}

/// <summary>
/// Linux-specific implementation of performance monitoring using /proc filesystem.
/// </summary>
public class PerformanceAnalyzerLinux : IPerformanceAnalyzer
{
	private ulong _prevIdleTime;
	private ulong _prevTotalTime;

	/// <summary>
	/// Gets the current CPU usage percentage by reading from /proc/stat.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the CPU usage as a percentage (0-100).</returns>
	public async Task<float> GetCpuUsageAsync()
	{
		var cpuStats = await File.ReadAllLinesAsync("/proc/stat");
		var cpuLine = cpuStats[0]; // "cpu  3357 0 4313 1362393 ..."
		var parts = cpuLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (parts[0] != "cpu")
			throw new InvalidOperationException("Unexpected format in /proc/stat");

		// Parse the fields (user, nice, system, idle, iowait, irq, softirq, steal, guest, guest_nice)
		var user = ulong.Parse(parts[1]);
		var nice = ulong.Parse(parts[2]);
		var system = ulong.Parse(parts[3]);
		var idle = ulong.Parse(parts[4]);
		var iowait = ulong.Parse(parts[5]);
		var irq = ulong.Parse(parts[6]);
		var softirq = ulong.Parse(parts[7]);
		var steal = ulong.Parse(parts[8]);

		var idleAllTime = idle + iowait;
		var totalTime = user + nice + system + idle + iowait + irq + softirq + steal;

		var diffIdle = idleAllTime - _prevIdleTime;
		var diffTotal = totalTime - _prevTotalTime;

		_prevIdleTime = idleAllTime;
		_prevTotalTime = totalTime;

		if (diffTotal == 0) return 0;
		return 100f * (1.0f - ((float)diffIdle / diffTotal));
	}

	/// <summary>
	/// Gets the available system memory in megabytes by reading from /proc/meminfo.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the available memory in megabytes.</returns>
	public async Task<float> GetAvailableMemoryMBAsync()
	{
		var memInfo = await File.ReadAllLinesAsync("/proc/meminfo");
		foreach (var line in memInfo)
		{
			if (line.StartsWith("MemAvailable:"))
			{
				var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 2 && int.TryParse(parts[1], out var kb))
				{
					return kb / 1024f;
				}
			}
		}

		throw new InvalidOperationException("MemAvailable not found in /proc/meminfo");
	}

	/// <summary>
	/// Gets the amount of memory currently used by the system in megabytes by reading from /proc/meminfo.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the used memory in megabytes.</returns>
	public async Task<float> GetApplicationMemoryUsedMBAsync()
	{
		var memInfo = await File.ReadAllLinesAsync("/proc/meminfo");
		float totalMemory = 0;
		float availableMemory = 0;

		foreach (var line in memInfo)
		{
			if (line.StartsWith("MemTotal:"))
			{
				var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 2 && int.TryParse(parts[1], out var kb))
				{
					totalMemory = kb / 1024f;
				}
			}
			else if (line.StartsWith("MemAvailable:"))
			{
				var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 2 && int.TryParse(parts[1], out var kb))
				{
					availableMemory = kb / 1024f;
				}
			}
		}

		if (totalMemory == 0 || availableMemory == 0)
		{
			throw new InvalidOperationException("Unable to determine memory usage from /proc/meminfo");
		}

		return totalMemory - availableMemory;
	}

	/// <summary>
	/// Gets the system memory usage as a percentage of total available memory by reading from /proc/meminfo.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the memory usage as a percentage (0-100).</returns>
	public async Task<float> GetApplicationMemoryUsagePercentageAsync()
	{
		var usedMemory = await GetApplicationMemoryUsedMBAsync();
		var memInfo = await File.ReadAllLinesAsync("/proc/meminfo");
		float totalMemory = 0;

		foreach (var line in memInfo)
		{
			if (line.StartsWith("MemTotal:"))
			{
				var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 2 && int.TryParse(parts[1], out var kb))
				{
					totalMemory = kb / 1024f;
				}
			}
		}

		if (totalMemory == 0)
		{
			throw new InvalidOperationException("Unable to determine total memory from /proc/meminfo");
		}

		return usedMemory / totalMemory * 100f;
	}
}

/// <summary>
/// Windows-specific implementation of performance monitoring using Windows Performance Counters.
/// </summary>
public class PerformanceAnalyzerWindows : IPerformanceAnalyzer
{
	private readonly PerformanceCounter _cpuCounter;
	private readonly PerformanceCounter _memoryCounter;

	/// <summary>
	/// Initializes a new instance of the <see cref="PerformanceAnalyzerWindows"/> class.
	/// </summary>
	/// <exception cref="PlatformNotSupportedException">Thrown when attempting to use this class on a non-Windows platform.</exception>
	public PerformanceAnalyzerWindows()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			throw new PlatformNotSupportedException("PerformanceAnalyzerWindows is not supported on Linux.");
		}

		_cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
		_memoryCounter = new PerformanceCounter("Memory", "Available MBytes");
	}

	/// <summary>
	/// Gets the current CPU usage percentage using Windows Performance Counters.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the CPU usage as a percentage (0-100).</returns>
	public Task<float> GetCpuUsageAsync()
	{
		return Task.FromResult(_cpuCounter.NextValue());
	}

	/// <summary>
	/// Gets the available system memory in megabytes using Windows Performance Counters.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the available memory in megabytes.</returns>
	public Task<float> GetAvailableMemoryMBAsync()
	{
		return Task.FromResult(_memoryCounter.NextValue());
	}

	/// <summary>
	/// Gets the amount of memory currently used by the system in megabytes using Windows Performance Counters.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the used memory in megabytes.</returns>
	/// <exception cref="InvalidOperationException">Thrown when unable to determine memory usage on Windows.</exception>
	public Task<float> GetApplicationMemoryUsedMBAsync()
	{
		var totalMemory = new PerformanceCounter("Memory", "Committed Bytes").NextValue() / (1024 * 1024);
		var availableMemory = _memoryCounter.NextValue();
		var usedMemory = totalMemory - availableMemory;

		if (totalMemory == 0)
		{
			throw new InvalidOperationException("Unable to determine memory usage on Windows.");
		}

		return Task.FromResult(usedMemory);
	}

	/// <summary>
	/// Gets the system memory usage as a percentage of total available memory using Windows Performance Counters.
	/// </summary>
	/// <returns>A task representing the asynchronous operation. The task result contains the memory usage as a percentage (0-100).</returns>
	/// <exception cref="InvalidOperationException">Thrown when unable to determine total memory on Windows.</exception>
	public async Task<float> GetApplicationMemoryUsagePercentageAsync()
	{
		var usedMemory = await GetApplicationMemoryUsedMBAsync();
		var totalMemory = new PerformanceCounter("Memory", "Committed Bytes").NextValue() / (1024 * 1024);

		if (totalMemory == 0)
		{
			throw new InvalidOperationException("Unable to determine total memory on Windows.");
		}

		return usedMemory / totalMemory * 100f;
	}
}

/// <summary>
/// Factory class for creating platform-specific performance analyzer instances.
/// </summary>
public static class PerformanceAnalyzerFactory
{
	/// <summary>
	/// Creates a platform-specific performance analyzer instance.
	/// </summary>
	/// <returns>An <see cref="IPerformanceAnalyzer"/> implementation appropriate for the current operating system.</returns>
	/// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported (only Windows and Linux are supported).</exception>
	public static IPerformanceAnalyzer Create()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return new PerformanceAnalyzerWindows();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return new PerformanceAnalyzerLinux();
		}
		else
		{
			throw new PlatformNotSupportedException("PerformanceAnalyzer is only supported on Windows and Linux.");
		}
	}
}
