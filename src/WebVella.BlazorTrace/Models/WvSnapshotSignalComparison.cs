using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvSnapshotSignalComparison
{
	public WvTraceSessionSignal PrimarySnapshotSignal { get; set; } = default!;
	public WvTraceSessionSignal SecondarySnapshotSignal { get; set; } = default!;
	public WvSnapshotSignalComparisonData ComparisonData { get; set; } = new();
}

public class WvSnapshotSignalComparisonData
{
	public int TraceListChange { get; set; }
	public string TraceListChangeHtml
	{
		get
		{
			if (TraceListChange == 0) return $"<span class='wv-mute'>=</span>";
			else if (TraceListChange < 0)
			{
				return $"<span class='wv-negative'>{TraceListChange}</span>";
			}
			return $"<span class='wv-positive'>+{TraceListChange}</span>";
		}
	}
}
