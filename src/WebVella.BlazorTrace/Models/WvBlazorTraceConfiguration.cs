using System.Reflection;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public class WvBlazorTraceConfiguration
{
	/// <summary>
	/// By default the tracing is enabled, but from this property you can stop the tracing globally
	/// </summary>
	public bool EnableTracing { get; set; } = true;

	/// <summary>
	/// For easier tool development
	/// </summary>
	public bool AutoShowModal { get; set; } = false;

	/// <summary>
	/// By default F1 will open the trace modal
	/// </summary>
	public bool EnableF1Shortcut { get; set; } = true;

	/// <summary>
	/// Assemblies name start strings that you want to always include in memory trace. 
	/// </summary>
	public List<string> MemoryIncludeAssemblyList { get; set; } = new();

	/// <summary>
	/// Assemblies name start strings that you want to exclude from memory trace. 
	/// Some framework assemblies are excluded by default. Set the <ExcludeDefaultFrameworkAssemblies> to false if you need them
	/// </summary>
	public List<string> MemoryExcludeAssemblyList { get; set; } = new();

	/// <summary>
	/// By default some framework assemblies are excluded for convenience.
	/// </summary>
	public bool ExcludeDefaultAssemblies { get; set; } = true;

	/// <summary>
	/// Field Names that are containing one of the strings in this list will be always included in the trace 
	/// </summary>
	public List<string> MemoryIncludeFieldNameList { get; set; } = new();

	/// <summary>
	/// Field Names that are containing one of the strings in this list will be excluded from the trace 
	/// Some field names are excluded by default. Set the <ExcludeDefaultFieldNames> to false if you need them
	/// </summary>
	public List<string> MemoryExcludeFieldNameList { get; set; } = new();

	/// <summary>
	/// By default some field names that are excluded for convenience.
	/// </summary>
	public bool ExcludeDefaultFieldNames { get; set; } = true;

	/// <summary>
	/// Override the default Trace method options
	/// </summary>
	public WvTraceMethodOptions? DefaultTraceMethodOptions { get; set; } = null;

	/// <summary>
	/// Override the default Trace signal options
	/// </summary>
	public WvTraceSignalOptions? DefaultTraceSignalOptions { get; set; } = null;
}

