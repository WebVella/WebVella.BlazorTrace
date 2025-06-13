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
	Task<Dictionary<string, WvTraceSessionModule>> GetModuleDictAsync(IJSRuntime jsRuntime,bool clone = true);
	Task<Dictionary<string, WvTraceSessionSignal>> GetSignalDictAsync(IJSRuntime jsRuntime,bool clone = true);
	Task ClearCurrentSessionAsync(IJSRuntime jsRuntime);
	Task<List<WvTraceMute>> GetTraceMutes(IJSRuntime jsRuntime);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService, IDisposable
{
	private static IServiceProvider _serviceProvider = default!;
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

	public WvBlazorTraceService(IWvBlazorTraceConfigurationService configurationService,
		IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		this._configuration = configurationService.GetConfiguraion();
	}

	public async Task<Dictionary<string, WvTraceSessionModule>> GetModuleDictAsync(IJSRuntime jsRuntime,bool clone = true)
	{
		Guid sessionId = await GetSessionId(jsRuntime);
		if (!_serviceStoreDict.ContainsKey(sessionId))
			_serviceStoreDict[sessionId] = new();
		if (clone)
			return _serviceStoreDict[sessionId].ModuleDict.Clone();
		else
			return _serviceStoreDict[sessionId].ModuleDict;
	}

	public async Task<Dictionary<string, WvTraceSessionSignal>> GetSignalDictAsync(IJSRuntime jsRuntime,bool clone = true)
	{
		Guid sessionId = await GetSessionId(jsRuntime);
		if (!_serviceStoreDict.ContainsKey(sessionId))
			_serviceStoreDict[sessionId] = new();

		if (clone)
			return _serviceStoreDict[sessionId].SignalDict.Clone();
		else
			return _serviceStoreDict[sessionId].SignalDict;
	}

	public async Task<List<WvTraceMute>> GetTraceMutes(IJSRuntime jsRuntime)
	{
		var store = await GetLocalStoreAsync(jsRuntime);
		return store.MutedTraces;
	}
	public async Task ClearCurrentSessionAsync(IJSRuntime jsRuntime)
	{
		Guid sessionId = await GetSessionId(jsRuntime);
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

