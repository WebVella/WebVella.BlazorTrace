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
		var methodComparisonDict = new Dictionary<string, WvSnapshotMethodComparison>();
		var memoryComparisonDict = new Dictionary<string, WvSnapshotMemoryComparison>();
		AddSnapshotToComparisonDictionary(methodComparisonDict, memoryComparisonDict, primarySn, true);
		if (primarySn.Id != secondarySn.Id)
			AddSnapshotToComparisonDictionary(methodComparisonDict, memoryComparisonDict, secondarySn, false);
		ProcessComparisonDictionary(methodComparisonDict);
		var result = new List<WvTraceRow>();
		foreach (var moduleName in primarySn.ModuleDict.Keys)
		{
			var module = primarySn.ModuleDict[moduleName];
			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				foreach (var componentTaggedInstance in component.TaggedInstances)
				{
					foreach (var tm in componentTaggedInstance.MethodsTotal(includeNotCalled: false))
					{
						result.Add(new WvTraceRow
						{
							Module = moduleName,
							Component = component.Name,
							Tag = componentTaggedInstance.Tag,
							Method = tm.Name,
							AverageMemoryKB = tm.AverageMemoryBytes.ToKilobytes(),
							AverageDurationMS = tm.AvarageDurationMs,
							TraceList = tm.TraceList,
							LimitHits = tm.LimitHits,
							MethodComparison = methodComparisonDict[tm.GenerateHash(moduleName, componentFullName, componentTaggedInstance.Tag)].ComparisonData
						});
					}
				}
			}
		}

		return result;
	}

	public static string GenerateHash(string? moduleName, string? componentFullname, string? tag, string? methodName)
		=> $"{moduleName}$$${componentFullname}$$${tag}$$${methodName}";

	public static void AddSnapshotToComparisonDictionary(
		Dictionary<string, WvSnapshotMethodComparison> methodComp,
		Dictionary<string, WvSnapshotMemoryComparison> memoryComp,
		WvSnapshot? snapshot,
		bool isPrimary = true)
	{
		if (methodComp is null) methodComp = new();
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
						if (!methodComp.ContainsKey(methodHash))
							methodComp[methodHash] = new();

						if (isPrimary)
							methodComp[methodHash].PrimarySnapshotMethod = method;
						else
							methodComp[methodHash].SecondarySnapshotMethod = method;
					}
				}
			}
		}
	}

	public static void ProcessComparisonDictionary(this Dictionary<string, WvSnapshotMethodComparison> dict)
	{
		if (dict is null) dict = new();
		foreach (var methodHash in dict.Keys)
		{
			var dictData = dict[methodHash];
			if (dictData.SecondarySnapshotMethod is null) continue;

			var pr = dictData.PrimarySnapshotMethod;
			var sc = dictData.SecondarySnapshotMethod;

			dictData.ComparisonData.MinDurationMS = GetValueChange(pr.MinDurationMs, sc?.MinDurationMs);
			dictData.ComparisonData.MaxDurationMS = GetValueChange(pr.MaxDurationMs, sc?.MaxDurationMs);
			dictData.ComparisonData.AverageDurationMS = GetValueChange(pr.AvarageDurationMs, sc?.AvarageDurationMs);
			dictData.ComparisonData.OnEnterMinMemoryKB = GetValueAsKB(GetValueChange(pr.OnEnterMinMemoryBytes, sc?.OnEnterMinMemoryBytes));
			dictData.ComparisonData.OnEnterMaxMemoryKB = GetValueAsKB(GetValueChange(pr.OnEnterMaxMemoryBytes, sc?.OnEnterMaxMemoryBytes));
			dictData.ComparisonData.OnExitMinMemoryKB = GetValueAsKB(GetValueChange(pr.OnExitMinMemoryBytes, sc?.OnExitMinMemoryBytes));
			dictData.ComparisonData.OnExitMaxMemoryKB = GetValueAsKB(GetValueChange(pr.OnExitMaxMemoryBytes, sc?.OnExitMaxMemoryBytes));
			dictData.ComparisonData.AverageMemoryKB = GetValueAsKB(GetValueChange(pr.AverageMemoryBytes, sc?.AverageMemoryBytes));
			dictData.ComparisonData.MinMemoryDeltaKB = GetValueAsKB(GetValueChange(pr.MinMemoryDeltaBytes, sc?.MinMemoryDeltaBytes));
			dictData.ComparisonData.MaxMemoryDeltaKB = GetValueAsKB(GetValueChange(pr.MaxMemoryDeltaBytes, sc?.MaxMemoryDeltaBytes));
			dictData.ComparisonData.OnEnterCallsCount = GetValueChange(pr.OnEnterCallsCount, sc?.OnEnterCallsCount);
			dictData.ComparisonData.OnExitCallsCount = GetValueChange(pr.OnExitCallsCount, sc?.OnExitCallsCount);
			dictData.ComparisonData.CompletedCallsCount = GetValueChange(pr.CompletedCallsCount, sc?.CompletedCallsCount);
			dictData.ComparisonData.TraceCount = GetValueChange((long)pr.TraceList.Count, (long?)sc?.TraceList.Count);
			dictData.ComparisonData.LimitHits = GetValueChange((long)pr.LimitHits.Count, (long?)sc?.LimitHits.Count);
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
