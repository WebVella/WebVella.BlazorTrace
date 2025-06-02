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
	Task AddBookmarkAsync(string id);
	Task RemoveBookmarkAsync(string id);

	Task<List<WvTraceMute>> GetAllTraceMutesAsync();
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
			ModuleDict = _moduleDict,
			SignalDict = _signalDict,
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
		var storeJson = await _getUnprotectedLocalStorageAsync(_snapshotStoreKey);
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

	public async Task AddBookmarkAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		if (!store.Bookmarked.Any(x => x == id))
		{
			store.Bookmarked.Add(id);
			await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		}
	}

	public async Task RemoveBookmarkAsync(string id)
	{
		var store = await GetLocalStoreAsync();
		store.Bookmarked = store.Bookmarked.Where(x => x != id).ToList();
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task SaveLastestRequestAsync(WvTraceModalRequest? request)
	{
		var store = await GetLocalStoreAsync();
		store.LastModalRequest = request;
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task<List<WvTraceMute>> GetAllTraceMutesAsync()
	{
		if (_traceMutes is null)
		{
			var store = await GetLocalStoreAsync();
			_traceMutes = store.TraceMutes;
		}
		if (_traceMutes is null)
			_traceMutes = new();
		return _traceMutes;
	}

	public async Task ToggleTraceMuteAsync(WvTraceMute traceMute)
	{
		var traceMutes = await GetAllTraceMutesAsync();
		if (traceMutes.Any(x => x.Id == traceMute.Id))
			await RemoveTraceMuteAsync(traceMute);
		else
			await AddTraceMuteAsync(traceMute);
	}

	public async Task AddTraceMuteAsync(WvTraceMute traceMute)
	{
		var store = await GetLocalStoreAsync();

		if (!store.TraceMutes.Any(x => x.Id == traceMute.Id))
		{
			store.TraceMutes.Add(traceMute);
			await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		}
		_traceMutes = store.TraceMutes;
	}
	public async Task RemoveTraceMuteAsync(WvTraceMute traceMute)
	{
		var store = await GetLocalStoreAsync();
		store.TraceMutes = store.TraceMutes.Where(x => x.Id != traceMute.Id).ToList();
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
		_traceMutes = store.TraceMutes;
	}

}

