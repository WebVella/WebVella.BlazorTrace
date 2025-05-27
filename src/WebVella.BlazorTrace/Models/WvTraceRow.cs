using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceRow
{
	public string Id { get => WvModalUtility.GenerateHash(Module, ComponentFullName, InstanceTag, Method); }
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? ComponentFullName { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }
	public bool IsBookmarked { get; set; } = false;
	public List<WvTraceSessionTrace> TraceList { get; set; } = new();
	public double? LastMemoryKB { get; set; }
	public long? LastDurationMS { get; set; }
	public List<WvTraceSessionLimitHit> LimitHits { get; set; } = new();
	public string LimitsHint
	{
		get
		{
			var hints = new List<string>();
			if (MethodCallsLimitHits.Count > 0) hints.Add($"{MethodCallsLimitHits.Count} call");
			if (MemoryTotalLimitHits.Count > 0) hints.Add($"{MemoryTotalLimitHits.Count} total memory");
			if (MemoryDeltaLimitHits.Count > 0) hints.Add($"{MemoryDeltaLimitHits.Count} memory delta");
			if (DurationLimitHits.Count > 0) hints.Add($"{DurationLimitHits.Count} duration");

			if (hints.Count == 0) return "no limits are hit";

			return String.Join("; ", hints);
		}
	}
	public string LimitsHtml
	{
		get
		{
			var html = new List<string>();
			if (MethodCallsLimitHits.Count > 0) 
				html.Add($"<strong class='wv-negative'>{MethodCallsLimitHits.Count}</strong>");
			else
				html.Add($"<span class='wv-mute'>0</span>");

			if (MemoryTotalLimitHits.Count > 0) 
				html.Add($"<strong class='wv-negative'>{MemoryTotalLimitHits.Count}</strong>");
			else
				html.Add($"<span class='wv-mute'>0</span>");

			if (MemoryDeltaLimitHits.Count > 0) 
				html.Add($"<strong class='wv-negative'>{MemoryDeltaLimitHits.Count}</strong>");
			else
				html.Add($"<span class='wv-mute'>0</span>");

			if (DurationLimitHits.Count > 0) 
				html.Add($"<strong class='wv-negative'>{DurationLimitHits.Count}</strong>");
			else
				html.Add($"<span class='wv-mute'>0</span>");

			return String.Join("<span class='wv-mute'> / </span>", html);
		}
	}
	public WvSnapshotMethodComparisonData MethodComparison { get; set; } = new();
	public WvSnapshotMemoryComparisonData MemoryComparison { get; set; } = new();
	public List<WvTraceSessionLimitHit> MemoryTotalLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.MemoryTotal).ToList(); }
	public List<WvTraceSessionLimitHit> MemoryDeltaLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.MemoryDelta).ToList(); }
	public List<WvTraceSessionLimitHit> MethodCallsLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.CallCount).ToList(); }
	public List<WvTraceSessionLimitHit> DurationLimitHits { get => LimitHits.Where(x => x.Type == WvTraceSessionLimitType.Duration).ToList(); }

	public bool ModuleMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((Module ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
			return true;

		return false;
	}

	public bool ComponentMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((Component ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
			return true;

		return false;
	}

	public bool MethodMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((Method ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
			return true;

		return false;
	}

	public bool CallsMatches(WvTraceModalCallsFilter? filter)
	{
		if (filter is null) return true;
		if (TraceList is null) return false;

		if (filter == WvTraceModalCallsFilter.LessThan5 && TraceList.Count <= 5) return true;
		if (filter == WvTraceModalCallsFilter.FiveTo42 && TraceList.Count > 5 && TraceList.Count <= 42) return true;
		if (filter == WvTraceModalCallsFilter.MoreThan42 && TraceList.Count > 42) return true;

		return false;
	}

	public bool MemoryMatches(WvTraceModalMemoryFilter? filter)
	{
		if (filter is null) return true;
		if (LastMemoryKB is null) return false;
		if (filter == WvTraceModalMemoryFilter.LessThan5 && LastMemoryKB.Value <= 5) return true;
		if (filter == WvTraceModalMemoryFilter.FiveTo42 && LastMemoryKB.Value > 5 && LastMemoryKB.Value <= 42) return true;
		if (filter == WvTraceModalMemoryFilter.MoreThan42 && LastMemoryKB.Value > 42) return true;

		return false;
	}

	public bool DurationMatches(WvTraceModalDurationFilter? filter)
	{
		if (filter is null) return true;
		if (LastDurationMS is null) return false;
		if (filter == WvTraceModalDurationFilter.LessThan5 && LastDurationMS.Value <= 5) return true;
		if (filter == WvTraceModalDurationFilter.FiveTo42 && LastDurationMS.Value > 5 && LastDurationMS.Value <= 42) return true;
		if (filter == WvTraceModalDurationFilter.MoreThan42 && LastDurationMS.Value > 42) return true;

		return false;
	}

	public bool LimitMatches(WvTraceModalLimitsFilter? filter)
	{
		if (filter is null) return true;
		if (LimitHits is null) return false;
		if (filter == WvTraceModalLimitsFilter.ZeroLimitHits && LimitHits.Count == 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedCallLimit && MethodCallsLimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedTotalMemory && MemoryTotalLimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedMemoryDelta && MemoryDeltaLimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedDuration && DurationLimitHits.Count > 0) return true;


		return false;
	}
}

