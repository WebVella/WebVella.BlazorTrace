using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvConsoleLog
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTimeOffset CreatedOn { get; set; }
	public string Type { get; set; } = default!;
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }
	public string? Signal { get; set; }

}

