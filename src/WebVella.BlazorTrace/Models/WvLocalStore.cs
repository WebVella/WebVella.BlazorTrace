using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvLocalStore
{
	public List<string> Pins { get; set; } = new();
	public List<WvTraceMute> MutedTraces { get; set; } = new();
	public WvTraceModalRequest? LastModalRequest { get; set; } = null;
	public List<WvSnapshot> Snapshots { get; set; } = new();
}

public class WvSnapshot
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTimeOffset CreatedOn { get; set; } = default!;
	public string Name { get; set; } = string.Empty;
	public Dictionary<string, WvTraceSessionModule> ModuleDict { get; set; } = new();
	public Dictionary<string, WvTraceSessionSignal> SignalDict { get; set; } = new();
}

