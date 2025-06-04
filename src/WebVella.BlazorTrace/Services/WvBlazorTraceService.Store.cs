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
	Task<WvSnapshot> RenameSnapshotAsync(Guid id, string? name = null);
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
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
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
		store.Snapshots.Add(snapshot);
		var json = JsonSerializer.Serialize(store);
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		return snapshot;
	}
	public async Task<WvSnapshot> RenameSnapshotAsync(Guid id, string? name = null)
	{
		var store = await GetLocalStoreAsync();
		var snapshot = store.Snapshots.FirstOrDefault(x => x.Id == id);
		if (snapshot is null)
			throw new Exception("Snapshot not found");

		if (name == "current") return snapshot;
		snapshot.Name = !String.IsNullOrWhiteSpace(name) ? name : snapshot.CreatedOn.ToString("yyyy-MM-dd-HH-mm-ss");
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		return snapshot;
	}
	public async Task<WvLocalStore> GetLocalStoreAsync()
	{
		try
		{
			var storeJson = await _getUnprotectedLocalStorageAsync(_snapshotStoreKey);
			if (String.IsNullOrWhiteSpace(storeJson))
				return new WvLocalStore();
			var store = JsonSerializer.Deserialize<WvLocalStore>(storeJson);
			if (store is null)
				return new WvLocalStore();
			return store;
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	public async Task<WvSnapshot?> GetSnapshotAsync(Guid id)
	{
		var store = await GetLocalStoreAsync();
		return store.Snapshots.FirstOrDefault(x => x.Id == id);
	}
	public async Task RemoveSnapshotAsync(Guid id)
	{
		var store = await GetLocalStoreAsync();
		store.Snapshots = store.Snapshots.Where(x => x.Id != id).ToList();
		if (store.LastModalRequest is not null && store.LastModalRequest.PrimarySnapshotId == id)
			store.LastModalRequest.PrimarySnapshotId = null;
		if (store.LastModalRequest is not null && store.LastModalRequest.SecondarySnapshotId == id)
			store.LastModalRequest.SecondarySnapshotId = null;

		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task AddPinAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		if (!store.Pins.Any(x => x == id))
		{
			store.Pins.Add(id);
			await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		}
	}

	public async Task RemovePinAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		store.Pins = store.Pins.Where(x => x != id).ToList();
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task SaveLastestRequestAsync(WvTraceModalRequest? request)
	{
		var store = await GetLocalStoreAsync();
		store.LastModalRequest = request;
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
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
			await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		}
		_traceMutesInternal = store.MutedTraces;
	}
	public async Task RemoveTraceMuteAsync(WvTraceMute traceMute)
	{
		var store = await GetLocalStoreAsync();
		store.MutedTraces = store.MutedTraces.Where(x => x.Id != traceMute.Id).ToList();
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		_traceMutesInternal = store.MutedTraces;
	}

}

