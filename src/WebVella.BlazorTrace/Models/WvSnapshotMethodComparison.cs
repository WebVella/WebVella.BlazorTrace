using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvSnapshotMethodComparison
{
	public WvTraceSessionMethod PrimarySnapshotMethod { get; set; } = new();
	public WvTraceSessionMethod? SecondarySnapshotMethod { get; set; } = null;
	public WvSnapshotMethodComparisonData ComparisonData { get; set; } = new();
}

public class WvSnapshotMethodComparisonData
{
	public long? MinDurationMS { get; set; }
	public long? MaxDurationMS { get; set; }
	public long? AverageDurationMS { get; set; }
	public double? OnEnterMinMemoryKB { get; set; }
	public double? OnEnterMaxMemoryKB { get; set; }
	public double? OnExitMinMemoryKB { get; set; }
	public double? OnExitMaxMemoryKB { get; set; }
	public double? AverageMemoryKB { get; set; }
	public double? MinMemoryDeltaKB { get; set; }
	public double? MaxMemoryDeltaKB { get; set; }
	public long? OnEnterCallsCount { get; set; }
	public long? OnExitCallsCount { get; set; }
	public long? CompletedCallsCount { get; set; }
	public long? TraceCount { get; set; }
	public long? LimitHits { get; set; }
}
