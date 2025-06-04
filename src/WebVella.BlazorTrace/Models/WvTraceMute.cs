using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceMute
{
	[JsonIgnore]
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
	public string? Assembly { get; set; }
	public string? OnEnterCustomData { get; set; }
	public string? OnExitCustomData { get; set; }
	public bool? IsPinned { get; set; }
	public WvTraceSessionLimitType? LimitType { get; set; }
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
	public WvTraceMute(WvTraceMuteType type, WvMethodTraceRow row, WvSnapshotMemoryComparisonDataField dataField)
	{
		Type = type;
		switch (type)
		{
			case WvTraceMuteType.Field:
				Field = dataField.FieldName;
				break;
			case WvTraceMuteType.FieldInModule:
				Module = row.Module;
				Field = dataField.FieldName;
				break;
			case WvTraceMuteType.FieldInComponent:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				Field = dataField.FieldName;
				break;
			case WvTraceMuteType.FieldInComponentInstance:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				InstanceTag = row.InstanceTag;
				Field = dataField.FieldName;
				break;
			case WvTraceMuteType.FieldInAssembly:
				Assembly = dataField.AssemblyName;
				Field = dataField.FieldName;
				break;
			case WvTraceMuteType.Assembly:
				Assembly = dataField.AssemblyName;
				break;
			default:
				break;
		}
	}
	public WvTraceMute(WvTraceMuteType type, WvMethodTraceRow row, string customData)
	{
		Type = type;
		switch (type)
		{
			case WvTraceMuteType.OnEnterCustomData:
				OnEnterCustomData = customData;
				break;
			case WvTraceMuteType.OnEnterCustomDataInModule:
				Module = row.Module;
				OnEnterCustomData = customData;
				break;
			case WvTraceMuteType.OnEnterCustomDataInComponent:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				OnEnterCustomData = customData;
				break;
			case WvTraceMuteType.OnEnterCustomDataInComponentInstance:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				InstanceTag = row.InstanceTag;
				OnEnterCustomData = customData;
				break;
			case WvTraceMuteType.OnExitCustomData:
				OnExitCustomData = customData;
				break;
			case WvTraceMuteType.OnExitCustomDataInModule:
				Module = row.Module;
				OnExitCustomData = customData;
				break;
			case WvTraceMuteType.OnExitCustomDataInComponent:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				OnExitCustomData = customData;
				break;
			case WvTraceMuteType.OnExitCustomDataInComponentInstance:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				InstanceTag = row.InstanceTag;
				OnExitCustomData = customData;
				break;
			default:
				break;
		}
	}
	public WvTraceMute(WvTraceMuteType type, WvMethodTraceRow row, WvTraceSessionLimitHit dataField)
	{
		Type = type;
		switch (type)
		{
			case WvTraceMuteType.Limit:
				LimitType = dataField.Type;
				break;
			case WvTraceMuteType.LimitInModule:
				Module = row.Module;
				LimitType = dataField.Type;
				break;
			case WvTraceMuteType.LimitInComponent:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				LimitType = dataField.Type;
				break;
			case WvTraceMuteType.LimitInComponentInstance:
				ComponentName = row.Component;
				ComponentFullName = row.ComponentFullName;
				InstanceTag = row.InstanceTag;
				LimitType = dataField.Type;
				break;
			default:
				break;
		}
	}
	public WvTraceMute(WvTraceMuteType type, WvSignalTraceRow row, WvTraceSessionSignalTrace trace)
	{
		Type = type;
		switch (type)
		{
			case WvTraceMuteType.SignalInModule:
				Module = trace.ModuleName;
				Signal = row.SignalName;
				break;
			case WvTraceMuteType.SignalInComponent:
				ComponentName = trace.ComponentName;
				ComponentFullName = trace.ComponentFullName;
				Signal = row.SignalName;
				break;
			case WvTraceMuteType.SignalInComponentInstance:
				ComponentName = trace.ComponentName;
				ComponentFullName = trace.ComponentFullName;
				InstanceTag = trace.InstanceTag;
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
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(OnEnterCustomData))
			return true;

		if ((OnEnterCustomData ?? String.Empty).ToLowerInvariant().Contains(searchLower))
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
		if (Type.WvBTToDescriptionString().ToLowerInvariant().Contains(searchLower))
			return true;
		return false;
	}
}
