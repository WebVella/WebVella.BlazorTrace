using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Utility;
public static class WvTraceUtility
{
	public static long? GetDurationMS(this WvTraceSessionTrace method)
	{
		if (method.EnteredOn is null || method.ExitedOn is null)
			return null;

		if (method.EnteredOn > method.ExitedOn)
			return null;
		return (method.ExitedOn.Value - method.EnteredOn.Value).Milliseconds;
	}
	public static long? GetMinDuration(this WvTraceSessionMethod method)
	{
		long minDuration = long.MaxValue;
		foreach (var trace in method.TraceList)
		{
			if (trace.DurationMs is null || trace.DurationMs.Value < 0) continue;
			if (trace.DurationMs < minDuration)
				minDuration = trace.DurationMs.Value;
		}
		return minDuration == long.MaxValue ? null : minDuration;
	}
	public static long? GetMaxDuration(this WvTraceSessionMethod method)
	{
		long maxDuration = -1;
		foreach (var trace in method.TraceList)
		{
			if (trace.DurationMs is null || trace.DurationMs.Value < 0) continue;
			if (trace.DurationMs > maxDuration)
				maxDuration = trace.DurationMs.Value;
		}
		return maxDuration == -1 ? null : maxDuration;
	}
	public static long? GetAverageDuration(this WvTraceSessionMethod method)
	{
		long durationSum = -1;
		decimal count = 1;
		foreach (var trace in method.TraceList)
		{
			if (trace.DurationMs is null || trace.DurationMs.Value < 0) continue;
			durationSum += trace.DurationMs.Value;
			count++;
		}
		if (durationSum == -1) return null;
		return (long)(durationSum / count);
	}
	public static long? GetMinMemory(this WvTraceSessionMethod method, bool isOnEnter)
	{
		long minMemory = long.MaxValue;
		foreach (var trace in method.TraceList)
		{
			if (isOnEnter)
			{
				if (trace.OnEnterMemoryBytes is null || trace.OnEnterMemoryBytes < 0) continue;
				if (trace.OnEnterMemoryBytes < minMemory)
					minMemory = trace.OnEnterMemoryBytes.Value;
			}
			else
			{
				if (trace.OnExitMemoryBytes is null || trace.OnExitMemoryBytes < 0) continue;
				if (trace.OnExitMemoryBytes < minMemory)
					minMemory = trace.OnExitMemoryBytes.Value;
			}
		}
		return minMemory == long.MaxValue ? null : minMemory;
	}
	public static long? GetMaxMemory(this WvTraceSessionMethod method, bool isOnEnter)
	{
		long maxMemory = -1;
		foreach (var trace in method.TraceList)
		{
			if (isOnEnter)
			{
				if (trace.OnEnterMemoryBytes is null || trace.OnEnterMemoryBytes < 0) continue;
				if (trace.OnEnterMemoryBytes > maxMemory)
					maxMemory = trace.OnEnterMemoryBytes.Value;
			}
			else
			{
				if (trace.OnExitMemoryBytes is null || trace.OnExitMemoryBytes < 0) continue;
				if (trace.OnExitMemoryBytes > maxMemory)
					maxMemory = trace.OnExitMemoryBytes.Value;
			}
		}
		return maxMemory == -1 ? null : maxMemory;
	}
	public static long? GetMinMemoryDelta(this WvTraceSessionMethod method)
	{
		long minMemoryDelta = long.MaxValue;
		foreach (var trace in method.TraceList)
		{
			if (trace.OnEnterMemoryBytes is null || trace.OnEnterMemoryBytes < 0) continue;
			if (trace.OnExitMemoryBytes is null || trace.OnExitMemoryBytes < 0) continue;

			var delta = trace.OnExitMemoryBytes.Value - trace.OnEnterMemoryBytes.Value;

			if (delta < minMemoryDelta)
				minMemoryDelta = delta;
		}
		return minMemoryDelta == long.MaxValue ? null : minMemoryDelta;
	}
	public static long? GetMaxMemoryDelta(this WvTraceSessionMethod method)
	{
		long maxMemoryDelta = -1;
		foreach (var trace in method.TraceList)
		{
			if (trace.OnEnterMemoryBytes is null || trace.OnEnterMemoryBytes < 0) continue;
			if (trace.OnExitMemoryBytes is null || trace.OnExitMemoryBytes < 0) continue;

			var delta = trace.OnExitMemoryBytes.Value - trace.OnEnterMemoryBytes.Value;
			if (delta > maxMemoryDelta)
				maxMemoryDelta = delta;
		}
		return maxMemoryDelta == -1 ? null : maxMemoryDelta;
	}
	public static long? GetAverageMemory(this WvTraceSessionMethod method)
	{
		long memorySum = -1;
		decimal count = 1;
		foreach (var trace in method.TraceList)
		{
			if (trace.OnEnterMemoryBytes is null && trace.OnExitMemoryBytes is null) continue;

			var traceMax = trace.OnEnterMemoryBytes ?? 0;
			if (traceMax < (trace.OnExitMemoryBytes ?? 0))
				traceMax = (trace.OnExitMemoryBytes ?? 0);
			memorySum += traceMax;
			count++;
		}
		if (memorySum == -1) return null;
		return (long)(memorySum / count);
	}
	public static long GetOnEnterCallCount(this WvTraceSessionMethod method)
	{
		return method.TraceList.Count(x => x.EnteredOn is not null);
	}
	public static long GetOnExitCallsCount(this WvTraceSessionMethod method)
	{
		return method.TraceList.Count(x => x.ExitedOn is not null);
	}
	public static long GetMaxCallsCount(this WvTraceSessionMethod method)
	{
		if (method.OnEnterCallsCount < method.OnExitCallsCount)
			return method.OnExitCallsCount;

		return method.OnEnterCallsCount;
	}
	public static long CompletedCallsCount(this WvTraceSessionMethod method)
	{
		return method.TraceList.Count(x => x.EnteredOn is not null && x.ExitedOn is not null);
	}
	public static long GetSize(this ComponentBase obj,
		List<WvTraceMemoryInfo> memoryDetails,
		WvBlazorTraceConfiguration configuration,
		int maxDepth = 5)
	{
		if (obj == null) return 0;
		return MemorySizeCalculator.CalculateComponentMemorySize(
		component: obj,
		memoryDetails: memoryDetails,
		configuration: configuration,
		maxDepth: maxDepth
		);
	}
	public static double ToKilobytes(this long bytes)
	{
		if (bytes < 0)
		{
			return -1; // Handle invalid input
		}
		const double kilobyteFactor = 1024.0;
		return Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
	}
	public static string GetMemoryInfoId(string assemblyFullName, string fieldName)
		=> $"{assemblyFullName}$$${fieldName}";

	public static void ConsoleLog(string message)
	{
#if DEBUG
		Console.WriteLine($"$$$$$$ [{DateTime.Now.ToString("HH:mm:ss:ffff")}] =>{message} ");
#endif
	}
}
