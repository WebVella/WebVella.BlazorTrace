using Microsoft.AspNetCore.Components;
using Nito.AsyncEx;
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
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	private static readonly AsyncLock _onEnterLock = new AsyncLock();
	private static readonly AsyncLock _onExitLock = new AsyncLock();
	private static readonly AsyncLock _onSignalLock = new AsyncLock();
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
		if (!_configuration.EnableTracing) return;
		using (_onEnterLock.Lock())
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
		if (!_configuration.EnableTracing) return;
		using (_onExitLock.Lock())
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

}

