using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceQueueAction
{
	public WvTraceQueueItemMethod MethodCalled { get; set; } = WvTraceQueueItemMethod.OnEnter;
	public ComponentBase Component { get; set; } = default!;
	public Guid? TraceId { get; set; } = null;
	public WvTraceMethodOptions Options { get; set; } = default!;
	public bool? FirstRender { get; set; } = null;
	public string? InstanceTag { get; set; } = null;
	public string? CallTag { get; set; } = null;
	public string MethodName { get; set; } = default!;
}

public enum WvTraceQueueItemMethod
{
	OnEnter = 0,
	OnExit = 1
}
