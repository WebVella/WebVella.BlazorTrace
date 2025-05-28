using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Models;
public class WvModuleUnionData
{
	public WvTraceSessionModule? Primary { get; set; }
	public WvTraceSessionModule? Secondary { get; set; }
	public Dictionary<string,WvComponentUnionData> ComponentDict { get; set; } = new();
	public Dictionary<string,WvSignalUnionData> SignalDict { get; set; } = new();
}


public class WvComponentUnionData
{
	public string? Name { get; set; }
	public WvTraceSessionComponent? Primary { get; set; }
	public WvTraceSessionComponent? Secondary { get; set; }
	public List<WvTaggedInstanceUnionData> TaggedInstances { get; set; } = new();
}

public class WvSignalUnionData
{
	public WvTraceSessionSignal? Primary { get; set; }
	public WvTraceSessionSignal? Secondary { get; set; }
}

public class WvTaggedInstanceUnionData
{
	public string? Tag { get; set; }
	public WvTraceSessionComponentTaggedInstance? Primary { get; set; }
	public WvTraceSessionComponentTaggedInstance? Secondary { get; set; }
	public Dictionary<string,WvMethodUnionData> MethodDict { get; set; } = new();
}

public class WvMethodUnionData
{
	public WvTraceSessionMethod? Primary { get; set; }
	public WvTraceSessionMethod? Secondary { get; set; }

}