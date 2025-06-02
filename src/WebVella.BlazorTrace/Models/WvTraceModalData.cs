using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;

public class WvTraceModalRequest
{
	public Guid? PrimarySnapshotId { get; set; } = null;
	public Guid? SecondarySnapshotId { get; set; } = null;
	public WvTraceModalMenu? Menu { get; set; } = null;
	public WvTraceModalRequestMethodsFilter MethodsFilter { get; set; } = new();
	public WvTraceModalRequestSignalsFilter SignalsFilter { get; set; } = new();
	public WvTraceModalRequestMutedFilter MutedFilter { get; set; } = new();
	public bool IsAutoRefresh { get; set; } = false;
	[JsonIgnore]
	public bool IsEmpty
	{
		get =>
		PrimarySnapshotId is null
		&& SecondarySnapshotId is null
		&& Menu == null
		&& !MethodsFilter.HasFilter
		&& !SignalsFilter.HasFilter
		&& !MutedFilter.HasFilter
		&& !IsAutoRefresh;
	}
	[JsonIgnore]
	public bool IsMethodMenu
	{
		get
		{
			if (
				Menu == WvTraceModalMenu.MethodCalls
				|| Menu == WvTraceModalMenu.MethodMemory
				|| Menu == WvTraceModalMenu.MethodDuration
				|| Menu == WvTraceModalMenu.MethodLimits
				|| Menu == WvTraceModalMenu.MethodName
			) return true;

			return false;
		}
	}

	[JsonIgnore]
	public bool IsSignalMenu
	{
		get
		{
			if (
				Menu == WvTraceModalMenu.SignalCalls
				|| Menu == WvTraceModalMenu.SignalLimits
				|| Menu == WvTraceModalMenu.SignalName
			) return true;

			return false;
		}
	}

	[JsonIgnore]
	public bool IsSnapshotMenu
	{
		get
		{
			if (
				Menu == WvTraceModalMenu.Snapshots
			) return true;

			return false;
		}
	}
	[JsonIgnore]
	public bool IsTraceMuteMenu
	{
		get
		{
			if (
				Menu == WvTraceModalMenu.TraceMutes
			) return true;

			return false;
		}
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
	[Description("Limits")]
	SignalLimits = 7,
	[Description("Name")]
	SignalName = 8,
	[Description("Snapshots")]
	Snapshots = 9,
	[Description("Muted")]
	TraceMutes = 10,
}
public enum WvTraceModalCallsFilter
{
	[Description("<= 25 calls")]
	LessThan25 = 0,
	[Description("25 to 50 calls")]
	TwentyFiveTo50 = 1,
	[Description("> 50 calls")]
	MoreThan50 = 2,
	[Description("positive change")]
	PositiveDelta = 3,
	[Description("negative change")]
	NegativeDelta = 4,
	[Description("no change")]
	NoDelta = 5,
}
public enum WvTraceModalMemoryFilter
{
	[Description("<= 50 KB")]
	LessThan50 = 0,
	[Description("50 to 500 KB")]
	FiftyTo500 = 1,
	[Description("> 500 KB")]
	MoreThan500 = 2,
	[Description("positive change")]
	PositiveDelta = 3,
	[Description("negative change")]
	NegativeDelta = 4,
	[Description("no change")]
	NoDelta = 5,
}
public enum WvTraceModalDurationFilter
{
	[Description("<= 50 ms")]
	LessThan50 = 0,
	[Description("50 to 500 ms")]
	FiftyTo500 = 1,
	[Description("> 500 ms")]
	MoreThan500 = 2,
	[Description("positive change")]
	PositiveDelta = 3,
	[Description("negative change")]
	NegativeDelta = 4,
	[Description("no change")]
	NoDelta = 5,
}
public enum WvTraceModalLimitsFilter
{
	[Description("has limits exceeded")]
	HasLimitHits = 0,
	[Description("0 limits exceeded")]
	ZeroLimitHits = 1,
	[Description("exceeds calls limit")]
	ExceedCallLimit = 2,
	[Description("exceeds total memory limit")]
	ExceedTotalMemory = 3,
	[Description("exceeds memory delta limit")]
	ExceedMemoryDelta = 4,
	[Description("exceeds duration limit")]
	ExceedDuration = 5,
}
public class WvTraceModalData
{
	public WvTraceModalRequest Request { get; set; } = new();
	public List<WvSnapshotListItem> SnapshotList { get; set; } = new();
	public List<WvMethodTraceRow> MethodTraceRows { get; set; } = new();
	public List<WvSignalTraceRow> SignalTraceRows { get; set; } = new();
}
public class WvTraceModalRequestMethodsFilter
{
	public string? ModuleFilter { get; set; } = null;
	public string? ComponentFilter { get; set; } = null;
	public string? MethodFilter { get; set; } = null;
	public WvTraceModalMemoryFilter? MemoryFilter { get; set; } = null;
	public WvTraceModalDurationFilter? DurationFilter { get; set; } = null;
	public WvTraceModalCallsFilter? CallsFilter { get; set; } = null;
	public WvTraceModalLimitsFilter? LimitsFilter { get; set; } = null;
	[JsonIgnore]
	public bool HasFilter
	{
		get => !String.IsNullOrWhiteSpace(ModuleFilter)
		|| !String.IsNullOrWhiteSpace(ComponentFilter)
		|| !String.IsNullOrWhiteSpace(MethodFilter)
		|| MemoryFilter is not null
		|| DurationFilter is not null
		|| CallsFilter is not null
		|| LimitsFilter is not null;
	}
}

public class WvTraceModalRequestSignalsFilter
{
	public string? ModuleFilter { get; set; } = null;
	public string? SignalNameFilter { get; set; } = null;
	public WvTraceModalCallsFilter? CallsFilter { get; set; } = null;
	public WvTraceModalLimitsFilter? LimitsFilter { get; set; } = null;

	[JsonIgnore]
	public bool HasFilter
	{
		get => !String.IsNullOrWhiteSpace(ModuleFilter)
		|| !String.IsNullOrWhiteSpace(SignalNameFilter)
		|| CallsFilter is not null
		|| LimitsFilter is not null;
	}
}

public class WvTraceModalRequestMutedFilter
{
	public string? ModuleFilter { get; set; } = null;
	public string? ComponentFilter { get; set; } = null;
	public string? MethodFilter { get; set; } = null;

	[JsonIgnore]
	public bool HasFilter
	{
		get => !String.IsNullOrWhiteSpace(ModuleFilter)
		|| !String.IsNullOrWhiteSpace(ComponentFilter)
		|| !String.IsNullOrWhiteSpace(MethodFilter)
		;
	}
}


