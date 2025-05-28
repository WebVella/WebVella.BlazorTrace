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
	public static double? ToKilobytes(this long? bytes)
	{
		if (bytes is null) return null;
		if (bytes < 0)
		{
			return -1; // Handle invalid input
		}
		const double kilobyteFactor = 1024.0;
		return Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
	}
	public static double ToKilobytes(this long bytes)
	{
		if (bytes < 0)
		{
			return -1; // Handle invalid input
		}
		const double kilobyteFactor = 1024.0;
		return Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
	}
	public static string ToKilobytesString(this long? bytes)
	{
		if (bytes == null) return "n/a";
			
		if (bytes < 0) return "err"; // Handle invalid input
		if(bytes == 0) return "0 KB";
		if(bytes <= 15) return bytes.ToString() + " bytes";
		const double kilobyteFactor = 1024.0;
		var kb = Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
		return kb.ToString() + " KB";
	}
	public static string ToKilobytesString(this long bytes)
	{
		if (bytes < 0) return "err"; // Handle invalid input
		if(bytes == 0) return "0 KB";
		if(bytes <= 15) return bytes.ToString() + " bytes";
		const double kilobyteFactor = 1024.0;
		var kb = Math.Round((double)bytes / kilobyteFactor, 2, MidpointRounding.AwayFromZero);
		return kb.ToString() + " KB";
	}
	public static string GetMemoryInfoId(string assemblyFullName, string fieldName)
		=> $"{assemblyFullName}$$${fieldName}";
	public static string GetFirstRenderString(this bool? firstRender)
	{
		if (firstRender == null) return "n/a";
		if (!firstRender.Value) return "no";
		return "yes";
	}
	public static string ToDurationMSString(this long? duration)
	{
		if (duration == null) return "n/a";
		return $"{duration.Value} ms";
	}
	public static string ToDurationMSString(this long duration)
	{
		return $"{duration} ms";
	}

	public static string GetLimitValueAsString(this long limit, WvTraceSessionLimitType type)
	{
		switch (type)
		{
			case WvTraceSessionLimitType.MemoryTotal:
			case WvTraceSessionLimitType.MemoryDelta:
				return limit.ToKilobytesString();
			case WvTraceSessionLimitType.Duration:
				return limit.ToDurationMSString();
			default:
				return limit.ToString();
		}
	}
}
