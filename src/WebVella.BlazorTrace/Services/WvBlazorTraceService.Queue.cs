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
	ConcurrentQueue<WvTraceQueueAction> GetQueue();
	void ProcessQueueForTests();
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	public ConcurrentQueue<WvTraceQueueAction> GetQueue() => _traceQueue;
	private void _addToQueue(WvTraceQueueAction trace)
	{
		_traceQueue.Enqueue(trace);
	}
	private void _processQueue()
	{
		_infiniteLoopCancellationTokenSource = new CancellationTokenSource();
		_infiniteLoop = Task.Run(async () =>
		{
			while (!_infiniteLoopCancellationTokenSource.IsCancellationRequested)
			{
				await Task.Delay(_infiniteLoopDelaySeconds * 1000);
				while (!_traceQueue.IsEmpty)
				{
					try
					{
						if (_traceQueue.TryDequeue(out var trace))
						{
							_processQueueTrace(trace);
						}
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
						break;
					}
				}

			}
		}, _infiniteLoopCancellationTokenSource.Token);
	}
	private void _processQueueTrace(WvTraceQueueAction action)
	{
		if (action is null) return;
		if (action.Component is null) return;
		var traceInfo = action.Component.GetInfo(action.TraceId, action.InstanceTag, action.MethodName);
		if (traceInfo is null)
			throw new Exception("callerInfo cannot be evaluated");
		_saveSessionTrace(traceInfo, action);
	}

	public void ProcessQueueForTests()
	{
		while (!_traceQueue.IsEmpty)
		{
			try
			{
				if (_traceQueue.TryDequeue(out var trace))
				{
					_processQueueTrace(trace);
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message.ToString() + Environment.NewLine + ex.StackTrace);
				break;
			}
		}
	}
}

