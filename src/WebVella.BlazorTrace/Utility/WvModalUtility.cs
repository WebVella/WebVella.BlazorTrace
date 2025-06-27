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
		WvSnapshot secondarySn, List<WvTraceMute> muteTraces, List<string> pins)
	{
		//Generate data on secondarySN (which should be the newest in the default case
		var unionDataDict = new Dictionary<string, WvModuleUnionData>();
		var methodComparisonDict = new Dictionary<string, WvSnapshotMethodComparison>();
		var memoryComparisonDict = new Dictionary<string, WvSnapshotMemoryComparison>();
		unionDataDict.AddSnapshotToComparisonDictionary(methodComparisonDict, memoryComparisonDict, primarySn, muteTraces, true);
		unionDataDict.AddSnapshotToComparisonDictionary(methodComparisonDict, memoryComparisonDict, secondarySn, muteTraces, false);
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
							LastMemoryBytes = methodUnionData.Secondary is not null ? methodUnionData.Secondary.LastMemoryBytes : 0,
							LastDurationMS = methodUnionData.Secondary is not null ? methodUnionData.Secondary.LastDurationMS : 0,
							TraceList = methodUnionData.Secondary is not null ? methodUnionData.Secondary.TraceList : new(),
							LimitHits = methodUnionData.Secondary is not null ? methodUnionData.Secondary.LimitHits : new(),
							MethodComparison = methodComparisonDict[methodHash].ComparisonData,
							MemoryComparison = memoryComparisonDict[methodHash].ComparisonData,
						};

						if (row.IsMuted(muteTraces, pins)) continue;
						row.LimitHits = row.LimitHits.GetUnmuted(
							row: row,
							muteList: muteTraces
						);

						result.Add(row);
					}
				}
			}
		}

		return result;
	}

	public static List<WvConsoleLog> GenerateMethodLogRows(this WvSnapshot primarySn, List<WvTraceMute> muteTraces)
	{
		var result = new List<WvConsoleLog>();
		foreach (var moduleName in primarySn.ModuleDict.Keys)
		{
			var module = primarySn.ModuleDict[moduleName];
			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				var component = module.ComponentDict[componentFullName];
				foreach (var componentTaggedInstance in component.TaggedInstances)
				{
					foreach (var method in componentTaggedInstance.MethodsTotal())
					{
						if (method.Name.IsMethodMuted(
							moduleName: moduleName,
							componentFullName: componentFullName,
							instanceTag: componentTaggedInstance.Tag,
							muteList: muteTraces
							)) continue;
						foreach (var trace in method.TraceList)
						{
							if (trace.EnteredOn is not null)
							{
								result.Add(trace.ConvertTraceToLog(
									isOnEnter: true,
									moduleName: moduleName,
									componentName: component.Name,
									instanceTag: componentTaggedInstance.Tag,
									methodName: method.Name,
									signal: null
									));
							}
							if (trace.ExitedOn is not null)
							{
								result.Add(trace.ConvertTraceToLog(
									isOnEnter: false,
									moduleName: moduleName,
									componentName: component.Name,
									instanceTag: componentTaggedInstance.Tag,
									methodName: method.Name,
									signal: null
									));
							}
						}
					}
				}
			}
		}
		return result.OrderBy(x => x.CreatedOn).ToList();
	}

	public static List<WvConsoleLog> GenerateSignalLogRows(this WvSnapshot primarySn, List<WvTraceMute> muteTraces)
	{
		var result = new List<WvConsoleLog>();
		foreach (var signalName in primarySn.SignalDict.Keys)
		{
			var signal = primarySn.SignalDict[signalName];
			foreach (var trace in signal.TraceList)
			{
				result.Add(trace.ConvertTraceToLog(
					moduleName: trace.ModuleName,
					componentName: trace.ComponentName,
					instanceTag: trace.InstanceTag,
					methodName: trace.MethodName,
					signal: signalName
					));
			}
		}

		return result.OrderBy(x => x.CreatedOn).ToList();
	}

	public static List<WvSignalTraceRow> GenerateSignalTraceRows(this WvSnapshot primarySn,
		WvSnapshot secondarySn, List<WvTraceMute> muteTraces, List<string> pins)
	{
		//Generate data on secondarySN (which should be the newest in the default case
		var unionDataDict = new Dictionary<string, WvSignalUnionData>();
		var signalComparisonDict = new Dictionary<string, WvSnapshotSignalComparison>();
		unionDataDict.AddSnapshotToSignalComparisonDictionary(signalComparisonDict, primarySn, muteTraces, true);
		unionDataDict.AddSnapshotToSignalComparisonDictionary(signalComparisonDict, secondarySn, muteTraces, false);
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

	public static string GenerateTraceMuteHash(WvTraceMute item)
		=> $"{item.Type}$$${item.Module ?? "undefined"}$$${item.ComponentFullName ?? "undefined"}$$${item.InstanceTag ?? "undefined"}$$${item.Method ?? "undefined"}" +
		$"$$${item.Signal ?? "undefined"}$$${item.Field ?? "undefined"}$$${item.OnEnterCustomData ?? "undefined"}" +
		$"$$${(item.IsPinned is null ? "undefined" : item.IsPinned.Value.ToString())}";

	public static void AddSnapshotToComparisonDictionary(this Dictionary<string, WvModuleUnionData> unionDict,
		Dictionary<string, WvSnapshotMethodComparison> methodComp,
		Dictionary<string, WvSnapshotMemoryComparison> memoryComp,
		WvSnapshot? snapshot,
		List<WvTraceMute> muteList,
		bool isPrimary = true)
	{
		if (unionDict is null) unionDict = new();
		if (methodComp is null) methodComp = new();
		if (memoryComp is null) memoryComp = new();
		if (snapshot is null) return;
		foreach (var moduleName in snapshot.ModuleDict.Keys)
		{
			if (moduleName.IsModuleMuted(muteList: muteList)) continue;
			var module = snapshot.ModuleDict[moduleName];
			unionDict.SetModuleUnionData(moduleName, module, isPrimary);

			foreach (var componentFullName in module.ComponentDict.Keys)
			{
				if (componentFullName.IsComponentMuted(muteList: muteList)) continue;
				var component = module.ComponentDict[componentFullName];
				unionDict.SetComponentUnionData(moduleName, componentFullName, component, isPrimary);

				foreach (var componentTaggedInstance in component.TaggedInstances)
				{
					if (componentTaggedInstance.Tag.IsComponentInstanceMuted(
						componentFullName: componentFullName,
						muteList: muteList)) continue;

					unionDict.SetTaggedInstanceUnionData(moduleName, componentFullName, componentTaggedInstance, isPrimary);

					foreach (var method in componentTaggedInstance.MethodsTotal())
					{
						if (method.Name.IsMethodMuted(
						moduleName: moduleName,
						componentFullName: componentFullName,
						instanceTag: componentTaggedInstance.Tag,
						muteList: muteList
						)) continue;

						var methodHash = method.GenerateHash(moduleName, componentFullName, componentTaggedInstance.Tag);
						var unmutedTraceList = method.TraceList.GetUnmuted(moduleName, componentFullName, componentTaggedInstance.Tag, muteList);
						if (unmutedTraceList.Count == 0) continue;
						method.TraceList = unmutedTraceList;
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
			List<WvTraceMute> muteList,
			bool isPrimary = true)
	{
		if (unionDict is null) unionDict = new();
		if (signalComp is null) signalComp = new();
		if (snapshot is null) return;
		foreach (var signalName in snapshot.SignalDict.Keys)
		{
			if (signalName.IsSignalMuted(muteList: muteList)) continue;
			var signal = snapshot.SignalDict[signalName];
			var unmutedTraceList = signal.TraceList.GetUnmuted(muteList);
			if (unmutedTraceList.Count == 0) continue;
			signal.TraceList = unmutedTraceList;

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
				compData.LastDurationChangeMS += sc.LastDurationMS ?? 0;
			}
			if (pr is not null)
			{
				compData.TraceListChange -= pr.TraceList.Count;
				compData.LastDurationChangeMS -= pr.LastDurationMS ?? 0;
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
							TypeName = memInfo.TypeName,
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
						var match = memoryComparison.ComparisonData.Fields.FirstOrDefault(x => x.Id == WvTraceUtility.WvBTGetMemoryInfoId(memInfo.AssemblyName, memInfo.FieldName));
						if (match is not null)
						{
							match.SecondarySnapshotBytes = memInfo.Size;
						}
						else
						{
							memoryComparison.ComparisonData.Fields.Add(new WvSnapshotMemoryComparisonDataField
							{
								FieldName = memInfo.FieldName,
								TypeName = memInfo.TypeName,
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
				TypeName = item.TypeName,
				PrimarySnapshotBytes = item.Size,
				SecondarySnapshotBytes = item.Size
			});
		}
		return result;
	}
	public static void SetModuleUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
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

	public static void SetComponentUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
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

	public static void SetSignalUnionData(this Dictionary<string, WvSignalUnionData> unionDict,
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

	public static void SetTaggedInstanceUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
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

	public static void SetMethodUnionData(this Dictionary<string, WvModuleUnionData> unionDict,
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

	public static string GenerateMuteDescriptionHtml(this WvTraceMuteType muteType,
		string? module = null,
		string? component = null,
		string? instanceTag = null,
		string? method = null,
		string? signalName = null,
		string? field = null,
		string? assembly = null,
		string? onEnterCustomData = null,
		string? onExitCustomData = null,
		WvTraceSessionLimitType? limitType = null)
	{
		string moduleHtml = !String.IsNullOrWhiteSpace(module) ? $"<span>{module}</span>" : "<span>undefined</span>";
		string componentHtml = !String.IsNullOrWhiteSpace(component) ? $"<span>{component}</span>" : "<span>undefined</span>";
		string instanceTagHtml = !String.IsNullOrWhiteSpace(instanceTag) ? $"<span>{instanceTag}</span>" : "<span>undefined</span>";
		string methodHtml = !String.IsNullOrWhiteSpace(method) ? $"<span>{method}</span>" : "<span>undefined</span>";
		string signalNameHtml = !String.IsNullOrWhiteSpace(signalName) ? $"<span>{signalName}</span>" : "<span>undefined</span>";
		string fieldHtml = !String.IsNullOrWhiteSpace(field) ? $"<span>{field}</span>" : "<span>undefined</span>";
		string assemblyHtml = !String.IsNullOrWhiteSpace(assembly) ? $"<span>{assembly}</span>" : "<span>undefined</span>";
		string onEnterCustomDataHtml = !String.IsNullOrWhiteSpace(onEnterCustomData) ? $"<span>{onEnterCustomData}</span>" : "<span>undefined</span>";
		string onExitCustomDataHtml = !String.IsNullOrWhiteSpace(onExitCustomData) ? $"<span>{onExitCustomData}</span>" : "<span>undefined</span>";
		string limitTypeHtml = limitType is not null ? $"<span>{limitType.Value.WvBTToDescriptionString()}</span>" : "<span>undefined</span>";
		switch (muteType)
		{
			case WvTraceMuteType.Module:
				return $"<span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.Component:
				return $"<span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.ComponentInstance:
				return $"<span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.Method:
				return $"<span class='wv-mute'>method</span> {methodHtml}";
			case WvTraceMuteType.MethodInModule:
				return $"<span class='wv-mute'>method</span> {methodHtml} <span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.MethodInComponent:
				return $"<span class='wv-mute'>method</span> {methodHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.MethodInComponentInstance:
				return $"<span class='wv-mute'>method</span> {methodHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.Signal:
				return $"<span class='wv-mute'>signal</span> {signalNameHtml}";
			case WvTraceMuteType.SignalInModule:
				return $"<span class='wv-mute'>signal</span> {signalNameHtml} <span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.SignalInComponent:
				return $"<span class='wv-mute'>signal</span> {signalNameHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.SignalInComponentInstance:
				return $"<span class='wv-mute'>signal</span> {signalNameHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.Field:
				return $"<span class='wv-mute'>field</span> {fieldHtml}";
			case WvTraceMuteType.FieldInModule:
				return $"<span class='wv-mute'>field</span> {fieldHtml} <span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.FieldInComponent:
				return $"<span class='wv-mute'>field</span> {fieldHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.FieldInComponentInstance:
				return $"<span class='wv-mute'>field</span> {fieldHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.FieldInAssembly:
				return $"<span class='wv-mute'>field</span> {fieldHtml} <span class='wv-mute'>assembly</span> {assemblyHtml}";
			case WvTraceMuteType.Assembly:
				return $"<span class='wv-mute'>assembly</span> {assemblyHtml}";
			case WvTraceMuteType.OnEnterCustomData:
				return $"<span class='wv-mute'>OnEnter custom data</span> {onEnterCustomDataHtml}";
			case WvTraceMuteType.OnEnterCustomDataInModule:
				return $"<span class='wv-mute'>OnEnter custom data</span> {onEnterCustomDataHtml} <span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.OnEnterCustomDataInComponent:
				return $"<span class='wv-mute'>OnEnter custom data</span> {onEnterCustomDataHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.OnEnterCustomDataInComponentInstance:
				return $"<span class='wv-mute'>OnEnter custom data</span> {onEnterCustomDataHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.OnExitCustomData:
				return $"<span class='wv-mute'>OnExit custom data</span> {onExitCustomDataHtml}";
			case WvTraceMuteType.OnExitCustomDataInModule:
				return $"<span class='wv-mute'>OnExit custom data</span> {onExitCustomDataHtml} <span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.OnExitCustomDataInComponent:
				return $"<span class='wv-mute'>OnExit custom data</span> {onExitCustomDataHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.OnExitCustomDataInComponentInstance:
				return $"<span class='wv-mute'>OnExit custom data</span> {onExitCustomDataHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.Limit:
				return $"<span class='wv-mute'>limit</span> {limitTypeHtml}";
			case WvTraceMuteType.LimitInModule:
				return $"<span class='wv-mute'>limit</span> {limitTypeHtml} <span class='wv-mute'>module</span> {moduleHtml}";
			case WvTraceMuteType.LimitInComponent:
				return $"<span class='wv-mute'>limit</span> {limitTypeHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>(all instances)</span>";
			case WvTraceMuteType.LimitInComponentInstance:
				return $"<span class='wv-mute'>limit</span> {limitTypeHtml} <span class='wv-mute'>component</span> {componentHtml} <span class='wv-mute'>instance</span> {instanceTagHtml}";
			case WvTraceMuteType.NotPinnedMethods:
				return "<span>not pinned</span> <span class='wv-mute'>methods</span>";
			case WvTraceMuteType.PinnedMethods:
				return "<span>pinned</span> <span class='wv-mute'>methods</span>";
			case WvTraceMuteType.NotPinnedSignals:
				return "<span>not pinned</span> <span class='wv-mute'>signals</span>";
			case WvTraceMuteType.PinnedSignals:
				return "<span>pinned</span> <span class='wv-mute'>signals</span>";
			default:
				break;
		}

		return String.Empty;
	}

	public static WvConsoleLog ConvertTraceToLog(this WvTraceSessionMethodTrace trace,
		bool isOnEnter,
		string? moduleName,
		string? componentName,
		string? instanceTag,
		string? methodName,
		string? signal
		)
	{
		if (isOnEnter && trace.EnteredOn is null)
			throw new Exception("EnteredOn is null");

		if (!isOnEnter && trace.ExitedOn is null)
			throw new Exception("ExitedOn is null");

		return new WvConsoleLog()
		{
			CreatedOn = isOnEnter ? trace.EnteredOn!.Value : trace.ExitedOn!.Value,
			Type = isOnEnter ? "OnEnter" : "OnExit",
			Module = moduleName,
			Component = componentName,
			InstanceTag = instanceTag,
			Method = methodName,
			Signal = signal
		};
	}

	public static WvConsoleLog ConvertTraceToLog(this WvTraceSessionSignalTrace trace,
		string? moduleName,
		string? componentName,
		string? instanceTag,
		string? methodName,
		string? signal
		)
	{

		return new WvConsoleLog()
		{
			CreatedOn = trace.SendOn,
			Type = "Signal",
			Module = moduleName,
			Component = componentName,
			InstanceTag = instanceTag,
			Method = methodName,
			Signal = signal
		};
	}
}
