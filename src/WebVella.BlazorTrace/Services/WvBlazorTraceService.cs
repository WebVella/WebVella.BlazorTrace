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
	Dictionary<string, WvTraceSessionModule> GetModuleDict();
	Dictionary<string, WvTraceSessionSignal> GetSignalDict();
	void ClearCurrentSession();
	Task<List<WvTraceMute>> GetTraceMutes();
}
public partial class WvBlazorTraceService : IWvBlazorTraceService, IDisposable
{
	private IJSRuntime _jSRuntime;
	private const string _snapshotStoreKey = "wvbtstore";
	private Dictionary<string, WvTraceSessionModule> _moduleDictInternal = new();
	private Dictionary<string, WvTraceSessionSignal> _signalDictInternal = new();
	private List<WvTraceMute>? _traceMutesInternal = null;
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
		bool triggerQueueProcessing = true)
	{
		this._jSRuntime = jSRuntime;
		this._configuration = configurationService.GetConfiguraion();
		if (triggerQueueProcessing)
			_processQueue();
	}
	public Dictionary<string, WvTraceSessionModule> GetModuleDict()
	{
		return _moduleDictInternal.Clone();
	}
	public Dictionary<string, WvTraceSessionSignal> GetSignalDict()
	{
		return _signalDictInternal.Clone();
	}

	public async Task<List<WvTraceMute>> GetTraceMutes()
	{
		if (_traceMutesInternal is null)
		{
			var store = await GetLocalStoreAsync();
			_traceMutesInternal = store.MutedTraces ?? new();
		}
		return _traceMutesInternal.Clone();
	}
	public void ClearCurrentSession()
	{
		_moduleDictInternal = new();
		_signalDictInternal = new();
	}
}

