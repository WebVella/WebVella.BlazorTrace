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
			if (TraceListChange == 0) return $"<span class='wv-mute'>=</span>";
			else if (TraceListChange < 0)
			{
				return $"<span class='wv-negative'>{TraceListChange}</span>";
			}
			return $"<span class='wv-positive'>+{TraceListChange}</span>";
		}
	}
	public long? LastDurationChangeMS { get; set; }
	public string LastDurationChangeMSHtml
	{
		get
		{
			if (LastDurationChangeMS == 0) return $"<span class='wv-mute'>=</span>";
			else if (LastDurationChangeMS < 0)
			{
				return $"<span class='wv-negative'>{LastDurationChangeMS}</span>";
			}
			return $"<span class='wv-positive'>+{LastDurationChangeMS}</span>";
		}
	}
}
