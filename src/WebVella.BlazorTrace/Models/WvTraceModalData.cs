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
	public WvTraceModalSort? Sort { get; set; } = null;
	public WvTraceModalMenu Menu { get; set; } = WvTraceModalMenu.MethodName;
	public bool SortAscending { get; set; } = true;
	public string? ModuleFilter { get; set; } = null;
	public string? ComponentFilter { get; set; } = null;
	public string? MethodFilter { get; set; } = null;
	public WvTraceModalMemoryFilter? MemoryFilter { get; set; } = null;
	public WvTraceModalDurationFilter? DurationFilter { get; set; } = null;
	public WvTraceModalCallsFilter? CallsFilter { get; set; } = null;
	public bool IsAutoRefresh { get; set; } = false;
	public bool IsEmpty
	{
		get => 
		PrimarySnapshotId is null
		&& SecondarySnapshotId is null
		&& Sort is null
		&& Menu == WvTraceModalMenu.MethodName
		&& SortAscending
		&& ModuleFilter is null
		&& ComponentFilter is null
		&& MethodFilter is null
		&& MemoryFilter is null
		&& DurationFilter is null
		&& CallsFilter is null
		&& !IsAutoRefresh;
	}
}

public enum WvTraceModalMenu
{
	MethodName = 0,
}

public enum WvTraceModalSort
{
	ByModule = 0,
	ByComponent = 1,
	ByMethod = 2,
	ByCalls = 3,
	ByDuration = 4,
	ByMemory = 5
}

public enum WvTraceModalMemoryFilter
{
	[Description("option")]
	Option1 = 0,
}

public enum WvTraceModalDurationFilter
{
	[Description("option")]
	Option1 = 0,
}

public enum WvTraceModalCallsFilter
{
	[Description("option")]
	Option1 = 0,
}


public class WvTraceModalData
{
	public WvTraceModalRequest Request { get; set; } = new();
	public List<WvSelectOption> SnapshotOptions { get; set; } = new();
	public List<WvTraceRow> TraceRows { get; set; } = new();
}

