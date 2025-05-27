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
public static partial class WvTraceUtility
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
	public static (long?, List<WvTraceMemoryInfo>?) GetLastMemory(this WvTraceSessionMethod method)
	{
		var lastExitedTrace = method.LastExitedTrace;
		if (lastExitedTrace is null)
			return (null, null);

		return ((lastExitedTrace.OnExitMemoryBytes ?? 0), lastExitedTrace.OnExitMemoryInfo);
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

	private static void CalculateLimitsInfo(this WvTraceSessionMethod method, WvTraceMethodOptions options)
	{
		method.LimitHits = new();

		//memory total
		{
			var maxMemory = method.OnEnterMaxMemoryBytes ?? 0;
			if (maxMemory < (method.OnExitMaxMemoryBytes ?? 0))
				maxMemory = (method.OnExitMaxMemoryBytes ?? 0);
			if (options.MemoryLimitTotalBytes < maxMemory)
			{
				method.LimitHits.Add(new WvTraceSessionLimitHit
				{
					Type = WvTraceSessionLimitType.MemoryTotal,
					Actual = maxMemory,
					Limit = options.MemoryLimitTotalBytes
				});
			}
		}

		//memory delta
		{
			if (options.MemoryLimitDeltaBytes < (method.MaxMemoryDeltaBytes ?? 0))
			{
				method.LimitHits.Add(new WvTraceSessionLimitHit
				{
					Type = WvTraceSessionLimitType.MemoryDelta,
					Actual = (method.MaxMemoryDeltaBytes ?? 0),
					Limit = options.MemoryLimitDeltaBytes
				});
			}
		}

		//calls
		{
			var maxCalls = method.OnEnterCallsCount;
			if (maxCalls < method.OnExitCallsCount)
				maxCalls = method.OnExitCallsCount;
			if (options.CallLimit < maxCalls)
			{
				method.LimitHits.Add(new WvTraceSessionLimitHit
				{
					Type = WvTraceSessionLimitType.MethodCalls,
					Actual = maxCalls,
					Limit = options.MemoryLimitDeltaBytes
				});
			}
		}

		//duration
		{
			var maxDuration = method.MinDurationMs ?? 0;
			if (maxDuration < (method.MaxDurationMs ?? 0))
				maxDuration = (method.MaxDurationMs ?? 0);
			if (options.DurationLimit < maxDuration)
			{
				method.LimitHits.Add(new WvTraceSessionLimitHit
				{
					Type = WvTraceSessionLimitType.Duration,
					Actual = maxDuration,
					Limit = options.DurationLimit
				});
			}
		}
	}
}
