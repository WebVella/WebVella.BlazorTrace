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
public static class WvModalUtility
{
	public static List<WvTraceRow> GenerateTraceRows(this WvSnapshot primarySn,
		WvSnapshot secondarySn)
	{
		//Generate data on secondarySN (which should be the newest in the default case
		var methodComparisonDict = new Dictionary<string, WvSnapshotMethodComparison>();
		var memoryComparisonDict = new Dictionary<string, WvSnapshotMemoryComparison>();
		methodComparisonDict.AddSnapshotToComparisonDictionary(memoryComparisonDict, primarySn, true);
		methodComparisonDict.AddSnapshotToComparisonDictionary(memoryComparisonDict, secondarySn, false);
		methodComparisonDict.ProcessComparisonDictionary(memoryComparisonDict);
		var result = new List<WvTraceRow>();

		foreach (var moduleName in secondarySn.ModuleDict.Keys)
		{
			var module = secondarySn.ModuleDict[moduleName];
			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				foreach (var componentTaggedInstance in component.TaggedInstances)
				{
					foreach (WvTraceSessionMethod tm in componentTaggedInstance.MethodsTotal(includeNotCalled: false))
					{
						var methodHash = tm.GenerateHash(moduleName, componentFullName, componentTaggedInstance.Tag);
						result.Add(new WvTraceRow
						{
							Module = moduleName,
							Component = component.Name,
							Tag = componentTaggedInstance.Tag,
							Method = tm.Name,
							LastMemoryKB = tm.LastMemoryBytes.ToKilobytes(),
							LastDurationMS = tm.LastDurationMs,
							TraceList = tm.TraceList,
							LimitHits = tm.LimitHits,
							MethodComparison = methodComparisonDict[methodHash].ComparisonData,
							MemoryComparison = memoryComparisonDict[methodHash].ComparisonData,
						});
					}
				}
			}
		}

		return result;
	}

	public static string GenerateHash(string? moduleName, string? componentFullname, string? tag, string? methodName)
		=> $"{moduleName}$$${componentFullname}$$${tag}$$${methodName}";

	public static void AddSnapshotToComparisonDictionary(this Dictionary<string, WvSnapshotMethodComparison> methodComp,
		Dictionary<string, WvSnapshotMemoryComparison> memoryComp,
		WvSnapshot? snapshot,
		bool isPrimary = true)
	{
		if (methodComp is null) methodComp = new();
		if (memoryComp is null) memoryComp = new();
		if (snapshot is null) return;
		foreach (var moduleName in snapshot.ModuleDict.Keys)
		{
			var module = snapshot.ModuleDict[moduleName];
			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				foreach (var componentTaggedInstance in component.TaggedInstances)
				{
					foreach (var method in componentTaggedInstance.MethodsTotal(includeNotCalled: true))
					{
						var methodHash = method.GenerateHash(moduleName, componentFullName, componentTaggedInstance.Tag);
						//Method comparison
						if (!methodComp.ContainsKey(methodHash))
							methodComp[methodHash] = new();
						if (isPrimary)
							methodComp[methodHash].PrimarySnapshotMethod = method;
						else
							methodComp[methodHash].SecondarySnapshotMethod = method;

						//Memory comparison
						if (!memoryComp.ContainsKey(methodHash))
							memoryComp[methodHash] = new();
						if (isPrimary)
							memoryComp[methodHash].PrimarySnapshotMethod = method;
						else
							memoryComp[methodHash].SecondarySnapshotMethod = method;
					}
				}
			}
		}
	}

	public static void ProcessComparisonDictionary(this Dictionary<string, WvSnapshotMethodComparison> methodDict,
		Dictionary<string, WvSnapshotMemoryComparison> memoryDict)
	{
		if (methodDict is null) methodDict = new();
		if (memoryDict is null) memoryDict = new();
		foreach (var methodHash in methodDict.Keys)
		{
			var methodComparison = methodDict[methodHash];
			if (methodComparison.SecondarySnapshotMethod is null) continue;
			//Method comparison
			var pr = methodComparison.PrimarySnapshotMethod;
			var sc = methodComparison.SecondarySnapshotMethod;
			var compData = methodComparison.ComparisonData;
			compData.TraceListChange = sc.TraceList.Count - pr.TraceList.Count;
		}

		foreach (var methodHash in memoryDict.Keys)
		{
			var memoryComparison = memoryDict[methodHash];
			if (memoryComparison.SecondarySnapshotMethod is null) continue;
			//Method comparison
			var pr = memoryComparison.PrimarySnapshotMethod;
			var sc = memoryComparison.SecondarySnapshotMethod;
			var prLastExitedTrace = pr.LastExitedTrace;
			var scLastExitedTrace = sc?.LastExitedTrace;
			if (prLastExitedTrace is null || prLastExitedTrace.OnExitMemoryInfo is null
			|| scLastExitedTrace is null || scLastExitedTrace.OnExitMemoryInfo is null) continue;

			var compData = memoryComparison.ComparisonData;
			compData.LastMemoryChangeBytes = (sc.LastMemoryBytes ?? 0) - (pr.LastMemoryBytes ?? 0);
			foreach (var memInfo in prLastExitedTrace.OnExitMemoryInfo)
			{
				memoryComparison.ComparisonData.Fields.Add(new WvSnapshotMemoryComparisonDataField
				{
					FieldName = memInfo.FieldName,
					AssemblyName = memInfo.AssemblyName,
					PrimarySnapshotBytes = memInfo.Size
				});
			}
			foreach (var memInfo in scLastExitedTrace.OnExitMemoryInfo)
			{
				var match = memoryComparison.ComparisonData.Fields.FirstOrDefault(x => x.Id == WvTraceUtility.GetMemoryInfoId(memInfo.AssemblyName, memInfo.FieldName));
				if (match is not null)
				{
					match.SecondarySnapshotBytes = memInfo.Size;
				}
				else
				{
					memoryComparison.ComparisonData.Fields.Add(new WvSnapshotMemoryComparisonDataField
					{
						FieldName = memInfo.FieldName,
						AssemblyName = memInfo.AssemblyName,
						SecondarySnapshotBytes = memInfo.Size
					});
				}
			}
		}
	}
	public static long? GetValueChange(this long? primary, long? secondary)
	{
		if (primary is null && secondary is null) return null;
		if (primary is null || secondary is null) return 0;
		return secondary.Value - primary;
	}

	public static double? GetValueAsKB(this long? value) => value.ToKilobytes();
}
