using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public record WvSignalTraceCSVRow
{
	public string? SignalName { get; set; }
	public string? SendOn { get; set; }
	public string? ModuleName { get; set; }
	public string? ComponentName { get; set; }
	public string? ComponentFullName { get; set; }
	public string? InstanceTag { get; set; }
	public string? MethodName { get; set; }
	public string? CustomData { get; set; }
}

