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
	public static WvTraceInfo? GetInfo(this object component, Guid? traceId, string? instanceTag, string methodName)
	{
		var componentType = component.GetType();
		return new WvTraceInfo
		{
			TraceId = traceId,
			MethodName = methodName,
			ComponentFullName = componentType.FullName,
			ComponentName = componentType.Name,
			InstanceTag = instanceTag,
			ModuleName = componentType?.Module.Name?.Replace(".dll", "")
		};
	}

	public static WvTraceSessionMethodTrace? GetMatchingOnEnterTrace(this List<WvTraceSessionMethodTrace> traceList, Guid? traceId){ 
		var orderedTraceList = traceList.OrderBy(x=> x.EnteredOn);
		if(traceId is not null) 
			return traceList.FirstOrDefault(x =>x.EnteredOn is not null && x.ExitedOn is null && x.TraceId == traceId.Value);
		
		//any not exited will do
		return traceList.FirstOrDefault(x =>x.EnteredOn is not null && x.ExitedOn is null);
	}
}
