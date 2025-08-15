using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public record WvMethodTraceCSVRow
{
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? ComponentFullName { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }

	//Trace
	public string? TraceId { get; set; }
	public string? EnteredOn { get; set; }
	public string? ExitedOn { get; set; }
	public string? DurationMs { get; set; }
	public string? OnEnterMemory { get; set; }
	public string? OnExitMemory { get; set; }
	public string? MemoryDelta { get; set; }
	public string? OnEnterCustomData { get; set; }
	public string? OnExitCustomData { get; set; }
	
}
