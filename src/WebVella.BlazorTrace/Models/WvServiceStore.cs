using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvServiceStore
{
	public Dictionary<string, WvTraceSessionModule> ModuleDict { get; set; } = new();
	public Dictionary<string, WvTraceSessionSignal> SignalDict { get; set; } = new();
}



