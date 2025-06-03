using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceMute
{
	public string Id { get => WvModalUtility.GenerateTraceMuteHash(this); }
	public WvTraceMuteType Type { get; set; } = WvTraceMuteType.Module;
	public string? Module { get; set; }
	public string? ComponentFullName { get; set; }
	/// <summary>
	/// NOTE: Component name is just for presentational purposes. 
	/// ComponentFullName is used in the match process.
	/// </summary>
	public string? ComponentName { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }
	public string? Signal { get; set; }
	public string? Field { get; set; }
	public string? CustomData { get; set; }
	public bool? IsPinned { get; set; }
	public WvTraceMute()
	{
	}

	public WvTraceMute(WvTraceMuteType type, WvMethodTraceRow row)
	{
		Type = type;
		switch (type)
		{
			case WvTraceMuteType.Module:
				Module = row.Module;
				break;
			case WvTraceMuteType.Component:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				break;
			case WvTraceMuteType.ComponentInstance:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				InstanceTag = row.InstanceTag;
				break;
			case WvTraceMuteType.Method:
				Method = row.Method;
				break;
			case WvTraceMuteType.MethodInModule:
				Module = row.Module;
				Method = row.Method;
				break;
			case WvTraceMuteType.MethodInComponent:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				Method = row.Method;
				break;
			case WvTraceMuteType.MethodInComponentInstance:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				InstanceTag = row.InstanceTag;
				Method = row.Method;
				break;
			case WvTraceMuteType.NotPinnedMethods:
			case WvTraceMuteType.NotPinnedSignals:
				IsPinned = false;
				break;
			case WvTraceMuteType.PinnedMethods:
			case WvTraceMuteType.PinnedSignals:
				IsPinned = true;
				break;
			default:
				break;
		}
	}
	public WvTraceMute(WvTraceMuteType type, WvSignalTraceRow row)
	{
		Type = type;
		switch (type)
		{
			case WvTraceMuteType.NotPinnedMethods:
				IsPinned = false;
				break;
			case WvTraceMuteType.PinnedMethods:
				IsPinned = true;
				break;
			case WvTraceMuteType.Signal:
				Signal = row.SignalName;
				break;
			default:
				break;
		}
	}
	public bool ModuleMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(Module))
			return true;
		if ((Module ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool ComponentMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(ComponentName))
			return true;
		if ((ComponentName ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool InstanceTagMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(InstanceTag))
			return true;
		if ((InstanceTag ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool MethodMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(Method))
			return true;
		if ((Method ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool SignalMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(Signal))
			return true;
		if ((Signal ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool FieldMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(Field))
			return true;
		if ((Field ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool CustomDataMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(CustomData))
			return true;

		if ((CustomData ?? String.Empty).ToLowerInvariant().Contains(searchLower))
			return true;

		return false;
	}
	public bool IsPinnedMatches(string? search)
	{
		if (!(Type == WvTraceMuteType.PinnedMethods
			|| Type == WvTraceMuteType.NotPinnedMethods
			|| Type == WvTraceMuteType.PinnedSignals
			|| Type == WvTraceMuteType.NotPinnedSignals
			)) return true;

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && IsPinned is null)
			return true;

		if ("true".Contains(searchLower) && IsPinned.HasValue && IsPinned.Value)
			return true;

		if ("false".Contains(searchLower) && IsPinned.HasValue && !IsPinned.Value)
			return true;

		return false;
	}
	public bool IsTypeMatches(string? search)
	{

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if(Type.WvBTToDescriptionString().ToLowerInvariant().Contains(searchLower))
			return true;
		return false;
	}
}
