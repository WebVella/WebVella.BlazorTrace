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
	public static bool IsMethodMuted(this string? methodName, string? moduleName, string? componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Method:
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.MethodInModule:
					if (muteTrace.Module.ProcessForMatch() != moduleName.ProcessForMatch()) break;
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.MethodInComponent:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.MethodInComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() != instanceTag.ProcessForMatch()) break;
					if (muteTrace.Method.ProcessForMatch() == methodName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}
	public static bool IsSignalMuted(this string? signalName, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Signal:
					if (muteTrace.Signal.ProcessForMatch() == signalName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}

	public static List<WvTraceSessionMethodTrace> GetUnmuted(this List<WvTraceSessionMethodTrace> traceList, string moduleName, string componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		var result = new List<WvTraceSessionMethodTrace>();
		foreach (var item in traceList)
		{
			if (item.IsMuted(
			moduleName: moduleName,
			componentFullName: componentFullName,
			instanceTag: instanceTag,
			muteList: muteList)) continue;
			item.OnEnterMemoryInfo = item.OnEnterMemoryInfo.ProcessMemoryInfoList(
				moduleName: moduleName,
				componentFullName: componentFullName,
				instanceTag: instanceTag,
				muteList: muteList
			);
			item.OnEnterMemoryBytes = item.OnEnterMemoryInfo?.Sum(x => x.Size);
			item.OnExitMemoryInfo = item.OnExitMemoryInfo.ProcessMemoryInfoList(
				moduleName: moduleName,
				componentFullName: componentFullName,
				instanceTag: instanceTag,
				muteList: muteList
			);
			item.OnExitMemoryBytes = item.OnExitMemoryInfo?.Sum(x => x.Size);
			result.Add(item);
		}
		return result;
	}
	public static bool IsMuted(this WvTraceSessionMethodTrace trace, string? moduleName, string? componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.OnEnterCustomData:
					if (muteTrace.OnEnterCustomData.ProcessForMatch() == trace.OnEnterCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnEnterCustomDataInModule:
					if (muteTrace.Module.ProcessForMatch() != moduleName.ProcessForMatch()) break;
					if (muteTrace.OnEnterCustomData.ProcessForMatch() == trace.OnEnterCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnEnterCustomDataInComponent:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.OnEnterCustomData.ProcessForMatch() == trace.OnEnterCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnEnterCustomDataInComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() != instanceTag.ProcessForMatch()) break;
					if (muteTrace.OnEnterCustomData.ProcessForMatch() == trace.OnEnterCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnExitCustomData:
					if (muteTrace.OnExitCustomData.ProcessForMatch() == trace.OnExitCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnExitCustomDataInModule:
					if (muteTrace.Module.ProcessForMatch() != moduleName.ProcessForMatch()) break;
					if (muteTrace.OnExitCustomData.ProcessForMatch() == trace.OnExitCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnExitCustomDataInComponent:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.OnExitCustomData.ProcessForMatch() == trace.OnExitCustomData.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.OnExitCustomDataInComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() != instanceTag.ProcessForMatch()) break;
					if (muteTrace.OnExitCustomData.ProcessForMatch() == trace.OnExitCustomData.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}

		return false;
	}

	public static List<WvTraceSessionSignalTrace> GetUnmuted(this List<WvTraceSessionSignalTrace> traceList, List<WvTraceMute> muteList)
	{
		var result = new List<WvTraceSessionSignalTrace>();
		foreach (var item in traceList)
		{
			if (item.IsMuted(
			muteList: muteList)) continue;
			result.Add(item);
		}
		return result;
	}

	public static bool IsMuted(this WvTraceSessionSignalTrace trace, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.SignalInModule:
					if (muteTrace.Module.ProcessForMatch() == trace.ModuleName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.SignalInComponent:
					if (muteTrace.ComponentFullName.ProcessForMatch() == trace.ComponentFullName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.SignalInComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != trace.ComponentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() == trace.InstanceTag.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}

		return false;
	}

	public static List<WvTraceMemoryInfo>? ProcessMemoryInfoList(this List<WvTraceMemoryInfo>? memoryInfoList, string moduleName, string componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		if (memoryInfoList is null) return null;
		var newList = new List<WvTraceMemoryInfo>();
		foreach (var memInfo in memoryInfoList)
		{
			if (memInfo.IsMuted(
				moduleName: moduleName,
				componentFullName: componentFullName,
				instanceTag: instanceTag,
				muteList: muteList
			)) continue;
			newList.Add(memInfo);
		}
		return newList;
	}

	public static bool IsMuted(this WvTraceMemoryInfo memInfo, string? moduleName, string? componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Field:
					if (muteTrace.Field.ProcessForMatch() == memInfo.FieldName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.FieldInModule:
					if (muteTrace.Module.ProcessForMatch() != moduleName.ProcessForMatch()) break;
					if (muteTrace.Field.ProcessForMatch() == memInfo.FieldName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.FieldInComponent:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.Field.ProcessForMatch() == memInfo.FieldName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.FieldInComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() != instanceTag.ProcessForMatch()) break;
					if (muteTrace.Field.ProcessForMatch() == memInfo.FieldName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.FieldInAssembly:
					if (muteTrace.Assembly.ProcessForMatch() != memInfo.AssemblyName.ProcessForMatch()) break;
					if (muteTrace.Field.ProcessForMatch() == memInfo.FieldName.ProcessForMatch()) return true;
					break;
				case WvTraceMuteType.Assembly:
					if (muteTrace.Assembly.ProcessForMatch() == memInfo.AssemblyName.ProcessForMatch()) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}
	public static bool IsMuted(this WvMethodTraceRow row, List<WvTraceMute> muteList, List<string> pins)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.PinnedMethods:
					if (pins.Contains(row.Id)) return true;
					break;
				case WvTraceMuteType.NotPinnedMethods:
					if (!pins.Contains(row.Id)) return true;
					break;
				default:
					break;
			}
		}

		return false;
	}
	public static string ProcessForMatch(this string? stringValue)
	{
		if (String.IsNullOrWhiteSpace(stringValue)) return String.Empty;
		return stringValue;
	}

	public static List<WvTraceSessionLimitHit> GetUnmuted(this List<WvTraceSessionLimitHit> limitHits,
		WvMethodTraceRow row, List<WvTraceMute> muteList)
	{
		var result = new List<WvTraceSessionLimitHit>();
		foreach (var item in limitHits)
		{
			if (item.IsMuted(
			moduleName: row.Module,
			componentFullName: row.ComponentFullName,
			instanceTag: row.InstanceTag,
			muteList: muteList)) continue;
			result.Add(item);
		}
		return result;
	}

	public static bool IsMuted(this WvTraceSessionLimitHit limitHit,
		string? moduleName, string? componentFullName, string? instanceTag, List<WvTraceMute> muteList)
	{
		foreach (var muteTrace in muteList)
		{
			switch (muteTrace.Type)
			{
				case WvTraceMuteType.Limit:
					if (muteTrace.LimitType == limitHit.Type) return true;
					break;
				case WvTraceMuteType.LimitInModule:
					if (muteTrace.Module.ProcessForMatch() != moduleName.ProcessForMatch()) break;
					if (muteTrace.LimitType == limitHit.Type) return true;
					break;
				case WvTraceMuteType.LimitInComponent:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.LimitType == limitHit.Type) return true;
					break;
				case WvTraceMuteType.LimitInComponentInstance:
					if (muteTrace.ComponentFullName.ProcessForMatch() != componentFullName.ProcessForMatch()) break;
					if (muteTrace.InstanceTag.ProcessForMatch() != instanceTag.ProcessForMatch()) break;
					if (muteTrace.LimitType == limitHit.Type) return true;
					break;
				default:
					break;
			}
		}
		return false;
	}
}
