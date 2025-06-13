using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
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
	Task<Dictionary<string, WvTraceSessionModule>> GetModuleDictAsync(bool clone = true);
	Task<Dictionary<string, WvTraceSessionSignal>> GetSignalDictAsync(bool clone = true);
	Task ClearCurrentSessionAsync();
	Task<List<WvTraceMute>> GetTraceMutes();
}
public partial class WvBlazorTraceService : IWvBlazorTraceService, IDisposable
{
	private static IServiceProvider _serviceProvider = default!;
	private IJSRuntime _jSRuntime;
	private const string _sessionStoreKey = "wvbtsession";
	private const string _generalStoreKey = "wvbtstore";
	private const string _snapshotStoreKeyPrefix = "wvbtsnapshot-";
	private Dictionary<Guid, WvServiceStore> _serviceStoreDict = new();
	private WvBlazorTraceConfiguration _configuration = new();
	private readonly ConcurrentQueue<WvTraceQueueAction> _traceQueue = new();
	private int _infiniteLoopDelaySeconds = 1;
	private Task? _infiniteLoop;
	private CancellationTokenSource? _infiniteLoopCancellationTokenSource;


	/// <summary>
	/// Needs to be implemented to destroy the infinite loop
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void Dispose()
	{
		if (_infiniteLoopCancellationTokenSource is not null)
			_infiniteLoopCancellationTokenSource.Cancel();
	}

	public WvBlazorTraceService(IJSRuntime jSRuntime,
		IWvBlazorTraceConfigurationService configurationService,
		IServiceProvider serviceProvider,
		bool triggerQueueProcessing = true)
	{
		_serviceProvider = serviceProvider;
		this._jSRuntime = jSRuntime;
		this._configuration = configurationService.GetConfiguraion();
		if (triggerQueueProcessing)
			_processQueue();
	}

	public async Task<Dictionary<string, WvTraceSessionModule>> GetModuleDictAsync(bool clone = true)
	{
		Guid sessionId = await GetSessionId();
		if (!_serviceStoreDict.ContainsKey(sessionId))
			_serviceStoreDict[sessionId] = new();
		if (clone)
			return _serviceStoreDict[sessionId].ModuleDict.Clone();
		else
			return _serviceStoreDict[sessionId].ModuleDict;
	}

	public async Task<Dictionary<string, WvTraceSessionSignal>> GetSignalDictAsync(bool clone = true)
	{
		Guid sessionId = await GetSessionId();
		if (!_serviceStoreDict.ContainsKey(sessionId))
			_serviceStoreDict[sessionId] = new();

		if (clone)
			return _serviceStoreDict[sessionId].SignalDict.Clone();
		else
			return _serviceStoreDict[sessionId].SignalDict;
	}

	public async Task<List<WvTraceMute>> GetTraceMutes()
	{
		var store = await GetLocalStoreAsync();
		return store.MutedTraces;
	}
	public async Task ClearCurrentSessionAsync()
	{
		Guid sessionId = await GetSessionId();
		_serviceStoreDict[sessionId] = new();
	}

	internal static IWvBlazorTraceService? GetScopedService()
	{
		if (_serviceProvider is null)
			return null;

		var service = _serviceProvider.GetRequiredService<IWvBlazorTraceService>();
		return service;
	}
}

