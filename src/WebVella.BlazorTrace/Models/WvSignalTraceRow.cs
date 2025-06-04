using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvSignalTraceRow
{
	[JsonIgnore]
	public string Id { get => WvModalUtility.GenerateSignalHash(SignalName); }
	public string? SignalName { get; set; }
	public bool IsPinned { get; set; } = false;
	public List<WvTraceSessionSignalTrace> TraceList { get; set; } = new();
	public List<WvTraceSessionLimitHit> LimitHits { get; set; } = new();
	[JsonIgnore]
	public string LimitsHint
	{
		get
		{
			var hints = new List<string>();
			if (LimitHits.Count > 0) hints.Add($"{LimitHits.Count} calls");

			if (hints.Count == 0) return "no limits are hit";

			return String.Join("; ", hints);
		}
	}
	[JsonIgnore]
	public string LimitsHtml
	{
		get
		{
			var html = new List<string>();
			if (LimitHits.Count > 0)
				html.Add($"<strong class='wv-negative'>{LimitHits.Count}</strong>");
			else
				html.Add($"<span class='wv-mute'>0</span>");

			return String.Join("<span class='wv-mute'> / </span>", html);
		}
	}
	public WvSnapshotSignalComparisonData SignalComparison { get; set; } = new();
	public bool SignalNameMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;

		if ((SignalName ?? String.Empty).ToLowerInvariant().Contains(search.Trim().ToLowerInvariant()))
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
		if (filter == WvTraceModalCallsFilter.PositiveDelta && SignalComparison.TraceListChange > 0) return true;
		if (filter == WvTraceModalCallsFilter.NegativeDelta && SignalComparison.TraceListChange < 0) return true;
		if (filter == WvTraceModalCallsFilter.NoDelta && SignalComparison.TraceListChange == 0) return true;

		return false;
	}


	public bool LimitMatches(WvTraceModalLimitsFilter? filter)
	{
		if (filter is null) return true;
		if (LimitHits is null) return false;
		if (filter == WvTraceModalLimitsFilter.HasLimitHits && LimitHits.Count > 0) return true;
		if (filter == WvTraceModalLimitsFilter.ZeroLimitHits && LimitHits.Count == 0) return true;
		if (filter == WvTraceModalLimitsFilter.ExceedCallLimit && LimitHits.Count > 0) return true;

		return false;
	}
}

