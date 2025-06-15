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
	[JsonIgnore]
	public object Caller { get; set; } = default!;
	public Guid? TraceId { get; set; } = null;
	public WvTraceMethodOptions MethodOptions { get; set; } = default!;
	public WvTraceSignalOptions SignalOptions { get; set; } = default!;
	public string SignalName { get; set; } = default!;
	public string? InstanceTag { get; set; } = null;
	public string? CustomData { get; set; } = null;
	public string MethodName { get; set; } = default!;
	public DateTimeOffset Timestamp { get; set; } = default!;
}

public enum WvTraceQueueItemMethod
{
	OnEnter = 0,
	OnExit = 1,
	Signal = 2
}
