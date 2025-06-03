using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace.Utility;
public static partial class WvTraceUtility
{
	public static string WvBTToKilobytesString(this long? bytes)
	{
		if (bytes == null) return "n/a";

		if (bytes < 0) return "err"; // Handle invalid input
		if (bytes == 0) return "0 KB";
		if (bytes <= 15) return bytes.ToString() + " bytes";
		const double kilobyteFactor = 1024.0;
		var kb = Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
		return kb.ToString() + " KB";
	}
	public static string WvBTToKilobytesString(this long bytes)
	{
		if (bytes < 0) return "err"; // Handle invalid input
		if (bytes == 0) return "0 KB";
		if (bytes <= 15) return bytes.ToString() + " bytes";
		const double kilobyteFactor = 1024.0;
		var kb = Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
		return kb.ToString() + " KB";
	}
	public static string WvBTGetMemoryInfoId(string assemblyFullName, string fieldName)
		=> $"{assemblyFullName}$$${fieldName}";
	public static string WvBTGetFirstRenderString(this bool? firstRender)
	{
		if (firstRender == null) return "n/a";
		if (!firstRender.Value) return "no";
		return "yes";
	}
	public static string WvBTToDurationMSString(this long? duration)
	{
		if (duration == null) return "n/a";
		return $"{duration.Value} ms";
	}
	public static string WvBTToDurationMSString(this long duration)
	{
		return $"{duration} ms";
	}
	public static string GetLimitValueAsString(this long limit, WvTraceSessionLimitType type)
	{
		switch (type)
		{
			case WvTraceSessionLimitType.MemoryTotal:
			case WvTraceSessionLimitType.MemoryDelta:
				return limit.WvBTToKilobytesString();
			case WvTraceSessionLimitType.Duration:
				return limit.WvBTToDurationMSString();
			default:
				return limit.ToString();
		}
	}

	public static string WvBTToTimeString(this DateTimeOffset? timestamp)
	{
		if (timestamp == null) return "n/a";
		return $"{timestamp.Value.ToString(WvConstants.TimestampFormat)} ms";
	}
	public static string WvBTToTimeString(this DateTimeOffset timestamp)
	{
		return $"{timestamp.ToString(WvConstants.TimestampFormat)} ms";
	}
}
