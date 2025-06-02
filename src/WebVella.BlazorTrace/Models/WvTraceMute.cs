using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace.Models;
public class WvTraceMute
{
	public string Id { get => WvModalUtility.GenerateTraceMuteHash(this);}
	public WvTraceMuteType Type { get; set; } = WvTraceMuteType.Module;
	public string? Module { get; set; }
	public string? Component { get; set; }
	public string? InstanceTag { get; set; }
	public string? Method { get; set; }
	public string? Signal { get; set; }
	public string? Field { get; set; }
	public string? CustomData { get; set; }
	public bool? IsBookmarkedMethod { get; set; }
	public bool? IsBookmarkedSignal { get; set; }

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
				IsBookmarkedMethod = false;
				break;
			case WvTraceMuteType.BookmarkedMethods:
				IsBookmarkedMethod = true;
				break;
			case WvTraceMuteType.NotBookmarkedSignals:
				IsBookmarkedSignal = false;
				break;
			case WvTraceMuteType.BookmarkedSignals:
				IsBookmarkedSignal = true;
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
				IsBookmarkedMethod = false;
				break;
			case WvTraceMuteType.BookmarkedMethods:
				IsBookmarkedMethod = true;
				break;
			case WvTraceMuteType.Signal:
				Signal = row.SignalName;
				break;
			default:
				break;
		}
	}

}
