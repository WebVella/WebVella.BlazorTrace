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
	void LogResult();
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
		var result = new WvTraceModalData();
		var store = await GetSnapshotStoreAsync();
		//Init request
		if (request is null || request.IsEmpty)
		{
			if (store.LastModalRequest is not null && !store.LastModalRequest.IsEmpty)
				request = store.LastModalRequest;
			else
				request = new();
		}
		result.Request = request;

		//Init snapshots
		result.SnapshotOptions = new(){
			new WvSelectOption{Value = null, Label = "current"}
		};
		foreach (var sn in store.Snapshots)
		{
			result.SnapshotOptions.Add(new WvSelectOption { Value = sn.Id.ToString(), Label = sn.Name });
		}

		WvSnapshot primarySN = new();
		WvSnapshot secondarySN = new();
		WvSnapshot currentSN = new()
		{
			CreatedOn = DateTime.Now,
			Id = Guid.Empty,
			ModuleDict = _moduleDict,
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

		result.TraceRows = WvModalUtility.GenerateTraceRows(
			primarySn: primarySN,
			secondarySn: secondarySN
		);
		return result;
	}

	public void LogResult()
	{
		foreach (var moduleName in _moduleDict.Keys)
		{
			var module = _moduleDict[moduleName];
			Console.WriteLine("------------------------------------------------------");
			Console.WriteLine($">>> {moduleName}");
			Console.WriteLine("------------------------------------------------------");
			foreach (var compFullName in module.ComponentDict.Keys.Order())
			{
				var component = module.ComponentDict[compFullName];
				foreach (var method in component.MethodsTotal(includeNotCalled: false))
				{
					Console.WriteLine($@"::: {component.Name} > {method.Name} ::: 
					enters/exits: {method.OnEnterCallsCount}/{method.OnExitCallsCount} 
					duration min/max: {method.MinDurationMs}ms / {method.MaxDurationMs}ms
					memory min/max: {(method.OnEnterMinMemoryBytes ?? 0).ToKilobytes()} KB / {(method.OnExitMaxMemoryBytes ?? 0).ToKilobytes()} KB");
				}
			}
		}
	}

}

