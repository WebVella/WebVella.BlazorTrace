using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceRow
{
	public string Id { get => WvModalUtility.GenerateHash(Module, ComponentFullName, Tag, Method); }
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? ComponentFullName { get; set; }
	public string? Tag { get; set; }
	public string? Method { get; set; }
	public bool IsBookmarked { get; set; } = false;
	public List<WvTraceSessionTrace> TraceList { get; set; } = new();
	public double? LastMemoryKB { get; set; }
	public long? LastDurationMS { get; set; }
	public long? LastDurationChangeMS { get; set; }
	public string LastDurationChangeMSHtml
	{
		get
		{
			if (LastDurationChangeMS == 0) return string.Empty;
			else if (LastDurationChangeMS < 0)
			{
				return $"<span class='negative'>{LastDurationChangeMS}</span>";
			}
			return $"<span class='positive'>+{LastDurationChangeMS}</span>";
		}
	}
	public List<WvTraceSessionLimitHit> LimitHits { get; set; } = new();
	public WvSnapshotMethodComparisonData MethodComparison { get; set; } = new();
	public WvSnapshotMemoryComparisonData MemoryComparison { get; set; } = new();
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

}

