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
	public string? Component { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }
	public string? Signal { get; set; }
	public string? Field { get; set; }
	public string? CustomData { get; set; }
	public bool? IsBookmarked { get; set; }
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
				Component = row.Component;
				break;
			case WvTraceMuteType.ComponentInstance:
				Component = row.Component;
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
				Component = row.Component;
				Method = row.Method;
				break;
			case WvTraceMuteType.MethodInComponentInstance:
				Component = row.Component;
				InstanceTag = row.InstanceTag;
				Method = row.Method;
				break;
			case WvTraceMuteType.NotBookmarkedMethods:
			case WvTraceMuteType.NotBookmarkedSignals:
				IsBookmarked = false;
				break;
			case WvTraceMuteType.BookmarkedMethods:
			case WvTraceMuteType.BookmarkedSignals:
				IsBookmarked = true;
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
			case WvTraceMuteType.NotBookmarkedMethods:
				IsBookmarked = false;
				break;
			case WvTraceMuteType.BookmarkedMethods:
				IsBookmarked = true;
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
		if ("undefined".Contains(searchLower) && String.IsNullOrWhiteSpace(Component))
			return true;
		if ((Component ?? String.Empty).ToLowerInvariant().Contains(searchLower))
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
	public bool IsBookmarkedMatches(string? search)
	{
		if (!(Type == WvTraceMuteType.BookmarkedMethods
			|| Type == WvTraceMuteType.NotBookmarkedMethods
			|| Type == WvTraceMuteType.BookmarkedSignals
			|| Type == WvTraceMuteType.NotBookmarkedSignals
			)) return true;

		if (string.IsNullOrWhiteSpace(search)) return true;
		var searchLower = search.Trim().ToLowerInvariant();
		if ("undefined".Contains(searchLower) && IsBookmarked is null)
			return true;

		if ("true".Contains(searchLower) && IsBookmarked.HasValue && IsBookmarked.Value)
			return true;

		if ("false".Contains(searchLower) && IsBookmarked.HasValue && !IsBookmarked.Value)
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
