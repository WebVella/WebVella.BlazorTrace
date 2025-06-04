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
		if (action.MethodCalled == WvTraceQueueItemMethod.OnEnter)
		{
			if (String.IsNullOrWhiteSpace(traceInfo.ModuleName))
				throw new ArgumentNullException(nameof(traceInfo), "ModuleName is required");

			if (String.IsNullOrWhiteSpace(traceInfo.ComponentFullName))
				throw new ArgumentNullException(nameof(traceInfo), "CompFullName is required");

			if (String.IsNullOrWhiteSpace(traceInfo.MethodName))
				throw new ArgumentNullException(nameof(traceInfo), "MethodName is required");
			if (!_moduleDictInternal.ContainsKey(traceInfo.ModuleName))
				_moduleDictInternal[traceInfo.ModuleName] = new();

			var module = _moduleDictInternal[traceInfo.ModuleName];
			if (!module.ComponentDict.ContainsKey(traceInfo.ComponentFullName))
			{
				module.ComponentDict[traceInfo.ComponentFullName] = new()
				{
					Name = traceInfo.ComponentName
				};
			}
			var component = module.ComponentDict[traceInfo.ComponentFullName];
			if (!component.TaggedInstances.Any(x => x.Tag == traceInfo.InstanceTag))
				component.TaggedInstances.Add(new WvTraceSessionComponentTaggedInstance() { Tag = traceInfo.InstanceTag });

			var componentTaggedInstance = component.TaggedInstances.Single(x => x.Tag == traceInfo.InstanceTag);
			var trace = new WvTraceSessionMethodTrace()
			{
				TraceId = traceInfo.TraceId ?? Guid.NewGuid(),
				OnEnterMemoryInfo = new List<WvTraceMemoryInfo>(),
				EnteredOn = action.Timestamp,
				OnEnterFirstRender = action.FirstRender,
				OnEnterCustomData = action.CustomData,
				OnEnterMemoryBytes = null,
				OnEnterOptions = action.MethodOptions,
				ExitedOn = null,
				OnExitCustomData = null,
				OnExitFirstRender = null,
				OnExitMemoryBytes = null,
				OnExitMemoryInfo = null,
				OnExitOptions = new()
			};
			trace.OnEnterMemoryBytes = action.Caller.GetSize(trace.OnEnterMemoryInfo, _configuration);

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
		else if (action.MethodCalled == WvTraceQueueItemMethod.OnExit)
		{
			if (String.IsNullOrWhiteSpace(traceInfo.ModuleName))
				throw new ArgumentNullException(nameof(traceInfo), "ModuleName is required");

			if (String.IsNullOrWhiteSpace(traceInfo.ComponentFullName))
				throw new ArgumentNullException(nameof(traceInfo), "CompFullName is required");

			if (String.IsNullOrWhiteSpace(traceInfo.MethodName))
				throw new ArgumentNullException(nameof(traceInfo), "MethodName is required");


			if (!_moduleDictInternal.ContainsKey(traceInfo.ModuleName))
				_moduleDictInternal[traceInfo.ModuleName] = new();

			var module = _moduleDictInternal[traceInfo.ModuleName];
			if (!module.ComponentDict.ContainsKey(traceInfo.ComponentFullName))
			{
				module.ComponentDict[traceInfo.ComponentFullName] = new()
				{
					Name = traceInfo.ComponentName
				};
			}
			var component = module.ComponentDict[traceInfo.ComponentFullName];
			if (!component.TaggedInstances.Any(x => x.Tag == traceInfo.InstanceTag))
				component.TaggedInstances.Add(new WvTraceSessionComponentTaggedInstance() { Tag = traceInfo.InstanceTag });

			var componentTaggedInstance = component.TaggedInstances.Single(x => x.Tag == traceInfo.InstanceTag);
			WvTraceSessionMethodTrace? trace = null;
			if (traceInfo.IsOnInitialized)
			{
				componentTaggedInstance.OnInitialized.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.OnInitialized.TraceList.GetMatchingOnEnterTrace(traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionMethodTrace();
					componentTaggedInstance.OnInitialized.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsOnParameterSet)
			{
				componentTaggedInstance.OnParameterSet.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.OnParameterSet.TraceList.GetMatchingOnEnterTrace(traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionMethodTrace();
					componentTaggedInstance.OnParameterSet.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsOnAfterRender)
			{
				componentTaggedInstance.OnAfterRender.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.OnAfterRender.TraceList.GetMatchingOnEnterTrace(traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionMethodTrace();
					componentTaggedInstance.OnAfterRender.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsShouldRender)
			{
				componentTaggedInstance.ShouldRender.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.ShouldRender.TraceList.GetMatchingOnEnterTrace(traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionMethodTrace();
					componentTaggedInstance.ShouldRender.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else if (traceInfo.IsDispose)
			{
				componentTaggedInstance.Dispose.Name = traceInfo.MethodName;
				var firstNotExitedTrace = componentTaggedInstance.Dispose.TraceList.GetMatchingOnEnterTrace(traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionMethodTrace();
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
				var firstNotExitedTrace = otherMethod.TraceList.GetMatchingOnEnterTrace(traceInfo.TraceId);
				if (firstNotExitedTrace is null)
				{
					trace = new WvTraceSessionMethodTrace();
					otherMethod.TraceList.Add(trace);
				}
				else
					trace = firstNotExitedTrace;
			}
			else
				throw new Exception("method type not supported");

			if (trace is null)
				throw new Exception("trace could not be initialized or found");

			trace.TraceId = action.TraceId ?? Guid.NewGuid();
			trace.OnExitMemoryInfo = new List<WvTraceMemoryInfo>();
			trace.ExitedOn = action.Timestamp;
			trace.OnExitCustomData = action.CustomData;
			trace.OnExitFirstRender = action.FirstRender;
			trace.OnExitMemoryBytes = action.Caller is null ? null : action.Caller.GetSize(trace.OnExitMemoryInfo, _configuration);
			trace.OnExitOptions = action.MethodOptions;

		}
		else if (action.MethodCalled == WvTraceQueueItemMethod.Signal)
		{
			if (String.IsNullOrWhiteSpace(action.SignalName))
				throw new ArgumentNullException(nameof(action), "SignalName is required");
			if (!_signalDictInternal.ContainsKey(action.SignalName))
			{
				_signalDictInternal[action.SignalName] = new();
			}
			var signal = _signalDictInternal[action.SignalName];
			signal.TraceList.Add(new WvTraceSessionSignalTrace
			{
				SendOn = action.Timestamp,
				ModuleName = traceInfo.ModuleName,
				ComponentName = traceInfo.ComponentName,
				ComponentFullName = traceInfo.ComponentFullName,
				InstanceTag = traceInfo.InstanceTag,
				MethodName = action.MethodName,
				CustomData = action.CustomData,
				Options = action.SignalOptions
			});
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
		try{ 
		return await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key);
		}catch(Exception ex){ 
			throw;
		}
	}

	private void checkModuleDict(Dictionary<string, WvTraceSessionModule> moduleDict){ 
		var module = moduleDict[moduleDict.Keys.First()];
		Console.WriteLine($"++++++ module HASH: {module.GetHashCode()}");
		var component = module.ComponentDict[module.ComponentDict.Keys.First()];
		Console.WriteLine($"++++++ component HASH: {component.GetHashCode()}");
		var instance = component.TaggedInstances.First();
		Console.WriteLine($"++++++ instance HASH: {instance.GetHashCode()}");
		var method = instance.OnInitialized;
		Console.WriteLine($"++++++ method HASH: {method.GetHashCode()}");
		var trace = method.TraceList[1];
		Console.WriteLine($"++++++ trace HASH: {trace.GetHashCode()}");
		var onExitInfoCount = trace.OnExitMemoryInfo!.Count;
		Console.WriteLine($"++++++ onExitInfoCount HASH: {trace.OnExitMemoryInfo.GetHashCode()}");

		if(onExitInfoCount != 2)
			throw new Exception("dadasd");
	}
}

