﻿using System.Reflection;

namespace WebVella.BlazorTrace;
public class WvBlazorTraceConfiguration
{
	/// <summary>
	/// By default the tracing is enabled, but from this property you can stop the tracing globally
	/// </summary>
	public bool EnableTracing { get; set; } = true;

	/// <summary>
	/// By default F1 will open the trace modal
	/// </summary>
	public bool EnableF1Shortcut { get; set; } = true;

	/// <summary>
	/// Assemblies name start strings that you want to always include in memory trace. 
	/// </summary>
	public List<string> MemoryTraceIncludedAssemblyStartWithList { get; set; } = new();

	/// <summary>
	/// Assemblies name start strings that you want to exclude from memory trace. 
	/// Some framework assemblies are excluded by default. Set the ExcludedFrameworkAssembliesByDefault to false if you need them
	/// </summary>
	public List<string> MemoryTraceExcludedAssemblyStartWithList { get; set; } = new();

	/// <summary>
	/// By default some framework assemblies are excluded for convenience.
	/// </summary>
	public bool ExcludedFrameworkAssembliesByDefault { get; set; } = true;
}

