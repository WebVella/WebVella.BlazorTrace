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
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="firstRender">when available, usually if the tracer is called by OnAfterRender methods</param>
	/// <param name="callTag">custom string or JSON that you need stored with each call for more advanced tracing</param>
	/// <param name="instanceTag">NULL by default, will group all instances of the component in the trace. A way to tag each separate instance of the component so you can see them separately</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnEnter(
		ComponentBase component,
		Guid? traceId = null,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? instanceTag = null,
		string? callTag = null,
		[CallerMemberName] string methodName = ""
		);

	/// <summary>
	/// A tracer placed just before the ending of a method. Usually in finally clause or before returns.
	/// </summary>
	/// <param name="component">the component instance that calls the method. Usually provided with 'this'</param>
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="firstRender">when available, usually if the tracer is called by OnAfterRender methods</param>
	/// <param name="callTag">custom string or JSON that you need stored with each call for more advanced tracing</param>
	/// <param name="instanceTag">NULL by default, will group all instances of the component in the trace. A way to tag each separate instance of the component so you can see them separately</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnExit(
		ComponentBase component,
		Guid? traceId = null,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? instanceTag = null,
		string? callTag = null,
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
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? instanceTag = null,
		string? callTag = null,
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
				Component = component,
				TraceId = traceId,
				CallTag = callTag,
				FirstRender = firstRender,
				InstanceTag = instanceTag,
				MethodName = methodName,
				Options = options ?? new WvTraceMethodOptions()
			});
		}
	}
	public void OnExit(
		ComponentBase component,
		Guid? traceId = null,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? instanceTag = null,
		string? callTag = null,
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
				Component = component,
				TraceId = traceId,
				CallTag = callTag,
				FirstRender = firstRender,
				InstanceTag = instanceTag,
				MethodName = methodName,
				Options = options ?? new WvTraceMethodOptions()
			});
		}
	}
}

