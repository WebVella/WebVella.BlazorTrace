using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceRow
{
	public string Id { get => $"{Module}-{Component}-{Method}"; }
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? Method { get; set; }
	public double? AverageMemoryKB { get; set; }
	public long? AverageDurationMS { get; set; }
	public long? CallsCount { get; set; }
	public List<WvTraceSessionLimitHit> LimitHits { get; set; } = new();
	public WvSnapshotMethodComparisonData ComparisonData { get; set; } = new();
	public List<WvTraceSessionLimitHit> MemoryTotalLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.MemoryTotal).ToList(); }
	public List<WvTraceSessionLimitHit> MemoryDeltaLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.MemoryDelta).ToList(); }
	public List<WvTraceSessionLimitHit> MethodCallsLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.MethodCalls).ToList(); }
	public List<WvTraceSessionLimitHit> DurationLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.Duration).ToList(); }

	public bool ModuleMatches(string search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((Module ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
			return true;

		return false;
	}

	public bool ComponentMatches(string search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((Component ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
			return true;

		return false;
	}

	public bool MethodMatches(string search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((Method ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
			return true;

		return false;
	}
	public bool HasLimitHits { get => LimitHits.Any(); }
	public bool HasMemoryTotalLimitHits { get => MemoryTotalLimitHits.Any(); }
	public bool HasMemoryDeltaLimitHits { get => MemoryDeltaLimitHits.Any(); }
	public bool HasMethodCallsLimitHits { get => MethodCallsLimitHits.Any(); }
	public bool HasDurationLimitHits { get => DurationLimitHits.Any(); }
	public Action? OnMemoryTotalLimitsView { get; set;}
	public Action? OnMemoryDeltaLimitHits { get; set;}
	public Action? OnMethodCallsLimitHits { get; set;}
	public Action? OnDurationLimitHits { get; set;}
}

