﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial interface IWvBlazorTraceService
{
	Task<WvTraceModalData> GetModalDataAsync(IJSRuntime jsRuntime, WvTraceModalRequest? request);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	/// <summary>
	/// Gets data for the trace modal
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<WvTraceModalData> GetModalDataAsync(IJSRuntime jsRuntime, WvTraceModalRequest? request)
	{
		ProcessQueue();
		var result = new WvTraceModalData();
		var store = await GetLocalStoreAsync(jsRuntime);
		//Init request
		if (request is null || request.IsEmpty)
		{
			if (store.LastModalRequest is not null && !store.LastModalRequest.IsEmpty)
				request = store.LastModalRequest;
			else
				request = new() { Menu = WvTraceModalMenu.MethodCalls };
		}
		result.Request = request;
		await SaveLastestRequestAsync(jsRuntime, request);
		//Init snapshots
		result.SnapshotList = await GetExistingSnapshots(jsRuntime);
		WvSnapshot primarySN = new();
		WvSnapshot secondarySN = new();
		WvSnapshot currentSN = new()
		{
			CreatedOn = DateTimeOffset.Now,
			Id = Guid.Empty,
			ModuleDict = GetModuleDict(),
			SignalDict = GetSignalDict(),
			Name = "current"
		};
		if (request.PrimarySnapshotId.HasValue)
		{
			var snapshot = await GetSnapshotAsync(jsRuntime, request.PrimarySnapshotId.Value);
			if (snapshot is null) throw new Exception($"Primary snapshot not found");
			primarySN = snapshot;
		}
		else
		{
			primarySN = currentSN;
		}
		if (request.SecondarySnapshotId.HasValue)
		{
			var snapshot = await GetSnapshotAsync(jsRuntime, request.SecondarySnapshotId.Value);
			if (snapshot is null) throw new Exception($"Secondary snapshot not found");
			secondarySN = snapshot;
		}
		else
		{
			secondarySN = currentSN;
		}

		if (result.Request.IsMethodDataMenu)
		{
			var traceRows = primarySN.GenerateMethodTraceRows(
				secondarySn: secondarySN,
				muteTraces: store.MutedTraces,
				pins: store.Pins
			);
			foreach (var row in traceRows)
			{
				if (!row.ModuleMatches(result.Request.MethodsFilter.ModuleFilter)) continue;
				if (!row.ComponentMatches(result.Request.MethodsFilter.ComponentFilter)) continue;
				if (!row.MethodMatches(result.Request.MethodsFilter.MethodFilter)) continue;
				if (!row.CallsMatches(result.Request.MethodsFilter.CallsFilter)) continue;
				if (!row.MemoryMatches(result.Request.MethodsFilter.MemoryFilter)) continue;
				if (!row.DurationMatches(result.Request.MethodsFilter.DurationFilter)) continue;
				if (!row.LimitMatches(result.Request.MethodsFilter.LimitsFilter)) continue;

				if (store.Pins.Contains(row.Id))
					row.IsPinned = true;
				else
					row.IsPinned = false;
				result.MethodTraceRows.Add(row);
			}
			if (result.Request.Menu == WvTraceModalMenu.MethodCalls)
			{
				result.MethodTraceRows = result.MethodTraceRows.OrderByDescending(x => x.TraceList.Count).ToList();
			}
			else if (result.Request.Menu == WvTraceModalMenu.MethodMemory)
			{
				result.MethodTraceRows = result.MethodTraceRows.OrderByDescending(x => x.LastMemoryBytes).ToList();
			}
			else if (result.Request.Menu == WvTraceModalMenu.MethodDuration)
			{
				result.MethodTraceRows = result.MethodTraceRows.OrderByDescending(x => x.LastDurationMS).ToList();
			}
			else if (result.Request.Menu == WvTraceModalMenu.MethodLimits)
			{
				result.MethodTraceRows = result.MethodTraceRows.OrderByDescending(x => x.LimitHits.Count).ToList();
			}
			else if (result.Request.Menu == WvTraceModalMenu.MethodName)
			{
				result.MethodTraceRows = result.MethodTraceRows.OrderBy(x => $"{x.Module}{x.ComponentFullName}{x.Method}").ToList();
			}
		}
		else if (result.Request.IsMethodLogMenu)
		{
			result.MethodLog = primarySN.GenerateMethodLogRows(muteTraces: store.MutedTraces);
		}
		else if (result.Request.IsSignalDataMenu)
		{
			var signalTraceRows = primarySN.GenerateSignalTraceRows(
				secondarySn: secondarySN,
				muteTraces: store.MutedTraces,
				pins: store.Pins
			);
			foreach (var row in signalTraceRows)
			{
				if (!row.SignalNameMatches(result.Request.SignalsFilter.SignalNameFilter)) continue;
				if (!row.CallsMatches(result.Request.SignalsFilter.CallsFilter)) continue;
				if (!row.LimitMatches(result.Request.SignalsFilter.LimitsFilter)) continue;

				if (store.Pins.Contains(row.Id))
					row.IsPinned = true;
				else
					row.IsPinned = false;
				result.SignalTraceRows.Add(row);
			}
			if (result.Request.Menu == WvTraceModalMenu.SignalCalls)
			{
				result.SignalTraceRows = result.SignalTraceRows.OrderByDescending(x => x.TraceList.Count).ToList();
			}
			else if (result.Request.Menu == WvTraceModalMenu.MethodLimits)
			{
				result.SignalTraceRows = result.SignalTraceRows.OrderByDescending(x => x.LimitHits.Count).ToList();
			}
			else if (result.Request.Menu == WvTraceModalMenu.MethodName)
			{
				result.SignalTraceRows = result.SignalTraceRows.OrderBy(x => $"{x.SignalName}").ToList();
			}
		}
		else if (result.Request.IsSignalLogMenu)
		{
			result.SignalLog = primarySN.GenerateSignalLogRows(muteTraces: store.MutedTraces);
		}
		else if (result.Request.IsTraceMuteMenu)
		{
			var traceRows = await GetTraceMutes(jsRuntime);
			foreach (var row in traceRows)
			{
				if (!row.IsTypeMatches(result.Request.MutedFilter.TypeFilter)) continue;
				if (!row.ModuleMatches(result.Request.MutedFilter.ModuleFilter)) continue;
				if (!row.ComponentMatches(result.Request.MutedFilter.ComponentFilter)) continue;
				if (!row.InstanceTagMatches(result.Request.MutedFilter.InstanceTag)) continue;
				if (!row.MethodMatches(result.Request.MutedFilter.MethodFilter)) continue;
				if (!row.SignalMatches(result.Request.MutedFilter.SignalFilter)) continue;
				if (!row.FieldMatches(result.Request.MutedFilter.FieldFilter)) continue;
				if (!row.CustomDataMatches(result.Request.MutedFilter.CustomDataFilter)) continue;
				if (!row.IsPinnedMatches(result.Request.MutedFilter.PinFilter)) continue;
				result.MutedTraceRows.Add(row);
			}
			result.MutedTraceRows = result.MutedTraceRows.OrderBy(x => $"{x.Id}").ToList();
		}

		return result;
	}
}

