using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial interface IWvBlazorTraceService
{
	/// <summary>
	/// A tracer placed at the immediate beginning of a method. Usually paired with OnExit tracer
	/// </summary>
	/// <param name="component">the component instance that calls the method. Usually provided with 'this'</param>
	/// <param name="traceId">unique identifier for pairing OnEnter and OnExit calls. If not provided will pair with the first non exited trace</param>
	/// <param name="instanceTag">NULL by default, will group all instances of the component in the trace. To tag each separate instance of the component you need to provide unique tag for it.</param>
	/// <param name="customData">custom string or JSON that you need stored with each call for more advanced tracing</param>
	/// <param name="firstRender">when available, usually if the tracer is called by OnAfterRender methods</param>
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnEnter(
		ComponentBase component,
		Guid? traceId = null,
		string? instanceTag = null,
		string? customData = null,
		bool? firstRender = null,
		WvTraceMethodOptions? options = null,
		[CallerMemberName] string methodName = ""
		);

	/// <summary>
	/// A tracer placed just before the ending of a method. Usually in finally clause or before returns.
	/// </summary>
	/// <param name="component">the component instance that calls the method. Usually provided with 'this'</param>
	/// <param name="traceId">unique identifier for pairing OnEnter and OnExit calls. If not provided will pair with the first non exited trace</param>
	/// <param name="instanceTag">NULL by default, will group all instances of the component in the trace. To tag each separate instance of the component you need to provide unique tag for it.</param>
	/// <param name="customData">custom string or JSON that you need stored with each call for more advanced tracing</param>
	/// <param name="firstRender">when available, usually if the tracer is called by OnAfterRender methods</param>
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnExit(
		ComponentBase component,
		Guid? traceId = null,
		string? instanceTag = null,
		string? customData = null,
		bool? firstRender = null,
		WvTraceMethodOptions? options = null,
		[CallerMemberName] string methodName = ""
		);

	/// <summary>
	/// A tracer that will log a signal call. Can be placed anywhere in a component or service method.
	/// </summary>
	/// <param name="caller">the caller instance that calls the method. Usually provided with 'this'</param>
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
	private readonly Lock _onEnterLock = new Lock();
	private readonly Lock _onExitLock = new Lock();
	public void OnEnter(
		ComponentBase component,
		Guid? traceId = null,
		string? instanceTag = null,
		string? customData = null,
		bool? firstRender = null,
		WvTraceMethodOptions? options = null,
		[CallerMemberName] string methodName = ""
	)
	{
#if !DEBUG
		return;
#endif
		if (!_configuration.EnableTracing) return;
		lock (_onEnterLock)
		{
			_addToQueue(new WvTraceQueueAction
			{
				MethodCalled = WvTraceQueueItemMethod.OnEnter,
				Caller = component,
				TraceId = traceId,
				CustomData = customData,
				FirstRender = firstRender,
				InstanceTag = instanceTag,
				MethodName = methodName,
				MethodOptions = options ?? (_configuration.DefaultTraceMethodOptions ?? new()),
				SignalOptions = new(),
				SignalName = String.Empty,
				Timestamp = DateTimeOffset.Now
			});
		}
	}
	public void OnExit(
		ComponentBase component,
		Guid? traceId = null,
		string? instanceTag = null,
		string? customData = null,
		bool? firstRender = null,
		WvTraceMethodOptions? options = null,
		[CallerMemberName] string methodName = ""
	)
	{
#if !DEBUG
		return;
#endif

		if (!_configuration.EnableTracing) return;
		lock (_onExitLock)
		{
			_addToQueue(new WvTraceQueueAction
			{
				MethodCalled = WvTraceQueueItemMethod.OnExit,
				Caller = component,
				TraceId = traceId,
				CustomData = customData,
				FirstRender = firstRender,
				InstanceTag = instanceTag,
				MethodName = methodName,
				MethodOptions = options ?? (_configuration.DefaultTraceMethodOptions ?? new()),
				SignalOptions = new(),
				SignalName = String.Empty,
				Timestamp = DateTimeOffset.Now
			});
		}
	}

	public void OnSignal(
		object caller,
		string signalName,
		string? instanceTag = null,
		string? customData = null,
		WvTraceSignalOptions? options = null,
		[CallerMemberName] string methodName = ""
		)
	{
#if !DEBUG
		return;
#endif

		if (!_configuration.EnableTracing) return;
		lock (_onExitLock)
		{
			_addToQueue(new WvTraceQueueAction
			{
				MethodCalled = WvTraceQueueItemMethod.Signal,
				Caller = caller,
				SignalName = signalName,
				TraceId = null,
				CustomData = customData,
				FirstRender = null,
				InstanceTag = instanceTag,
				MethodName = methodName,
				MethodOptions = new(),
				SignalOptions = options ?? (_configuration.DefaultTraceSignalOptions ?? new()),
				Timestamp = DateTimeOffset.Now
			});
		}
	}
}

