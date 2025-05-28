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
	public static List<WvMethodTraceRow> GenerateMethodTraceRows(this WvSnapshot primarySn,
		WvSnapshot secondarySn)
	{
		//Generate data on secondarySN (which should be the newest in the default case
		var unionDataDict = new Dictionary<string, WvModuleUnionData>();
		var methodComparisonDict = new Dictionary<string, WvSnapshotMethodComparison>();
		var memoryComparisonDict = new Dictionary<string, WvSnapshotMemoryComparison>();
		unionDataDict.AddSnapshotToComparisonDictionary(methodComparisonDict, memoryComparisonDict, primarySn, true);
		unionDataDict.AddSnapshotToComparisonDictionary(methodComparisonDict, memoryComparisonDict, secondarySn, false);
		unionDataDict.ProcessComparisonDictionary(methodComparisonDict, memoryComparisonDict);
		var result = new List<WvMethodTraceRow>();

		foreach (var moduleName in unionDataDict.Keys)
		{
			foreach (var componentFullName in unionDataDict[moduleName].ComponentDict.Keys)
			{
				foreach (var componentTaggedInstance in unionDataDict[moduleName].ComponentDict[componentFullName].TaggedInstances)
				{
					foreach (var methodHash in componentTaggedInstance.MethodDict.Keys)
					{
						var methodUnionData = componentTaggedInstance.MethodDict[methodHash];
						if (methodUnionData.Primary is null && methodUnionData.Secondary is null)
							continue;

						var row = new WvMethodTraceRow
						{
							Module = moduleName,
							Component = unionDataDict[moduleName].ComponentDict[componentFullName].Name,
							ComponentFullName = componentFullName,
							InstanceTag = componentTaggedInstance.Tag,
							Method = methodUnionData.Secondary?.Name ?? methodUnionData.Primary?.Name,
							LastMemoryKB = methodUnionData.Secondary is not null ? methodUnionData.Secondary.LastMemoryBytes.ToKilobytes() : 0,
							LastDurationMS = methodUnionData.Secondary is not null ? methodUnionData.Secondary.LastDurationMS : 0,
							TraceList = methodUnionData.Secondary is not null ? methodUnionData.Secondary.TraceList : new(),
							LimitHits = methodUnionData.Secondary is not null ? methodUnionData.Secondary.LimitHits : new(),
							MethodComparison = methodComparisonDict[methodHash].ComparisonData,
							MemoryComparison = memoryComparisonDict[methodHash].ComparisonData,
						};
						result.Add(row);
					}
				}
			}
		}

		return result;
	}

	public static List<WvSignalTraceRow> GenerateSignalTraceRows(this WvSnapshot primarySn,
		WvSnapshot secondarySn)
	{
		//Generate data on secondarySN (which should be the newest in the default case
		var unionDataDict = new Dictionary<string, WvSignalUnionData>();
		var signalComparisonDict = new Dictionary<string, WvSnapshotSignalComparison>();
		unionDataDict.AddSnapshotToSignalComparisonDictionary(signalComparisonDict, primarySn, true);
		unionDataDict.AddSnapshotToSignalComparisonDictionary(signalComparisonDict, secondarySn, false);
		unionDataDict.ProcessSignalComparisonDictionary(signalComparisonDict);
		var result = new List<WvSignalTraceRow>();

		foreach (var signalName in unionDataDict.Keys)
		{

			var signalUnionData = unionDataDict[signalName];
			if (signalUnionData.Primary is null && signalUnionData.Secondary is null)
				continue;

			var row = new WvSignalTraceRow
			{
				SignalName = signalName,
				TraceList = signalUnionData.Secondary is not null ? signalUnionData.Secondary.TraceList : new(),
				LimitHits = signalUnionData.Secondary is not null ? signalUnionData.Secondary.LimitHits : new(),
				SignalComparison = signalComparisonDict[signalName].ComparisonData,
			};
			result.Add(row);
		}

		return result;
	}

	public static string GenerateMethodHash(string? moduleName, string? componentFullname, string? tag, string? methodName)
		=> $"{moduleName}$$${componentFullname}$$${tag}$$${methodName}";

	public static string GenerateSignalHash(string? signalName)
		=> $"{signalName}";

	public static void AddSnapshotToComparisonDictionary(this Dictionary<string, WvModuleUnionData> unionDict, Dictionary<string, WvSnapshotMethodComparison> methodComp,
		Dictionary<string, WvSnapshotMemoryComparison> memoryComp,
		WvSnapshot? snapshot,
		bool isPrimary = true)
	{
		if (unionDict is null) unionDict = new();
		if (methodComp is null) methodComp = new();
		if (memoryComp is null) memoryComp = new();
		if (snapshot is null) return;
		foreach (var moduleName in snapshot.ModuleDict.Keys)
		{
			var module = snapshot.ModuleDict[moduleName];
			unionDict.SetModuleUnionData(moduleName, module, isPrimary);

			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				unionDict.SetComponentUnionData(moduleName, componentFullName, component, isPrimary);

				foreach (var componentTaggedInstance in component.TaggedInstances)
				{
					unionDict.SetTaggedInstanceUnionData(moduleName, componentFullName, componentTaggedInstance, isPrimary);

					foreach (var method in componentTaggedInstance.MethodsTotal())
					{
						var methodHash = method.GenerateHash(moduleName, componentFullName, componentTaggedInstance.Tag);
						unionDict.SetMethodUnionData(moduleName, componentFullName, componentTaggedInstance.Tag, methodHash, method, isPrimary);
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

	public static void AddSnapshotToSignalComparisonDictionary(this Dictionary<string, WvSignalUnionData> unionDict, Dictionary<string, WvSnapshotSignalComparison> signalComp,
			WvSnapshot? snapshot,
			bool isPrimary = true)
	{
		if (unionDict is null) unionDict = new();
		if (signalComp is null) signalComp = new();
		if (snapshot is null) return;
		foreach (var signalName in snapshot.SignalDict.Keys)
		{
			var signal = snapshot.SignalDict[signalName];
			unionDict.SetSignalUnionData(signalName, signal, isPrimary);
			if (!signalComp.ContainsKey(signalName))
				signalComp[signalName] = new();

			if (isPrimary)
				signalComp[signalName].PrimarySnapshotSignal = signal;
			else
				signalComp[signalName].SecondarySnapshotSignal = signal;
		}
	}
	public static void ProcessComparisonDictionary(this Dictionary<string, WvModuleUnionData> unionDict, Dictionary<string, WvSnapshotMethodComparison> methodDict,
		Dictionary<string, WvSnapshotMemoryComparison> memoryDict)
	{
		if (unionDict is null) unionDict = new();
		if (methodDict is null) methodDict = new();
		if (memoryDict is null) memoryDict = new();
		foreach (var methodHash in methodDict.Keys)
		{
			var methodComparison = methodDict[methodHash];
			//Method comparison
			var pr = methodComparison.PrimarySnapshotMethod;
			var sc = methodComparison.SecondarySnapshotMethod;
			var compData = methodComparison.ComparisonData;
			compData.TraceListChange = 0;
			compData.LastDurationChangeMS = 0;
			if (sc is not null)
			{
				compData.TraceListChange += sc.TraceList.Count;
				compData.LastDurationChangeMS += sc.LastDurationMS;
			}
			if (pr is not null)
			{
				compData.TraceListChange -= pr.TraceList.Count;
				compData.LastDurationChangeMS -= pr.LastDurationMS;
			}
		}

		foreach (var methodHash in memoryDict.Keys)
		{
			var memoryComparison = memoryDict[methodHash];
			//Method comparison
			var pr = memoryComparison.PrimarySnapshotMethod;
			var sc = memoryComparison.SecondarySnapshotMethod;
			if (pr is not null)
			{
				if (pr.LastExitedTrace is not null && pr.LastExitedTrace.OnExitMemoryInfo is not null)
				{
					foreach (var memInfo in pr.LastExitedTrace.OnExitMemoryInfo)
					{
						memoryComparison.ComparisonData.Fields.Add(new WvSnapshotMemoryComparisonDataField
						{
							FieldName = memInfo.FieldName,
							AssemblyName = memInfo.AssemblyName,
							PrimarySnapshotBytes = memInfo.Size
						});
					}
				}
				memoryComparison.ComparisonData.LastMemoryChangeBytes -= pr.LastMemoryBytes ?? 0;
			}
			if (sc is not null)
			{
				if (sc.LastExitedTrace is not null && sc.LastExitedTrace.OnExitMemoryInfo is not null)
				{
					foreach (var memInfo in sc.LastExitedTrace.OnExitMemoryInfo)
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
				memoryComparison.ComparisonData.LastMemoryChangeBytes += sc.LastMemoryBytes ?? 0;
			}
		}
	}


	public static void ProcessSignalComparisonDictionary(this Dictionary<string, WvSignalUnionData> unionDict, Dictionary<string, WvSnapshotSignalComparison> signalDict)
	{
		if (unionDict is null) unionDict = new();
		if (signalDict is null) signalDict = new();
		foreach (var signalName in signalDict.Keys)
		{
			var signalComparison = signalDict[signalName];
			//Method comparison
			var pr = signalComparison.PrimarySnapshotSignal;
			var sc = signalComparison.SecondarySnapshotSignal;
			var compData = signalComparison.ComparisonData;
			compData.TraceListChange = 0;
			if (sc is not null)
			{
				compData.TraceListChange += sc.TraceList.Count;
			}
			if (pr is not null)
			{
				compData.TraceListChange -= pr.TraceList.Count;
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

	public static List<WvSnapshotMemoryComparisonDataField> ToMemoryDataFields(this List<WvTraceMemoryInfo>? memInfo)
	{
		var result = new List<WvSnapshotMemoryComparisonDataField>();
		if (memInfo is null) return result;
		foreach (var item in memInfo)
		{
			result.Add(new WvSnapshotMemoryComparisonDataField
			{
				AssemblyName = item.AssemblyName,
				FieldName = item.FieldName,
				PrimarySnapshotBytes = item.Size,
				SecondarySnapshotBytes = item.Size
			});
		}
		return result;
	}

	private static void SetModuleUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
		string moduleName,
		WvTraceSessionModule module,
		bool isPrimary)
	{
		if (!unionDict.ContainsKey(moduleName))
			unionDict[moduleName] = new();

		if (isPrimary)
			unionDict[moduleName].Primary = module;
		else
			unionDict[moduleName].Secondary = module;
	}

	private static void SetComponentUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
		string moduleName,
		string componentFullName,
		WvTraceSessionComponent component,
		bool isPrimary)
	{

		if (!unionDict[moduleName].ComponentDict.ContainsKey(componentFullName))
			unionDict[moduleName].ComponentDict[componentFullName] = new() { Name = component.Name };

		if (isPrimary)
			unionDict[moduleName].ComponentDict[componentFullName].Primary = component;
		else
			unionDict[moduleName].ComponentDict[componentFullName].Secondary = component;
	}

	private static void SetSignalUnionData(this Dictionary<string, WvSignalUnionData> unionDict,
		string signalName,
		WvTraceSessionSignal signal,
		bool isPrimary)
	{

		if (!unionDict.ContainsKey(signalName))
			unionDict[signalName] = new();

		if (isPrimary)
			unionDict[signalName].Primary = signal;
		else
			unionDict[signalName].Secondary = signal;
	}

	private static void SetTaggedInstanceUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
		string moduleName,
		string componentFullName,
		WvTraceSessionComponentTaggedInstance taggedInstance,
		bool isPrimary)
	{
		var tag = taggedInstance.Tag ?? String.Empty;
		var matchedInstanceUnionData = unionDict[moduleName].ComponentDict[componentFullName].TaggedInstances.FirstOrDefault(x => x.Tag == tag);
		if (matchedInstanceUnionData is null)
		{
			matchedInstanceUnionData = new WvTaggedInstanceUnionData
			{
				Tag = tag,
			};
			unionDict[moduleName].ComponentDict[componentFullName].TaggedInstances.Add(matchedInstanceUnionData);
		}
		if (isPrimary)
			matchedInstanceUnionData.Primary = taggedInstance;
		else
			matchedInstanceUnionData.Secondary = taggedInstance;
	}

	private static void SetMethodUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
		string moduleName,
		string componentFullName,
		string? taggedInstanceTag,
		string methodHash,
		WvTraceSessionMethod method,
		bool isPrimary)
	{
		taggedInstanceTag = taggedInstanceTag ?? String.Empty;
		var matchedInstanceUnionData = unionDict[moduleName].ComponentDict[componentFullName].TaggedInstances.Single(x => x.Tag == taggedInstanceTag);
		if (!matchedInstanceUnionData.MethodDict.ContainsKey(methodHash))
			matchedInstanceUnionData.MethodDict[methodHash] = new();
		if (isPrimary)
			matchedInstanceUnionData.MethodDict[methodHash].Primary = method;
		else
			matchedInstanceUnionData.MethodDict[methodHash].Secondary = method;
	}
}
