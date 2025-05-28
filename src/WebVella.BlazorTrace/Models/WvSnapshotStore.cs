using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvSnapshotStore
{
	public List<string> Bookmarked { get; set; } = new();
	public WvTraceModalRequest? LastModalRequest { get; set; } = null;
	public List<WvSnapshot> Snapshots { get; set; } = new();
}

public class WvSnapshot
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTime CreatedOn { get; set; } = default!;
	public string Name { get; set; } = string.Empty;
	public Dictionary<string, WvTraceSessionModule> ModuleDict { get; set; } = new();
}

