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
	Task CreateSnapshotAsync(string? name = null);
	Task RenameSnapshotAsync(Guid id, string? name = null);
	Task<WvSnapshotStore> GetSnapshotStoreAsync();
	Task<WvSnapshot?> GetSnapshotAsync(Guid id);
	Task RemoveSnapshotAsync(Guid id);
}
public partial class WvBlazorTraceService : IWvBlazorTraceService
{
	public async Task SaveModalRequest(WvTraceModalRequest? request)
	{
		var store = await GetSnapshotStoreAsync();
		store.LastModalRequest = request;
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}
	public async Task CreateSnapshotAsync(string? name)
	{
		var store = await GetSnapshotStoreAsync();
		var snapshot = new WvSnapshot
		{
			Id = Guid.NewGuid(),
			CreatedOn = DateTimeOffset.Now,
			Name = !String.IsNullOrWhiteSpace(name) ? name : DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
			ModuleDict = _moduleDict
		};
		store.Snapshots.Add(snapshot);
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}
	public async Task RenameSnapshotAsync(Guid id, string? name = null)
	{
		var store = await GetSnapshotStoreAsync();
		var snapshot = store.Snapshots.FirstOrDefault(x => x.Id == id);
		if (snapshot is null)
			throw new Exception("Snapshot not found");
		snapshot.Name = !String.IsNullOrWhiteSpace(name) ? name : DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}
	public async Task<WvSnapshotStore> GetSnapshotStoreAsync()
	{
		var storeJson = await _getUnprotectedLocalStorageAsync(_snapshotStoreKey);
		if (String.IsNullOrWhiteSpace(storeJson))
			return new WvSnapshotStore();

		var store = JsonSerializer.Deserialize<WvSnapshotStore>(storeJson);
		if (store is null)
			return new WvSnapshotStore();

		return store;
	}
	public async Task<WvSnapshot?> GetSnapshotAsync(Guid id)
	{
		var store = await GetSnapshotStoreAsync();
		return store.Snapshots.FirstOrDefault(x => x.Id == id);
	}
	public async Task RemoveSnapshotAsync(Guid id)
	{
		var store = await GetSnapshotStoreAsync();
		store.Snapshots = store.Snapshots.Where(x => x.Id != id).ToList();
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task AddBookmarkAsync(string id)
	{
		var store = await GetSnapshotStoreAsync();
		if (!store.Bookmarked.Any(x => x == id))
		{
			store.Bookmarked.Add(id);
		}
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}

	public async Task RemoveBookmarkAsync(string id)
	{
		var store = await GetSnapshotStoreAsync();
		store.Bookmarked = store.Bookmarked.Where(x=> x != id).ToList();
		await _setUnprotectedLocalStorageAsync(_snapshotStoreKey, JsonSerializer.Serialize(store));
	}
}

