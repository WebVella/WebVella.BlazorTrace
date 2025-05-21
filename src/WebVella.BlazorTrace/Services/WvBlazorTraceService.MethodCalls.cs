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
	/// A tracer placed at the immediate beginning of a method. Usually paired with OnExit tracer
	/// </summary>
	/// <param name="component">the component instance that calls the method. Usually provided with 'this'</param>
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="firstRender">when available, usually if the tracer is called by OnAfterRender methods</param>
	/// <param name="payloadJson">custom payload that you need stored for more advanced tracing</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnEnter(
		ComponentBase component,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? payloadJson = null,
		[CallerMemberName] string methodName = ""
		);

	/// <summary>
	/// A tracer placed just before the ending of a method. Usually in finally clause or before returns.
	/// </summary>
	/// <param name="component">the component instance that calls the method. Usually provided with 'this'</param>
	/// <param name="options">setting up the limits, exceeding which will be presented by the service as a problem</param>
	/// <param name="firstRender">when available, usually if the tracer is called by OnAfterRender methods</param>
	/// <param name="payloadJson">custom payload that you need stored for more advanced tracing</param>
	/// <param name="methodName">automatically initialized in most cases by its attribute. Override only if you need to.</param>
	void OnExit(
		ComponentBase component,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? payloadJson = null,
		[CallerMemberName] string methodName = ""
		);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	public void OnEnter(
		ComponentBase component,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? payloadJson = null,
		[CallerMemberName] string methodName = ""
	)
	{
#if !DEBUG
		return;
#endif

		if (component is null) return;
		var timestamp = DateTimeOffset.Now;
		var callerInfo = _getInfo(component, methodName);
		var trace = _findOrInitMethodTrace(callerInfo, isOnEnter: true);
		trace.EnteredOn = timestamp;
		trace.FirstRender = firstRender;
		trace.EnterPayload = payloadJson;
		trace.OnEnterMemoryBytes = component is null ? null : component.GetSize();
	}
	public void OnExit(
		ComponentBase component,
		WvTraceMethodOptions? options = null,
		bool? firstRender = null,
		string? payloadJson = null,
		[CallerMemberName] string methodName = ""
	)
	{
#if !DEBUG
		return;
#endif
		var timestamp = DateTimeOffset.Now;
		var callerInfo = _getInfo(component, methodName);
		var trace = _findOrInitMethodTrace(callerInfo, isOnEnter: false);
		trace.ExitedOn = timestamp;
		trace.FirstRender = firstRender;
		trace.ExitPayload = payloadJson;
		trace.OnExitMemoryBytes = component is null ? null : component.GetSize();
	}
}

