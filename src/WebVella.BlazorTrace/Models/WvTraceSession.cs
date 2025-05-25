using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceSessionModule
{
	public Dictionary<string, WvTraceSessionComponent> ComponentDict { get; set; } = new();
}

public class WvTraceSessionComponent
{
	public string? Name { get; set; }
	public List<WvTraceSessionComponentTaggedInstance> TaggedInstances { get; set; } = new();
}

public class WvTraceSessionComponentTaggedInstance
{
	public string? Tag { get; set; }
	public WvTraceSessionMethod OnInitialized { get; set; } = new() { Name = "OnInitialized" };
	public WvTraceSessionMethod OnParameterSet { get; set; } = new() { Name = "OnParameterSet" };
	public WvTraceSessionMethod OnAfterRender { get; set; } = new() { Name = "OnAfterRender" };
	public WvTraceSessionMethod ShouldRender { get; set; } = new() { Name = "ShouldRender" };
	public WvTraceSessionMethod Dispose { get; set; } = new() { Name = "Dispose" };
	public List<WvTraceSessionMethod> OtherMethods { get; set; } = new();
	public List<WvTraceSessionSignal> Signals { get; set; } = new();

	public List<WvTraceSessionMethod> MethodsTotal(bool includeNotCalled = false)
	{
		var methods = new List<WvTraceSessionMethod>();
		methods.Add(OnInitialized);
		methods.Add(OnParameterSet);
		methods.Add(OnAfterRender);
		methods.Add(ShouldRender);
		methods.Add(Dispose);
		methods.AddRange(OtherMethods);
		if (includeNotCalled)
			return methods;

		var calledMethods = new List<WvTraceSessionMethod>();
		foreach (var method in methods)
		{
			if (method.MaxCallsCount > 0)
				calledMethods.Add(method);
		}
		return calledMethods;
	}
}

public class WvTraceSessionMethod
{
	public string? Name { get; set; } = null;
	public bool IsBookmarked { get; set; } = false;

	[JsonIgnore]
	public long? MinDurationMs { get => this.GetMinDuration(); }
	[JsonIgnore]
	public long? MaxDurationMs { get => this.GetMaxDuration(); }
	[JsonIgnore]
	public long? LastDurationMs { get => this.GetAverageDuration(); }
	[JsonIgnore]
	public long? OnEnterMinMemoryBytes { get => this.GetMinMemory(isOnEnter: true); }
	[JsonIgnore]
	public long? OnEnterMaxMemoryBytes { get => this.GetMaxMemory(isOnEnter: true); }
	[JsonIgnore]
	public long? OnExitMinMemoryBytes { get => this.GetMinMemory(isOnEnter: false); }
	[JsonIgnore]
	public long? OnExitMaxMemoryBytes { get => this.GetMaxMemory(isOnEnter: false); }
	[JsonIgnore]
	public long? LastMemoryBytes { get => this.GetLastMemory().Item1; }
	[JsonIgnore]
	public List<WvTraceMemoryInfo>? LastMemoryInfo { get => this.GetLastMemory().Item2; }
	[JsonIgnore]
	public long? MinMemoryDeltaBytes { get => this.GetMinMemoryDelta(); }
	[JsonIgnore]
	public long? MaxMemoryDeltaBytes { get => this.GetMaxMemoryDelta(); }
	[JsonIgnore]
	public long OnEnterCallsCount { get => this.GetOnEnterCallCount(); }
	[JsonIgnore]
	public long OnExitCallsCount { get => this.GetOnExitCallsCount(); }
	[JsonIgnore]
	public long MaxCallsCount { get => this.GetMaxCallsCount(); }
	[JsonIgnore]
	public long CompletedCallsCount { get => this.CompletedCallsCount(); }
	public List<WvTraceSessionTrace> TraceList { get; set; } = new();
	public WvTraceSessionTrace? LastExitedTrace
	{
		get => TraceList.Where(x => x.ExitedOn is not null).OrderByDescending(x => x.ExitedOn).LastOrDefault();
	}
	public List<WvTraceSessionLimitHit> LimitHits { get; set; } = new();
	public string GenerateHash(string moduleName, string componentFullname, string? tag)
	{
		return WvModalUtility.GenerateHash(moduleName, componentFullname, tag, Name);
	}
}


public class WvTraceSessionTrace
{
	public DateTimeOffset? EnteredOn { get; set; } = null;
	public DateTimeOffset? ExitedOn { get; set; } = null;
	public long? DurationMs { get => this.GetDurationMS(); }
	public long? OnEnterMemoryBytes { get; set; } = null;
	public List<WvTraceMemoryInfo>? OnEnterMemoryInfo { get; set; } = null;
	public long? OnExitMemoryBytes { get; set; } = null;
	public List<WvTraceMemoryInfo>? OnExitMemoryInfo { get; set; } = null;
	public bool? OnEnterFirstRender { get; set; } = null;
	public bool? OnExitFirstRender { get; set; } = null;
	public string? OnEnterCallTag { get; set; } = null;
	public string? OnExitCallTag { get; set; } = null;
}

public class WvTraceSessionSignal
{
	public DateTimeOffset SendOn { get; set; } = default!;
	public string? Payload { get; set; } = null;
}

public class WvTraceSessionLimitHit
{
	public WvTraceSessionLimitType Type { get; set; }
	public long Limit { get; set; } = 0;
	public long Actual { get; set; } = 0;
}

public enum WvTraceSessionLimitType
{
	MethodCalls = 0,
	Duration = 1,
	MemoryTotal = 2,
	MemoryDelta = 3
}