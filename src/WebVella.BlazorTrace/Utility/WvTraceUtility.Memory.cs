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
	public static long GetSize(this ComponentBase obj,
		List<WvTraceMemoryInfo> memoryDetails,
		WvBlazorTraceConfiguration configuration,
		int maxDepth = 5)
	{
		if (obj == null) return 0;
		return MemorySizeCalculator.CalculateComponentMemorySize(
		component: obj,
		memoryDetails: memoryDetails,
		configuration: configuration,
		maxDepth: maxDepth
		);
	}

}
