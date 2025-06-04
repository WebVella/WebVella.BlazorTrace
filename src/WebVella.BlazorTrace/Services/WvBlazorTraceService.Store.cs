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
	Task<WvSnapshot> CreateSnapshotAsync(string? name = null);
	Task RenameSnapshotAsync(Guid id, string? name = null);
	Task<WvLocalStore> GetLocalStoreAsync();
	Task<WvSnapshot?> GetSnapshotAsync(Guid id);
	Task RemoveSnapshotAsync(Guid id);
	Task AddPinAsync(string id);
	Task RemovePinAsync(string id);
	Task ToggleTraceMuteAsync(WvTraceMute traceMute);
	Task AddTraceMuteAsync(WvTraceMute traceMute);
	Task RemoveTraceMuteAsync(WvTraceMute traceMute);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	public async Task SaveModalRequestAsync(WvTraceModalRequest? request)
	{
		var store = await GetLocalStoreAsync();
		store.LastModalRequest = request;
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}
	public async Task<WvSnapshot> CreateSnapshotAsync(string? name)
	{
		var store = await GetLocalStoreAsync();
		var snapshot = new WvSnapshot
		{
			Id = Guid.NewGuid(),
			CreatedOn = DateTimeOffset.Now,
			Name = !String.IsNullOrWhiteSpace(name) ? name : DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
			ModuleDict = GetModuleDict(),
			SignalDict = GetSignalDict(),
		};
		WvSnapshotStore snapshotStore = new WvSnapshotStore
		{
			Id = snapshot.Id,
			CreatedOn = snapshot.CreatedOn,
			Name = snapshot.Name,
			CompressedModuleDict = JsonSerializer.Serialize(snapshot.ModuleDict).CompressString(),
			CompressedSignalDict = JsonSerializer.Serialize(snapshot.SignalDict).CompressString(),
		};
		store.Snapshots.Add(snapshotStore);
		var json = JsonSerializer.Serialize(store);
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, json);
		return snapshot;
	}
	public async Task RenameSnapshotAsync(Guid id, string? name = null)
	{
		var store = await GetLocalStoreAsync();
		var snapshot = store.Snapshots.FirstOrDefault(x => x.Id == id);
		if (snapshot is null)
			throw new Exception("Snapshot not found");

		if (name == "current") return;
		snapshot.Name = !String.IsNullOrWhiteSpace(name) ? name : snapshot.CreatedOn.ToString("yyyy-MM-dd-HH-mm-ss");
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}
	public async Task<WvLocalStore> GetLocalStoreAsync()
	{
		var storeJson = await new JsService(_jSRuntime).GetUnprotectedLocalStorageAsync(_snapshotStoreKey);
		if (String.IsNullOrWhiteSpace(storeJson))
			return new WvLocalStore();
		var store = JsonSerializer.Deserialize<WvLocalStore>(storeJson);
		if (store is null)
			return new WvLocalStore();
		return store;
	}
	public async Task<WvSnapshot?> GetSnapshotAsync(Guid id)
	{
		var store = await GetLocalStoreAsync();
		var storeSn = store.Snapshots.FirstOrDefault(x => x.Id == id);
		if (storeSn is null) return null;
		var sn = new WvSnapshot
		{
			Id = storeSn.Id,
			CreatedOn = storeSn.CreatedOn,
			Name = storeSn.Name,
			ModuleDict = new(),
			SignalDict = new()
		};
		if(!String.IsNullOrWhiteSpace(storeSn.CompressedModuleDict)) 
			sn.ModuleDict = JsonSerializer.Deserialize<Dictionary<string, WvTraceSessionModule>>(storeSn.CompressedModuleDict.DecompressString()) ?? new();
		if(!String.IsNullOrWhiteSpace(storeSn.CompressedSignalDict)) 
			sn.SignalDict = JsonSerializer.Deserialize<Dictionary<string, WvTraceSessionSignal>>(storeSn.CompressedSignalDict.DecompressString()) ?? new();		

		return sn;

	}
	public async Task RemoveSnapshotAsync(Guid id)
	{
		var store = await GetLocalStoreAsync();
		store.Snapshots = store.Snapshots.Where(x => x.Id != id).ToList();
		if (store.LastModalRequest is not null && store.LastModalRequest.PrimarySnapshotId == id)
			store.LastModalRequest.PrimarySnapshotId = null;
		if (store.LastModalRequest is not null && store.LastModalRequest.SecondarySnapshotId == id)
			store.LastModalRequest.SecondarySnapshotId = null;

		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task AddPinAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		if (!store.Pins.Any(x => x == id))
		{
			store.Pins.Add(id);
			await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		}
	}

	public async Task RemovePinAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		store.Pins = store.Pins.Where(x => x != id).ToList();
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task SaveLastestRequestAsync(WvTraceModalRequest? request)
	{
		var store = await GetLocalStoreAsync();
		store.LastModalRequest = request;
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task ToggleTraceMuteAsync(WvTraceMute traceMute)
	{
		var traceMutes = await GetTraceMutes();
		if (traceMutes.Any(x => x.Id == traceMute.Id))
			await RemoveTraceMuteAsync(traceMute);
		else
			await AddTraceMuteAsync(traceMute);
	}

	public async Task AddTraceMuteAsync(WvTraceMute traceMute)
	{
		var store = await GetLocalStoreAsync();

		if (!store.MutedTraces.Any(x => x.Id == traceMute.Id))
		{
			store.MutedTraces.Add(traceMute);
			await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		}
		_traceMutesInternal = store.MutedTraces;
	}
	public async Task RemoveTraceMuteAsync(WvTraceMute traceMute)
	{
		var store = await GetLocalStoreAsync();
		store.MutedTraces = store.MutedTraces.Where(x => x.Id != traceMute.Id).ToList();
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		_traceMutesInternal = store.MutedTraces;
	}

}

