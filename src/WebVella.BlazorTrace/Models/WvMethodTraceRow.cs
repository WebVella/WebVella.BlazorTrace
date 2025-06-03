using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvMethodTraceRow
{
	public string Id { get => WvModalUtility.GenerateMethodHash(Module, ComponentFullName, InstanceTag, Method); }
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? ComponentFullName { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }
	public bool IsPinned { get; set; } = false;
	public List<WvTraceSessionMethodTrace> TraceList { get; set; } = new();
	public long? LastMemoryBytes { get; set; }
	public long? LastDurationMS { get; set; }
	public List<WvTraceSessionLimitHit> LimitHits { get; set; } = new();
	public string LimitsHint
	{
		get
		{
			var hints = new List<string>();
			if (MethodCallsLimitHits.Count > 0) hints.Add($"{MethodCallsLimitHits.Count} calls");
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

		if (filter == WvTraceModalCallsFilter.LessThan25 && TraceList.Count <= 25) return true;
		if (filter == WvTraceModalCallsFilter.TwentyFiveTo50 && TraceList.Count > 25 && TraceList.Count <= 50) return true;
		if (filter == WvTraceModalCallsFilter.MoreThan50 && TraceList.Count > 50) return true;
		if (filter == WvTraceModalCallsFilter.PositiveDelta && MethodComparison.TraceListChange > 0) return true;
		if (filter == WvTraceModalCallsFilter.NegativeDelta && MethodComparison.TraceListChange < 0) return true;
		if (filter == WvTraceModalCallsFilter.NoDelta && MethodComparison.TraceListChange == 0) return true;

		return false;
	}

	public bool MemoryMatches(WvTraceModalMemoryFilter? filter)
	{
		if (filter is null) return true;
		if (LastMemoryBytes is null) return false;
		if (filter == WvTraceModalMemoryFilter.LessThan50 && LastMemoryBytes <= 50 * 1024) return true;
		if (filter == WvTraceModalMemoryFilter.FiftyTo500 && LastMemoryBytes > 50 * 1024 && LastMemoryBytes <= 500 * 1024) return true;
		if (filter == WvTraceModalMemoryFilter.MoreThan500 && LastMemoryBytes > 500 * 1024) return true;
		if (filter == WvTraceModalMemoryFilter.PositiveDelta && MemoryComparison.LastMemoryChangeBytes > 0) return true;
		if (filter == WvTraceModalMemoryFilter.NegativeDelta && MemoryComparison.LastMemoryChangeBytes < 0) return true;
		if (filter == WvTraceModalMemoryFilter.NoDelta && MemoryComparison.LastMemoryChangeBytes == 0) return true;

		return false;
	}

	public bool DurationMatches(WvTraceModalDurationFilter? filter)
	{
		if (filter is null) return true;
		if (LastDurationMS is null) return false;
		if (filter == WvTraceModalDurationFilter.LessThan50 && LastDurationMS.Value <= 50) return true;
		if (filter == WvTraceModalDurationFilter.FiftyTo500 && LastDurationMS.Value > 50 && LastDurationMS.Value <= 500) return true;
		if (filter == WvTraceModalDurationFilter.MoreThan500 && LastDurationMS.Value > 500) return true;
		if (filter == WvTraceModalDurationFilter.PositiveDelta && MethodComparison.LastDurationChangeMS > 0) return true;
		if (filter == WvTraceModalDurationFilter.NegativeDelta && MethodComparison.LastDurationChangeMS < 0) return true;
		if (filter == WvTraceModalDurationFilter.NoDelta && MethodComparison.LastDurationChangeMS == 0) return true;

		return false;
	}

	public bool LimitMatches(WvTraceModalLimitsFilter? filter)
	{
		if (filter is null) return true;
		if (LimitHits is null) return false;
		if (filter == WvTraceModalLimitsFilter.HasLimitHits && LimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ZeroLimitHits && LimitHits.Count == 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedCallLimit && MethodCallsLimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedTotalMemory && MemoryTotalLimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedMemoryDelta && MemoryDeltaLimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedDuration && DurationLimitHits.Count > 0) return true;


		return false;
	}
}

