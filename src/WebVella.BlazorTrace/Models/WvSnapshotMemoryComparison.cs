using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvSnapshotMemoryComparison
{
	public WvTraceSessionMethod PrimarySnapshotMethod { get; set; } = default!;
	public WvTraceSessionMethod SecondarySnapshotMethod { get; set; } = default!;
	public WvSnapshotMemoryComparisonData ComparisonData { get; set; } = new();
}

public class WvSnapshotMemoryComparisonData
{
	public long LastMemoryChangeBytes { get; set; }
	public string LastMemoryChangeKBHtml
	{
		get
		{
			if (LastMemoryChangeBytes == 0) return $"<span class='mute'>=</span>";
			else if (LastMemoryChangeBytes < 0)
			{
				return $"<span class='negative'>{LastMemoryChangeBytes.ToKilobytes()}KB</span>";
			}
			return $"<span class='positive'>+{LastMemoryChangeBytes.ToKilobytes()}KB</span>";
		}
	}
	public List<WvSnapshotMemoryComparisonDataField> Fields { get; set; } = new();
}


public class WvSnapshotMemoryComparisonDataField
{
	public string Id { get => WvTraceUtility.GetMemoryInfoId(AssemblyName, FieldName); }
	public string FieldName { get; set; } = String.Empty;
	public string AssemblyName { get; set; } = String.Empty;
	public long? PrimarySnapshotBytes { get; set; }
	public long? SecondarySnapshotBytes { get; set; }
	public long ChangeBytes => (SecondarySnapshotBytes ?? 0) - (PrimarySnapshotBytes ?? 0);
	public string ChangeKBHtml
	{
		get
		{
			if (ChangeBytes == 0) return $"<span class='mute'>=</span>";
			else if (ChangeBytes < 0)
			{
				return $"<span class='negative'>{ChangeBytes.ToKilobytes()}KB</span>";
			}
			return $"<span class='positive'>+{ChangeBytes.ToKilobytes()}KB</span>";
		}
	}

}