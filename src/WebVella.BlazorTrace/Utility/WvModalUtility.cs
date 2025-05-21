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
		WvSnapshot? secondarySn)
	{
		var comparisonDict = new Dictionary<string, WvSnapshotMethodComparison>();
		AddSnapshotToComparisonDictionary(comparisonDict, primarySn, true);
		AddSnapshotToComparisonDictionary(comparisonDict, secondarySn, false);
		ProcessComparisonDictionary(comparisonDict);
		var result = new List<WvTraceRow>();
		foreach (var moduleName in primarySn.ModuleDict.Keys)
		{
			var module = primarySn.ModuleDict[moduleName];
			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				foreach (var tm in component.MethodsTotal(includeNotCalled:false))
				{
					result.Add(new WvTraceRow
					{
						Module = moduleName,
						Component = component.Name,
						Method = tm.Name,
						AverageMemoryKB = tm.AverageMemoryBytes is null ? null : tm.AverageMemoryBytes.Value.ToKilobytes(),
						AverageDurationMS = tm.AvarageDurationMs,
						CallsCount = tm.MaxCallsCount,
						LimitHits = tm.LimitHits,
						ComparisonData = comparisonDict[tm.GenerateHash(moduleName,componentFullName)].ComparisonData
					});
				}
			}
		}

		return result;
	}

	public static void AddSnapshotToComparisonDictionary(
		this Dictionary<string, WvSnapshotMethodComparison> dict,
		WvSnapshot? snapshot,
		bool isPrimary = true)
	{
		if (dict is null) dict = new();
		if (snapshot is null) return;
		foreach (var moduleName in snapshot.ModuleDict.Keys)
		{
			var module = snapshot.ModuleDict[moduleName];
			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				foreach (var method in component.MethodsTotal(includeNotCalled:true))
				{
					var methodHash = method.GenerateHash(moduleName, componentFullName);
					if (!dict.ContainsKey(methodHash))
						dict[methodHash] = new();

					if (isPrimary)
						dict[methodHash].PrimarySnapshotMethod = method;
					else
						dict[methodHash].SecondarySnapshotMethod = method;
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

			dictData.ComparisonData.MinDurationMs = GetValueChange(pr.MinDurationMs,sc?.MinDurationMs);
			dictData.ComparisonData.MaxDurationMs = GetValueChange(pr.MaxDurationMs,sc?.MaxDurationMs);
			dictData.ComparisonData.AvarageDurationMs = GetValueChange(pr.AvarageDurationMs,sc?.AvarageDurationMs);
			dictData.ComparisonData.OnEnterMinMemoryBytes = GetValueChange(pr.OnEnterMinMemoryBytes,sc?.OnEnterMinMemoryBytes);
			dictData.ComparisonData.OnEnterMaxMemoryBytes = GetValueChange(pr.OnEnterMaxMemoryBytes,sc?.OnEnterMaxMemoryBytes);
			dictData.ComparisonData.OnExitMinMemoryBytes = GetValueChange(pr.OnExitMinMemoryBytes,sc?.OnExitMinMemoryBytes);
			dictData.ComparisonData.OnExitMaxMemoryBytes = GetValueChange(pr.OnExitMaxMemoryBytes,sc?.OnExitMaxMemoryBytes);
			dictData.ComparisonData.AverageMemoryBytes = GetValueChange(pr.AverageMemoryBytes,sc?.AverageMemoryBytes);
			dictData.ComparisonData.MinMemoryDeltaBytes = GetValueChange(pr.MinMemoryDeltaBytes,sc?.MinMemoryDeltaBytes);
			dictData.ComparisonData.MaxMemoryDeltaBytes = GetValueChange(pr.MaxMemoryDeltaBytes,sc?.MaxMemoryDeltaBytes);
			dictData.ComparisonData.OnEnterCallsCount = GetValueChange(pr.OnEnterCallsCount,sc?.OnEnterCallsCount);
			dictData.ComparisonData.OnExitCallsCount = GetValueChange(pr.OnExitCallsCount,sc?.OnExitCallsCount);
			dictData.ComparisonData.MaxCallsCount = GetValueChange(pr.MaxCallsCount,sc?.MaxCallsCount);
			dictData.ComparisonData.CompletedCallsCount = GetValueChange(pr.CompletedCallsCount,sc?.CompletedCallsCount);
			dictData.ComparisonData.TraceCount = GetValueChange((long)pr.TraceList.Count,(long?)sc?.TraceList.Count);
			dictData.ComparisonData.LimitHits = GetValueChange((long)pr.LimitHits.Count,(long?)sc?.LimitHits.Count);
		}
	}
	public static long? GetValueChange(this long? primary, long? secondary)
	{
		if (primary is null && secondary is null) return null;
		if (primary is null || secondary is null) return 0;
		return secondary.Value - primary;
	}
}
