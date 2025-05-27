using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;

public class WvTraceModalRequest
{
	public Guid? PrimarySnapshotId { get; set; } = null;
	public Guid? SecondarySnapshotId { get; set; } = null;
	public WvTraceModalMenu? Menu { get; set; } = null;
	public bool SortAscending { get; set; } = true;
	public string? ModuleFilter { get; set; } = null;
	public string? ComponentFilter { get; set; } = null;
	public string? MethodFilter { get; set; } = null;
	public WvTraceModalMemoryFilter? MemoryFilter { get; set; } = null;
	public WvTraceModalDurationFilter? DurationFilter { get; set; } = null;
	public WvTraceModalCallsFilter? CallsFilter { get; set; } = null;
	public WvTraceModalLimitsFilter? LimitsFilter { get; set; } = null;
	public bool IsAutoRefresh { get; set; } = false;
	public bool IsEmpty
	{
		get => 
		PrimarySnapshotId is null
		&& SecondarySnapshotId is null
		&& Menu == null
		&& SortAscending
		&& ModuleFilter is null
		&& ComponentFilter is null
		&& MethodFilter is null
		&& MemoryFilter is null
		&& DurationFilter is null
		&& CallsFilter is null
		&& LimitsFilter is null
		&& !IsAutoRefresh;
	}
	public bool HasFilter{ 
		get => !String.IsNullOrWhiteSpace(ModuleFilter)
		|| !String.IsNullOrWhiteSpace(ComponentFilter)
		|| !String.IsNullOrWhiteSpace(MethodFilter)
		|| MemoryFilter is not null
		|| DurationFilter is not null
		|| CallsFilter is not null;
	}
}

public enum WvTraceModalMenu
{
	[Description("Calls")]
	MethodCalls = 0,
	[Description("Memory")]
	MethodMemory = 1,
	[Description("Duration")]
	MethodDuration = 2,
	[Description("Limits")]
	MethodLimits = 3,
	[Description("Name")]
	MethodName = 4,
	[Description("Calls")]
	SignalCalls = 5,
	[Description("Memory")]
	SignalMemory = 6,
	[Description("Limits")]
	SignalLimits = 7,
	[Description("Name")]
	SignalName = 8,
}
public enum WvTraceModalMemoryFilter
{
	[Description("<= 5 KB")]
	LessThan5 = 0,
	[Description("5 to 42 KB")]
	FiveTo42= 1,
	[Description("> 42 KB")]
	MoreThan42 = 2
}

public enum WvTraceModalDurationFilter
{
	[Description("<= 5 ms")]
	LessThan5 = 0,
	[Description("5 to 42 ms")]
	FiveTo42= 1,
	[Description("> 42 ms")]
	MoreThan42 = 2
}

public enum WvTraceModalCallsFilter
{
	[Description("<= 5 calls")]
	LessThan5 = 0,
	[Description("5 to 42 calls")]
	FiveTo42 = 1,
	[Description("> 42 calls")]
	MoreThan42 = 2
}

public enum WvTraceModalLimitsFilter
{
	[Description("0 limits exceeded")]
	ZeroLimitHits = 0,
	[Description("exceeds calls limit")]
	ExceedCallLimit= 1,
	[Description("exceeds total memory limit")]
	ExceedTotalMemory= 2,
	[Description("exceeds memory delta limit")]
	ExceedMemoryDelta= 3,
	[Description("exceeds duration limit")]
	ExceedDuration= 4,
}

public class WvTraceModalData
{
	public WvTraceModalRequest Request { get; set; } = new();
	public List<WvSelectOption> SnapshotOptions { get; set; } = new();
	public List<WvTraceRow> TraceRows { get; set; } = new();
}

