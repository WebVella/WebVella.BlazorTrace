using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvLocalStore
{
	public List<string> Pins { get; set; } = new();
	public List<WvTraceMute> MutedTraces { get; set; } = new();
	public WvTraceModalRequest? LastModalRequest { get; set; } = null;
	public List<WvSnapshotStore> Snapshots { get; set; } = new();
}

public class WvSnapshotStore
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTimeOffset CreatedOn { get; set; } = default!;
	public string Name { get; set; } = string.Empty;
	public string? CompressedModuleDict { get; set; } = null;
	public string? CompressedSignalDict { get; set; } = null;
}

