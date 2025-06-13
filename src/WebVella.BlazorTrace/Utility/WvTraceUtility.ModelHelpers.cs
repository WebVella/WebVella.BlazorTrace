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
	public static long? GetDurationMS(this WvTraceSessionMethodTrace method)
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
	public static string CalculateLimitsHTML(this WvTraceSessionMethodTrace trace, bool isOnEnter)
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

	public static Dictionary<string, WvTraceSessionModule> Clone(this Dictionary<string, WvTraceSessionModule> original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new Dictionary<string, WvTraceSessionModule>();
		foreach (var moduleName in original.Keys)
		{
			target[moduleName] = original[moduleName].Clone();
		}
		return target;
	}
	public static WvTraceSessionModule Clone(this WvTraceSessionModule original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionModule();
		foreach (var componentFullName in original.ComponentDict.Keys)
		{
			target.ComponentDict[componentFullName] = original.ComponentDict[componentFullName].Clone();
		}
		return target;
	}
	public static WvTraceSessionComponent Clone(this WvTraceSessionComponent original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionComponent
		{
			Name = original.Name,
		};
		foreach (var componentInstanceOriginal in original.TaggedInstances)
		{
			target.TaggedInstances.Add(componentInstanceOriginal.Clone());
		}
		return target;
	}
	public static WvTraceSessionComponentTaggedInstance Clone(this WvTraceSessionComponentTaggedInstance original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionComponentTaggedInstance
		{
			Tag = original.Tag,
			OnInitialized = original.OnInitialized.Clone(),
			OnParameterSet = original.OnParameterSet.Clone(),
			OnAfterRender = original.OnAfterRender.Clone(),
			ShouldRender = original.ShouldRender.Clone(),
			Dispose = original.Dispose.Clone(),
			OtherMethods = new()
		};
		foreach (var method in original.OtherMethods)
		{
			target.OtherMethods.Add(method.Clone());
		}
		return target;
	}
	public static WvTraceSessionMethod Clone(this WvTraceSessionMethod original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionMethod()
		{
			Name = original.Name,
			TraceList = new()
		};
		foreach (var trace in original.TraceList)
		{
			target.TraceList.Add(trace.Clone());
		}
		return target;
	}
	public static WvTraceSessionMethodTrace Clone(this WvTraceSessionMethodTrace original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionMethodTrace()
		{
			TraceId = original.TraceId,
			EnteredOn = original.EnteredOn,
			ExitedOn = original.ExitedOn,
			OnEnterMemoryBytes = original.OnEnterMemoryBytes,
			OnExitMemoryBytes = original.OnExitMemoryBytes,
			OnEnterFirstRender = original.OnEnterFirstRender,
			OnExitFirstRender = original.OnExitFirstRender,
			OnEnterCustomData = original.OnEnterCustomData,
			OnExitCustomData = original.OnExitCustomData,
			OnEnterOptions = original.OnEnterOptions.Clone(),
			OnExitOptions = original.OnExitOptions.Clone(),
			OnEnterMemoryInfo = original.OnEnterMemoryInfo is null ? null : new(),
			OnExitMemoryInfo = original.OnExitMemoryInfo is null ? null : new(),
		};

		foreach (var item in (original.OnEnterMemoryInfo ?? new()))
			target.OnEnterMemoryInfo!.Add(item.Clone());

		foreach (var item in (original.OnExitMemoryInfo ?? new()))
			target.OnExitMemoryInfo!.Add(item.Clone());

		return target;
	}
	public static WvTraceMethodOptions Clone(this WvTraceMethodOptions original)
	{
		if (original is null)
			throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceMethodOptions()
		{
			CallLimit = original.CallLimit,
			DurationLimitMS = original.DurationLimitMS,
			MemoryLimitDeltaBytes = original.MemoryLimitDeltaBytes,
			MemoryLimitTotalBytes = original.MemoryLimitTotalBytes,
		};

		return target;
	}
	public static WvTraceMemoryInfo Clone(this WvTraceMemoryInfo original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceMemoryInfo()
		{
			AssemblyName = original.AssemblyName,
			FieldName = original.FieldName,
			Size = original.Size,
			TypeName = original.TypeName,
		};

		return target;
	}
	public static Dictionary<string, WvTraceSessionSignal> Clone(this Dictionary<string, WvTraceSessionSignal> original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new Dictionary<string, WvTraceSessionSignal>();
		foreach (var moduleName in original.Keys)
		{
			target[moduleName] = original[moduleName].Clone();
		}
		return target;
	}
	public static WvTraceSessionSignal Clone(this WvTraceSessionSignal original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionSignal();
		foreach (var trace in original.TraceList)
		{
			target.TraceList.Add(trace.Clone());
		}
		return target;
	}
	public static WvTraceSessionSignalTrace Clone(this WvTraceSessionSignalTrace original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSessionSignalTrace()
		{
			SendOn = original.SendOn,
			MethodName = original.MethodName,
			ComponentFullName = original.ComponentFullName,
			ComponentName = original.ComponentName,
			CustomData = original.CustomData,
			InstanceTag = original.InstanceTag,
			ModuleName = original.ModuleName,
			Options = original.Options.Clone(),
		};
		return target;
	}
	public static WvTraceSignalOptions Clone(this WvTraceSignalOptions original)
	{
		if (original is null) throw new Exception("Cannot clone nullable object ");
		var target = new WvTraceSignalOptions()
		{
			CallLimit = original.CallLimit,
		};
		return target;
	}

	//public static List<WvTraceMute> Clone(this List<WvTraceMute> original)
	//{
	//	if (original is null) throw new Exception("Cannot clone nullable object ");
	//	var target = new List<WvTraceMute>();
	//	foreach (var item in original)
	//	{
	//		target.Add(item.Clone());
	//	}

	//	return target;
	//}

	//public static WvTraceMute Clone(this WvTraceMute original)
	//{
	//	if (original is null) throw new Exception("Cannot clone nullable object ");
	//	var target = new WvTraceMute()
	//	{
	//		ComponentName = original.ComponentName,
	//		ComponentFullName = original.ComponentFullName,
	//		OnEnterCustomData = original.OnEnterCustomData,
	//		InstanceTag = original.InstanceTag,
	//		Field = original.Field,
	//		IsPinned = original.IsPinned,
	//		Method = original.Method,
	//		Module = original.Module,
	//		Signal = original.Signal,
	//		Type = original.Type,
	//	};

	//	return target;
	//}

}
