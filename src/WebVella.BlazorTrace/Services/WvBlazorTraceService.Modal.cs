using Microsoft.AspNetCore.Components;
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
	Task<WvTraceModalData> GetModalData(WvTraceModalRequest? request);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	/// <summary>
	/// Gets data for the trace modal
	/// </summary>
	/// <param name="request"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public async Task<WvTraceModalData> GetModalData(WvTraceModalRequest? request)
	{
		ForceProcessQueue();
		var result = new WvTraceModalData();
		var store = await GetSnapshotStoreAsync();
		//Init request
		if (request is null || request.IsEmpty)
		{
			if (store.LastModalRequest is not null && !store.LastModalRequest.IsEmpty)
				request = store.LastModalRequest;
			else
				request = new() { Menu = WvTraceModalMenu.MethodCalls };
		}
		result.Request = request;
		await SaveLastestRequestAsync(request);
		//Init snapshots
		result.SnapshotList = new();
		foreach (var sn in store.Snapshots)
		{
			result.SnapshotList.Add(new WvSnapshotListItem
			{
				Id = sn.Id,
				Name = sn.Name,
				CreatedOn = sn.CreatedOn
			});
		}
		result.SnapshotList = result.SnapshotList.OrderBy(x => x.Name).ToList();
		WvSnapshot primarySN = new();
		WvSnapshot secondarySN = new();
		WvSnapshot currentSN = new()
		{
			CreatedOn = DateTimeOffset.Now,
			Id = Guid.Empty,
			ModuleDict = _moduleDict,
			SignalDict = _signalDict,
			Name = "current"
		};
		if (request.PrimarySnapshotId.HasValue)
		{
			var snapshot = store.Snapshots.FirstOrDefault(x => x.Id == request.PrimarySnapshotId);
			if (snapshot is null) throw new Exception($"Primary snapshot not found");
			primarySN = snapshot;
		}
		else
		{
			primarySN = currentSN;
		}
		if (request.SecondarySnapshotId.HasValue)
		{
			var snapshot = store.Snapshots.FirstOrDefault(x => x.Id == request.SecondarySnapshotId);
			if (snapshot is null) throw new Exception($"Secondary snapshot not found");
			secondarySN = snapshot;
		}
		else
		{
			secondarySN = currentSN;
		}

		if (result.Request.IsMethodMenu)
		{
			var traceRows = primarySN.GenerateMethodTraceRows(
				secondarySn: secondarySN
			);
			foreach (var row in traceRows)
			{
				if (!row.ModuleMatches(result.Request.ModuleFilter)) continue;
				if (!row.ComponentMatches(result.Request.ComponentFilter)) continue;
				if (!row.MethodMatches(result.Request.MethodFilter)) continue;
				if (!row.CallsMatches(result.Request.CallsFilter)) continue;
				if (!row.MemoryMatches(result.Request.MemoryFilter)) continue;
				if (!row.DurationMatches(result.Request.DurationFilter)) continue;
				if (!row.LimitMatches(result.Request.LimitsFilter)) continue;

				if (store.Bookmarked.Contains(row.Id))
					row.IsBookmarked = true;
				else
					row.IsBookmarked = false;
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
		else if(result.Request.IsSignalMenu){ 
			var signalTraceRows = primarySN.GenerateSignalTraceRows(
				secondarySn: secondarySN
			);		
			foreach (var row in signalTraceRows)
			{
				if (!row.SignalNameMatches(result.Request.SignalNameFilter)) continue;
				if (!row.CallsMatches(result.Request.CallsFilter)) continue;
				if (!row.LimitMatches(result.Request.LimitsFilter)) continue;

				if (store.Bookmarked.Contains(row.Id))
					row.IsBookmarked = true;
				else
					row.IsBookmarked = false;
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
		return result;
	}
}

