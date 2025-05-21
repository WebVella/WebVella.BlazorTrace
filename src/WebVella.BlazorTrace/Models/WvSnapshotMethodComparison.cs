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
	public long? MinDurationMs { get; set; }
	public long? MaxDurationMs { get; set; }
	public long? AvarageDurationMs { get; set; }
	public long? OnEnterMinMemoryBytes { get; set; }
	public long? OnEnterMaxMemoryBytes { get; set; }
	public long? OnExitMinMemoryBytes { get; set; }
	public long? OnExitMaxMemoryBytes { get; set; }
	public long? AverageMemoryBytes { get; set; }
	public long? MinMemoryDeltaBytes { get; set; }
	public long? MaxMemoryDeltaBytes { get; set; }
	public long? OnEnterCallsCount { get; set; }
	public long? OnExitCallsCount { get; set; }
	public long? MaxCallsCount { get; set; }
	public long? CompletedCallsCount { get; set; }
	public long? TraceCount { get; set; }
	public long? LimitHits { get; set; }
}
