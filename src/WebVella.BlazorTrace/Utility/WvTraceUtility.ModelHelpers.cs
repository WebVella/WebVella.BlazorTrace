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
	public static long? GetLastDuration(this WvTraceSessionMethod method)
	{
		var lastExitedTrace = method.LastExitedTrace;
		if (lastExitedTrace is null
		|| lastExitedTrace.EnteredOn is null || lastExitedTrace.ExitedOn is null)
			return null;

		return (lastExitedTrace.ExitedOn.Value - lastExitedTrace.EnteredOn.Value).Milliseconds;
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

	public static List<WvTraceSessionLimitHit> CalculateLimitsInfo(this WvTraceSessionMethod method)
	{
		var result = new List<WvTraceSessionLimitHit>();
		int processedOnEnter = 1;
		int processedOnExit = 1;
		foreach (var trace in method.TraceList)
		{
			#region << OnEnter >>
			if (trace.OnEnterOptions is not null)
			{
				//memory total
				{
					if (trace.OnEnterOptions.MemoryLimitTotalBytes < (trace.OnEnterMemoryBytes ?? 0))
					{
						result.Add(new WvTraceSessionLimitHit
						{
							IsOnEnter = true,
							Type = WvTraceSessionLimitType.MemoryTotal,
							Actual = trace.OnEnterMemoryBytes ?? 0,
							Limit = trace.OnEnterOptions.MemoryLimitTotalBytes
						});
					}
				}
				//calls
				{
					if (trace.OnEnterOptions.CallLimit < processedOnEnter)
					{
						result.Add(new WvTraceSessionLimitHit
						{
							IsOnEnter = true,
							Type = WvTraceSessionLimitType.CallCount,
							Actual = processedOnEnter,
							Limit = trace.OnEnterOptions.CallLimit
						});
					}
				}
			}
			#endregion
			#region << OnExit >>
			if (trace.OnExitOptions is not null)
			{
				//memory total
				if (trace.OnExitOptions.MemoryLimitTotalBytes < (trace.OnExitMemoryBytes ?? 0))
				{
					result.Add(new WvTraceSessionLimitHit
					{
						IsOnEnter = false,
						Type = WvTraceSessionLimitType.MemoryTotal,
						Actual = trace.OnExitMemoryBytes ?? 0,
						Limit = trace.OnExitOptions.MemoryLimitTotalBytes
					});
				}
				//memory delta
				{
					var delta = (trace.OnExitMemoryBytes ?? 0) - (trace.OnEnterMemoryBytes ?? 0);
					if (trace.OnExitOptions.MemoryLimitDeltaBytes < delta)
					{
						result.Add(new WvTraceSessionLimitHit
						{
							IsOnEnter = false,
							Type = WvTraceSessionLimitType.MemoryDelta,
							Actual = delta,
							Limit = trace.OnExitOptions.MemoryLimitDeltaBytes
						});
					}
				}
				//calls
				{
					if (trace.OnExitOptions.CallLimit < processedOnExit)
					{
						result.Add(new WvTraceSessionLimitHit
						{
							IsOnEnter = false,
							Type = WvTraceSessionLimitType.CallCount,
							Actual = processedOnExit,
							Limit = trace.OnExitOptions.CallLimit
						});
					}
				}
				//duration
				{
					if (trace.EnteredOn is not null && trace.ExitedOn is not null)
					{
						var delta = (trace.ExitedOn.Value - trace.EnteredOn.Value).Milliseconds;
						if (trace.OnExitOptions.DurationLimitMS < delta)
						{
							result.Add(new WvTraceSessionLimitHit
							{
								IsOnEnter = false,
								Type = WvTraceSessionLimitType.Duration,
								Actual = delta,
								Limit = trace.OnExitOptions.DurationLimitMS
							});
						}
					}
				}
			}
			#endregion

			if (trace.EnteredOn is not null)
				processedOnEnter++;
			if (trace.ExitedOn is not null)
				processedOnExit++;
		}


		return result;
	}

	public static string CalculateLimitsHTML(this WvTraceSessionTrace trace, bool isOnEnter)
	{
		var list = new List<string>();
		if (isOnEnter && trace.OnEnterOptions is not null)
		{
			list.Add($"<div>{trace.OnEnterOptions.CallLimit} <span class='wv-mute'>calls</span></div>");
			list.Add($"<div>{trace.OnEnterOptions.MemoryLimitTotalBytes.WvBTToKilobytesString()} <span class='wv-mute'>total memory</span></div>");
			list.Add($"<div>{trace.OnEnterOptions.MemoryLimitDeltaBytes.WvBTToKilobytesString()} <span class='wv-mute'>delta memory</span></div>");
			list.Add($"<div>{trace.OnEnterOptions.DurationLimitMS.WvBTToDurationMSString()} <span class='wv-mute'>duration</span></div>");
		}
		else if (!isOnEnter && trace.OnExitOptions is not null)
		{
			list.Add($"<div>{trace.OnExitOptions.CallLimit} <span class='wv-mute'>calls</span></div>");
			list.Add($"<div>{trace.OnExitOptions.MemoryLimitTotalBytes.WvBTToKilobytesString()} <span class='wv-mute'>total memory</span></div>");
			list.Add($"<div>{trace.OnExitOptions.MemoryLimitDeltaBytes.WvBTToKilobytesString()} <span class='wv-mute'>delta memory</span></div>");
			list.Add($"<div>{trace.OnExitOptions.DurationLimitMS.WvBTToDurationMSString()} <span class='wv-mute'>duration</span></div>");
		}
		return String.Join("", list);
	}

	public static List<WvTraceSessionLimitHit> CalculateLimitsInfo(this WvTraceSessionSignal signal)
	{
		var result = new List<WvTraceSessionLimitHit>();
		int processedCalls = 1;
		foreach (var trace in signal.TraceList)
		{
			if (trace.Options is not null)
			{
				//calls
				{
					if (trace.Options.CallLimit < processedCalls)
					{
						result.Add(new WvTraceSessionLimitHit
						{
							IsOnEnter = true,
							Type = WvTraceSessionLimitType.CallCount,
							Actual = processedCalls,
							Limit = trace.Options.CallLimit
						});
					}
				}
			}
			processedCalls++;
		}

		return result;
	}

	public static string CalculateLimitsHTML(this WvTraceSessionSignalTrace trace)
	{
		var list = new List<string>();

		if (trace.Options is not null)
		{
			list.Add($"<div>{trace.Options.CallLimit} <span class='wv-mute'>calls</span></div>");
		}
		return String.Join("", list);
	}
}
