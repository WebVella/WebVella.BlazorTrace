using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Utility;
public static partial class WvTraceUtility
{
	public static bool IsModuleMuted(this string? moduleName, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Module:
					if (muteTrace.Module.ProcessForMatch() == moduleName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static bool IsComponentMuted(this string? componentFullName, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Component:
					if (muteTrace.ComponentFullName.ProcessForMatch() == componentFullName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static bool IsComponentInstanceMuted(this string? instanceTag, string? componentFullName, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.ComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() == instanceTag.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static bool IsMethodMuted(this string? methodName, string moduleName, string componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Method:
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.MethodInModule:
					if(muteTrace.Module.ProcessForMatch() != moduleName.ProcessForMatch()) break;
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.MethodInComponent:
					if(muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.MethodInComponentInstance:
					if(muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if(muteTrace.InstanceTag.ProcessForMatch() != instanceTag.ProcessForMatch()) break;
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static List<WvTraceSessionMethodTrace> GetUnmuted(this List<WvTraceSessionMethodTrace> traceList, List<WvTraceMute> muteList)
	{
		var result = new List<WvTraceSessionMethodTrace>();
		foreach (var item in traceList)
		{
			if (item.IsMuted(muteList)) continue;
			item.OnEnterMemoryInfo = item.OnEnterMemoryInfo.ProcessMemoryInfoList(muteList);
			item.OnExitMemoryInfo = item.OnExitMemoryInfo.ProcessMemoryInfoList(muteList);

			result.Add(item);
		}
		return result;
	}

	public static List<WvTraceMemoryInfo>? ProcessMemoryInfoList(this List<WvTraceMemoryInfo>? memoryInfoList, List<WvTraceMute> muteList)
	{
		if (memoryInfoList is null) return null;
		var newList = new List<WvTraceMemoryInfo>();
		foreach (var memInfo in memoryInfoList)
		{
			if (memInfo.IsMuted(muteList)) continue;
			newList.Add(memInfo);
		}
		return newList;
	}

	public static bool IsMuted(this WvMethodTraceRow row, List<WvTraceMute> muteList, List<string> pins)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.PinnedMethods:
					if(pins.Contains(row.Id)) return true;
					break;
				case WvTraceMuteType.NotPinnedMethods:
					if(!pins.Contains(row.Id)) return true;
					break;
				default:
					break;
			}
		}

		return false;
	}
	public static bool IsMuted(this WvTraceSessionMethodTrace trace, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Module:

					break;
				default:
					break;
			}
		}

		return false;
	}
	public static bool IsMuted(this WvTraceMemoryInfo memInfo, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Field:
					if (muteTrace.Field.ProcessForMatch() == memInfo.FieldName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static string ProcessForMatch(this string? stringValue){ 
		if(String.IsNullOrWhiteSpace(stringValue)) return String.Empty;
		return stringValue;
	}
}
