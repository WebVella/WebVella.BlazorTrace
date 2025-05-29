using System.Reflection;

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
	public List<string> MemoryTraceIncludeAssemblyStartWithList { get; set; } = new();

	/// <summary>
	/// Assemblies name start strings that you want to exclude from memory trace. 
	/// Some framework assemblies are excluded by default. Set the <ExcludeDefaultFrameworkAssemblies> to false if you need them
	/// </summary>
	public List<string> MemoryTraceExcludeAssemblyStartWithList { get; set; } = new();

	/// <summary>
	/// By default some framework assemblies are excluded for convenience.
	/// </summary>
	public bool ExcludeDefaultFrameworkAssemblies { get; set; } = true;

	/// <summary>
	/// Field Names that are containing one of the strings in this list will be always included in the trace 
	/// </summary>
	public List<string> MemoryTraceIncludeFieldNameContainsFromList { get; set; } = new();

	/// <summary>
	/// Field Names that are containing one of the strings in this list will be excluded from the trace 
	/// Some field names are excluded by default. Set the <ExcludeDefaultFieldNames> to false if you need them
	/// </summary>
	public List<string> MemoryTraceExcludeFieldNameContainsFromList { get; set; } = new();

	/// <summary>
	/// By default some field names that are excluded for convenience.
	/// </summary>
	public bool ExcludeDefaultFieldNames { get; set; } = true;
}

