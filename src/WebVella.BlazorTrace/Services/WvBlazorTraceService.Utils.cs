using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Concurrent;
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
public partial class WvBlazorTraceService : IWvBlazorTraceService, IDisposable
{
	private void _saveSessionTrace(WvTraceInfo traceInfo, WvTraceQueueAction action)
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
		if (!component.TaggedInstances.Any(x => x.Tag == traceInfo.InstanceTag))
			component.TaggedInstances.Add(new WvTraceSessionComponentTaggedInstance() { Tag = traceInfo.InstanceTag });

		var componentTaggedInstance = component.TaggedInstances.Single(x => x.Tag == traceInfo.InstanceTag);

		if (action.MethodCalled == WvTraceQueueItemMethod.OnEnter)
		{
			var trace = new WvTraceSessionTrace()
			{
				TraceId = traceInfo.TraceId,
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>(),
				EnteredOn = action.Timestamp,
				OnEnterFirstRender = action.FirstRender,
				OnEnterCallTag = action.CallTag,
				OnEnterMemoryBytes = null,
				OnEnterOptions = action.MethodOptions,
				ExitedOn = null,
				OnExitCallTag = null,
				OnExitFirstRender = null,
				OnExitMemoryBytes = null,
				OnExitMemoryInfo = null,
				OnExitOptions = default!
			};
			trace.OnEnterMemoryBytes = action.Component is null ? null : action.Component.GetSize(trace.OnEnterMemoryInfo, _configuration);

			if (traceInfo.IsOnInitialized)
			{
				componentTaggedInstance.OnInitialized.Name = traceInfo.MethodName;
				componentTaggedInstance.OnInitialized.TraceList.Add(trace);
			}
			else if (traceInfo.IsOnParameterSet)
			{
				componentTaggedInstance.OnParameterSet.Name = traceInfo.MethodName;
				componentTaggedInstance.OnParameterSet.TraceList.Add(trace);
			}
			else if (traceInfo.IsOnAfterRender)
			{
				componentTaggedInstance.OnAfterRender.Name = traceInfo.MethodName;
				componentTaggedInstance.OnAfterRender.TraceList.Add(trace);
			}
			else if (traceInfo.IsShouldRender)
			{
				componentTaggedInstance.ShouldRender.Name = traceInfo.MethodName;
				componentTaggedInstance.ShouldRender.TraceList.Add(trace);
			}
			else if (traceInfo.IsDispose)
			{
				componentTaggedInstance.Dispose.Name = traceInfo.MethodName;
				componentTaggedInstance.Dispose.TraceList.Add(trace);
			}
			else if (traceInfo.IsOther)
			{
				var otherMethod = componentTaggedInstance.OtherMethods.FirstOrDefault(x => x.Name == traceInfo.MethodName);
				if (otherMethod is null)
				{
					otherMethod = new() { Name = traceInfo.MethodName };
					componentTaggedInstance.OtherMethods.Add(otherMethod);
				}
				otherMethod.TraceList.Add(trace);
			}
			else
				throw new Exception("method type not supported");
		}
		else if(action.MethodCalled == WvTraceQueueItemMethod.OnExit)
		{

			WvTraceSessionTrace? trace = null;
			if (traceInfo.IsOnInitialized)
			{
				componentTaggedInstance.OnInitialized.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.OnInitialized.TraceList
					.FirstOrDefault(x => x.ExitedOn is null && x.TraceId == traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionTrace();
					componentTaggedInstance.OnInitialized.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsOnParameterSet)
			{
				componentTaggedInstance.OnParameterSet.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.OnParameterSet.TraceList
					.FirstOrDefault(x => x.ExitedOn is null && x.TraceId == traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionTrace();
					componentTaggedInstance.OnParameterSet.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsOnAfterRender)
			{
				componentTaggedInstance.OnAfterRender.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.OnAfterRender.TraceList
					.FirstOrDefault(x => x.ExitedOn is null && x.TraceId == traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionTrace();
					componentTaggedInstance.OnAfterRender.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsShouldRender)
			{
				componentTaggedInstance.ShouldRender.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.ShouldRender.TraceList
					.FirstOrDefault(x => x.ExitedOn is null && x.TraceId == traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionTrace();
					componentTaggedInstance.ShouldRender.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsDispose)
			{
				componentTaggedInstance.Dispose.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.Dispose.TraceList
					.FirstOrDefault(x => x.ExitedOn is null && x.TraceId == traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionTrace();
					componentTaggedInstance.Dispose.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsOther)
			{
				var otherMethod = componentTaggedInstance.OtherMethods.FirstOrDefault(x => x.Name == traceInfo.MethodName);
				if (otherMethod is null)
				{
					otherMethod = new() { Name = traceInfo.MethodName };
					componentTaggedInstance.OtherMethods.Add(otherMethod);
				}
				var firstNotExitedTrace = otherMethod.TraceList
					.FirstOrDefault(x => x.ExitedOn is null && x.TraceId == traceInfo.TraceId);
				if (firstNotExitedTrace is null)
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

			trace.TraceId = action.TraceId;
			trace.OnExitMemoryInfo = new List<WvTraceMemoryInfo>();
			trace.ExitedOn = action.Timestamp;
			trace.OnExitCallTag = action.CallTag;
			trace.OnExitFirstRender = action.FirstRender;
			trace.OnExitMemoryBytes = action.Component is null ? null : action.Component.GetSize(trace.OnExitMemoryInfo, _configuration);
			trace.OnExitOptions = action.MethodOptions;

		}
		else if(action.MethodCalled == WvTraceQueueItemMethod.Signal)
		{
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

