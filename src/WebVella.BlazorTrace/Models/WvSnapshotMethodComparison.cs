using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvSnapshotMethodComparison
{
	public WvTraceSessionMethod PrimarySnapshotMethod { get; set; } = default!;
	public WvTraceSessionMethod SecondarySnapshotMethod { get; set; } = default!;
	public WvSnapshotMethodComparisonData ComparisonData { get; set; } = new();
}

public class WvSnapshotMethodComparisonData
{
	public int TraceListChange { get; set; }
	public string TraceListChangeHtml
	{
		get
		{
			if (TraceListChange == 0) return $"<span class='mute'>=</span>";
			else if (TraceListChange < 0)
			{
				return $"<span class='negative'>{TraceListChange}</span>";
			}
			return $"<span class='positive'>+{TraceListChange}</span>";
		}
	}
}
