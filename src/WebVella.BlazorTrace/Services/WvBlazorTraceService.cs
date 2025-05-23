using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial interface IWvBlazorTraceService
{
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	private IJSRuntime _jSRuntime;
	private const string _snapshotStoreKey = "wvbtstore";
	private Dictionary<string, WvTraceSessionModule> _moduleDict = new();
	private WvBlazorTraceConfiguration _configuration = new();
	public WvBlazorTraceService(IJSRuntime jSRuntime, IWvBlazorTraceConfigurationService configurationService)
	{
		this._jSRuntime = jSRuntime;
		this._configuration = configurationService.GetConfiguraion();
	}
	private WvTraceInfo? _getInfo(ComponentBase component, string methodName)
	{
		var componentType = component.GetType();
		return new WvTraceInfo
		{
			MethodName = methodName,
			ComponentFullName = componentType.FullName,
			ComponentName = (componentType.FullName ?? "").Split(".", StringSplitOptions.RemoveEmptyEntries).LastOrDefault(),
			ModuleName = componentType?.Module.Name?.Replace(".dll", "")
		};
	}

	private WvTraceSessionTrace _findOrInitMethodTrace(WvTraceInfo traceInfo, bool isOnEnter = true)
	{
		if (String.IsNullOrWhiteSpace(traceInfo.ModuleName))
			throw new ArgumentNullException(nameof(traceInfo), "ModuleName is required");

		if (String.IsNullOrWhiteSpace(traceInfo.ComponentFullName))
			throw new ArgumentNullException(nameof(traceInfo), "CompFullName is required");

		if (String.IsNullOrWhiteSpace(traceInfo.MethodName))
			throw new ArgumentNullException(nameof(traceInfo), "MethodName is required");


		if (!_moduleDict.ContainsKey(traceInfo.ModuleName))
			_moduleDict[traceInfo.ModuleName] = new();

		var module = _moduleDict[traceInfo.ModuleName];

		if (!module.ComponentDict.ContainsKey(traceInfo.ComponentFullName))
			module.ComponentDict[traceInfo.ComponentFullName] = new()
			{
				Name = traceInfo.ComponentName
			};

		var component = module.ComponentDict[traceInfo.ComponentFullName];

		WvTraceSessionTrace? trace = null;
		if (traceInfo.IsOnInitialized)
		{
			var firstNotExitedTrace = component.OnInitialized.TraceList.FirstOrDefault(x => x.ExitedOn is null);
			if (isOnEnter || firstNotExitedTrace is null)
			{
				trace = new WvTraceSessionTrace();
				component.OnInitialized.TraceList.Add(trace);
			}
			else
				trace = firstNotExitedTrace;
		}
		else if (traceInfo.IsOnParameterSet)
		{
			var firstNotExitedTrace = component.OnParameterSet.TraceList.FirstOrDefault(x => x.ExitedOn is null);
			if (isOnEnter || firstNotExitedTrace is null)
			{
				trace = new WvTraceSessionTrace();
				component.OnParameterSet.TraceList.Add(trace);
			}
			else
				trace = firstNotExitedTrace;
		}
		else if (traceInfo.IsOnAfterRender)
		{
			var firstNotExitedTrace = component.OnAfterRender.TraceList.FirstOrDefault(x => x.ExitedOn is null);
			if (isOnEnter || firstNotExitedTrace is null)
			{
				trace = new WvTraceSessionTrace();
				component.OnAfterRender.TraceList.Add(trace);
			}
			else
				trace = firstNotExitedTrace;
		}
		else if (traceInfo.IsShouldRender)
		{
			var firstNotExitedTrace = component.ShouldRender.TraceList.FirstOrDefault(x => x.ExitedOn is null);
			if (isOnEnter || firstNotExitedTrace is null)
			{
				trace = new WvTraceSessionTrace();
				component.ShouldRender.TraceList.Add(trace);
			}
			else
				trace = firstNotExitedTrace;
		}
		else if (traceInfo.IsDispose)
		{
			var firstNotExitedTrace = component.Dispose.TraceList.FirstOrDefault(x => x.ExitedOn is null);
			if (isOnEnter || firstNotExitedTrace is null)
			{
				trace = new WvTraceSessionTrace();
				component.ShouldRender.TraceList.Add(trace);
			}
			else
				trace = firstNotExitedTrace;
		}
		else if (traceInfo.IsOther)
		{
			var otherMethod = component.OtherMethods.FirstOrDefault(x => x.Name == traceInfo.MethodName);
			if (otherMethod is null)
			{
				otherMethod = new() { Name = traceInfo.MethodName };
				component.OtherMethods.Add(otherMethod);
			}

			var firstNotExitedTrace = otherMethod.TraceList.FirstOrDefault(x => x.ExitedOn is null);
			if (isOnEnter || firstNotExitedTrace is null)
			{
				trace = new WvTraceSessionTrace();
				otherMethod.TraceList.Add(trace);
			}
			else
				trace = firstNotExitedTrace;
		}
		else
			throw new Exception("method type not supported");

		if (trace is null)
			throw new Exception("trace could not be initialized or found");

		return trace;
	}

	private void _calculateLimitsInfo(WvTraceSessionMethod method, WvTraceMethodOptions options)
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

	private async Task _setUnprotectedLocalStorageAsync(string key, string value)
	{
		await _jSRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
	}

	private async Task _removeUnprotectedLocalStorageAsync(string key)
	{
		await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	private async Task<string> _getUnprotectedLocalStorageAsync(string key)
	{
		return await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
	}
}

