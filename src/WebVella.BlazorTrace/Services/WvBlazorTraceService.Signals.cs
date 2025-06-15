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
	/// <summary>
	/// A tracer that will log a signal call. Can be placed anywhere in a component or service method.
	/// </summary>
	/// <param name="caller">the class instance that calls the method. Usually provided with 'this'</param>
	/// <param name="signalName">Unique identifier of the signal</param>
	/// <param name="instanceTag">NULL by default, will group all instances of the caller in the trace. To tag each separate instance of the caller you need to provide unique tag for it.</param>
	/// <param name="customData">custom string or JSON that you need stored with each call for more advanced tracing</param>
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnSignal(
		object caller,
		string signalName,
		string? instanceTag = null,
		string? customData = null,
		WvTraceSignalOptions? options = null,
		[CallerMemberName] string methodName = ""
		);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	public void OnSignal(
		object caller,
		string signalName,
		string? instanceTag = null,
		string? customData = null,
		WvTraceSignalOptions? options = null,
		[CallerMemberName] string methodName = ""
		)
	{
		if (!_configuration.EnableTracing) return;
		using (_onSignalLock.Lock())
		{
			_addToQueue(new WvTraceQueueAction
			{
				MethodCalled = WvTraceQueueItemMethod.Signal,
				Caller = caller,
				SignalName = signalName,
				TraceId = null,
				CustomData = customData,
				InstanceTag = instanceTag,
				MethodName = methodName,
				MethodOptions = new(),
				SignalOptions = options ?? (_configuration.DefaultTraceSignalOptions ?? new()),
				Timestamp = DateTimeOffset.Now
			});
		}
	}
}

