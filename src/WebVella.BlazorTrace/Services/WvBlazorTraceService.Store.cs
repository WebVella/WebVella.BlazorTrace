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
	public async Task<WvLocalStore> GetLocalStoreAsync()
	{
		var storeJson = await new JsService(_jSRuntime).GetUnprotectedLocalStorageAsync(_generalStoreKey);
		if (String.IsNullOrWhiteSpace(storeJson))
			return new WvLocalStore();
		var store = JsonSerializer.Deserialize<WvLocalStore>(storeJson);
		if (store is null)
			return new WvLocalStore();
		return store;
	}
	public async Task<WvSnapshotStore?> GetLocalSnapshotStoreByIdAsync(Guid id)
	{
		return await GetLocalSnapshotStoreByKeyAsync($"{_snapshotStoreKeyPrefix}{id}");
	}

	public async Task<WvSnapshotStore?> GetLocalSnapshotStoreByKeyAsync(string snapshotStoreKey)
	{
		var storedSnapshotKeys = await new JsService(_jSRuntime).GetUnprotectedLocalStorageKeysAsync(_snapshotStoreKeyPrefix);
		if (!storedSnapshotKeys.Any(x => x == snapshotStoreKey)) return null;
		var snapshotJson = await new JsService(_jSRuntime).GetUnprotectedLocalStorageAsync(snapshotStoreKey);
		if (String.IsNullOrWhiteSpace(snapshotJson)) return null;
		var storeSn = JsonSerializer.Deserialize<WvSnapshotStore>(snapshotJson);
		return storeSn;
	}

	public async Task SaveModalRequestAsync(WvTraceModalRequest? request)
	{
		var store = await GetLocalStoreAsync();
		store.LastModalRequest = request;
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));
	}
	public async Task<WvSnapshot> CreateSnapshotAsync(string? name)
	{
		var snapshot = new WvSnapshot
		{
			Id = Guid.NewGuid(),
			CreatedOn = DateTimeOffset.Now,
			Name = !String.IsNullOrWhiteSpace(name) ? name : DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
			ModuleDict = await GetModuleDictAsync(),
			SignalDict = await GetSignalDictAsync(),
		};
		WvSnapshotStore snapshotStore = new WvSnapshotStore
		{
			Id = snapshot.Id,
			CreatedOn = snapshot.CreatedOn,
			Name = snapshot.Name,
			CompressedModuleDict = JsonSerializer.Serialize(snapshot.ModuleDict).CompressString(),
			CompressedSignalDict = JsonSerializer.Serialize(snapshot.SignalDict).CompressString(),
		};
		var json = JsonSerializer.Serialize(snapshotStore);
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync($"{_snapshotStoreKeyPrefix}{snapshot.Id}", json);
		return snapshot;
	}
	public async Task RenameSnapshotAsync(Guid id, string? name = null)
	{
		WvSnapshotStore? snapshotStore = await GetLocalSnapshotStoreByIdAsync(id);
		if (snapshotStore is null)
			throw new Exception("Snapshot not found");

		if (name == "current") return;
		snapshotStore.Name = !String.IsNullOrWhiteSpace(name) ? name : snapshotStore.CreatedOn.ToString("yyyy-MM-dd-HH-mm-ss");
		var snapshotStoreKey = $"{_snapshotStoreKeyPrefix}{id}";

		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(snapshotStoreKey, JsonSerializer.Serialize(snapshotStore));
	}
	public async Task<WvSnapshot?> GetSnapshotAsync(Guid id)
	{
		var snapshotStoreKey = $"{_snapshotStoreKeyPrefix}{id}";
		var storedSnapshotKeys = await new JsService(_jSRuntime).GetUnprotectedLocalStorageKeysAsync(_snapshotStoreKeyPrefix);
		if (!storedSnapshotKeys.Any(x => x == snapshotStoreKey)) return null;
		var snapshotJson = await new JsService(_jSRuntime).GetUnprotectedLocalStorageAsync(snapshotStoreKey);
		if (String.IsNullOrWhiteSpace(snapshotJson)) return null;
		var storeSn = await GetLocalSnapshotStoreByIdAsync(id);
		if (storeSn is null) return null;

		var sn = new WvSnapshot
		{
			Id = storeSn.Id,
			CreatedOn = storeSn.CreatedOn,
			Name = storeSn.Name,
			ModuleDict = new(),
			SignalDict = new()
		};
		if (!String.IsNullOrWhiteSpace(storeSn.CompressedModuleDict))
			sn.ModuleDict = JsonSerializer.Deserialize<Dictionary<string, WvTraceSessionModule>>(storeSn.CompressedModuleDict.DecompressString()) ?? new();
		if (!String.IsNullOrWhiteSpace(storeSn.CompressedSignalDict))
			sn.SignalDict = JsonSerializer.Deserialize<Dictionary<string, WvTraceSessionSignal>>(storeSn.CompressedSignalDict.DecompressString()) ?? new();

		return sn;

	}
	public async Task<List<WvSnapshotListItem>> GetExistingSnapshots()
	{
		var result = new List<WvSnapshotListItem>();
		var storedSnapshotKeys = await new JsService(_jSRuntime).GetUnprotectedLocalStorageKeysAsync(_snapshotStoreKeyPrefix);
		foreach (var snKey in storedSnapshotKeys)
		{
			var snStore = await GetLocalSnapshotStoreByKeyAsync(snKey);
			if (snStore == null) continue;
			result.Add(new WvSnapshotListItem
			{
				CreatedOn = snStore.CreatedOn,
				Name = snStore.Name,
				Id = snStore.Id,
			});
		}
		return result.OrderBy(x => x.Name).ToList();
	}
	public async Task RemoveSnapshotAsync(Guid id)
	{
		var store = await GetLocalStoreAsync();
		if (store.LastModalRequest is not null && store.LastModalRequest.PrimarySnapshotId == id)
			store.LastModalRequest.PrimarySnapshotId = null;
		if (store.LastModalRequest is not null && store.LastModalRequest.SecondarySnapshotId == id)
			store.LastModalRequest.SecondarySnapshotId = null;
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));

		var snapshotStore = await GetLocalSnapshotStoreByIdAsync(id);
		var snapshotStoreKey = $"{_snapshotStoreKeyPrefix}{id}";
		if (snapshotStore is null) return;
		await new JsService(_jSRuntime).RemoveUnprotectedLocalStorageAsync(snapshotStoreKey);
	}

	public async Task AddPinAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		if (!store.Pins.Any(x => x == id))
		{
			store.Pins.Add(id);
			await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));
		}
	}

	public async Task RemovePinAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		store.Pins = store.Pins.Where(x => x != id).ToList();
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task SaveLastestRequestAsync(WvTraceModalRequest? request)
	{
		var store = await GetLocalStoreAsync();
		store.LastModalRequest = request;
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));
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
			await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));
		}
	}
	public async Task RemoveTraceMuteAsync(WvTraceMute traceMute)
	{
		var store = await GetLocalStoreAsync();
		store.MutedTraces = store.MutedTraces.Where(x => x.Id != traceMute.Id).ToList();
		await new JsService(_jSRuntime).SetUnprotectedLocalStorageAsync(_generalStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task<Guid> GetSessionId()
	{
		Guid sessionId = Guid.Empty;
		var sessionIdString = await new JsService(_jSRuntime).GetUnprotectedSessionStorageAsync(_sessionStoreKey);
		if (!String.IsNullOrWhiteSpace(sessionIdString))
		{
			if (Guid.TryParse(sessionIdString, out Guid outGuid))
			{
				sessionId = outGuid;
			}
		}
		if (sessionId == Guid.Empty)
		{
			sessionId = Guid.NewGuid();
			await new JsService(_jSRuntime).SetUnprotectedSessionStorageAsync(_sessionStoreKey, sessionId.ToString());
		}
		return sessionId;
	}

}

