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
	void ForceProcessQueue();
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	public ConcurrentQueue<WvTraceQueueAction> GetQueue() => _traceQueue;
	// Because of the force process the queue process could be triggered from two methods
	public Lock _queueProcessLock = LockFactory.Create();
	private void _addToQueue(WvTraceQueueAction trace)
	{
		_traceQueue.Enqueue(trace);
	}
	private void _processQueue()
	{
		_infiniteLoopCancellationTokenSource = new CancellationTokenSource();
		_infiniteLoop = Task.Run(async () =>
		{
			//Just to be sure that local trace mutes are loaded
			_ = await GetTraceMutes();
			while (!_infiniteLoopCancellationTokenSource.IsCancellationRequested)
			{
				await Task.Delay(_infiniteLoopDelaySeconds * 1000);
				lock (_queueProcessLock)
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
		}, _infiniteLoopCancellationTokenSource.Token);
	}
	private void _processQueueTrace(WvTraceQueueAction action)
	{
		if (action is null) return;
		if (action.Caller is null) return;
		var traceInfo = action.Caller.GetInfo(action.TraceId, action.InstanceTag, action.MethodName);
		if (traceInfo is null)
			throw new Exception("callerInfo cannot be evaluated");
		_saveSessionTrace(traceInfo, action);
	}

	public void ForceProcessQueue()
	{
		lock (_queueProcessLock)
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
}

