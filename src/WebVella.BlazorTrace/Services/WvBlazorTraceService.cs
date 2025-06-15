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
	Dictionary<string, WvTraceSessionModule> GetModuleDict(bool clone = true);
	Dictionary<string, WvTraceSessionSignal> GetSignalDict(bool clone = true);
	void ClearCurrentSession();
	Task<List<WvTraceMute>> GetTraceMutes(IJSRuntime jsRuntime);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService, IDisposable
{
	private static IServiceProvider _serviceProvider = default!;
	private const string _sessionStoreKey = "wvbtsession";
	private const string _generalStoreKey = "wvbtstore";
	private const string _snapshotStoreKeyPrefix = "wvbtsnapshot-";
	private WvServiceStore _serviceStore = new();
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

	public Dictionary<string, WvTraceSessionModule> GetModuleDict(bool clone = true)
	{
		if (clone)
			return _serviceStore.ModuleDict.Clone();
		else
			return _serviceStore.ModuleDict;
	}

	public Dictionary<string, WvTraceSessionSignal> GetSignalDict(bool clone = true)
	{
		if (clone)
			return _serviceStore.SignalDict.Clone();
		else
			return _serviceStore.SignalDict;
	}


	public async Task<List<WvTraceMute>> GetTraceMutes(IJSRuntime jsRuntime)
	{
		var store = await GetLocalStoreAsync(jsRuntime);
		return store.MutedTraces;
	}
	public void ClearCurrentSession()
	{
		_serviceStore = new();
	}

	internal static IWvBlazorTraceService? GetScopedService()
	{
		if (_serviceProvider is null)
			return null;

		var service = _serviceProvider.GetRequiredService<IWvBlazorTraceService>();
		return service;
	}
}

