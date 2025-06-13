using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public class JsService
{
	protected IJSRuntime JSRuntime { get; }
	public JsService(IJSRuntime jsRuntime)
	{
		JSRuntime = jsRuntime;
	}
	public async ValueTask<bool> AddKeyEventListener(object objectRef, string methodName, string keyCode, string? listenerId = null)
	{
		try
		{
			if (keyCode == "Escape")
				return await JSRuntime.InvokeAsync<bool>(
					 "WebVellaBlazorTrace.addEscapeKeyEventListener",
					 objectRef, listenerId, methodName);
			else if (keyCode == "F1")
				return await JSRuntime.InvokeAsync<bool>(
					 "WebVellaBlazorTrace.addF1KeyEventListener",
					 objectRef, methodName);
		}
		catch
		{
		}
		return false;
	}

	public async ValueTask<bool> RemoveKeyEventListener(string keyCode, string? listenerId = null)
	{
		try
		{
			if (keyCode == "Escape")
				return await JSRuntime.InvokeAsync<bool>(
				 "WebVellaBlazorTrace.removeEscapeKeyEventListener", listenerId);
			else if (keyCode == "F1")
				return await JSRuntime.InvokeAsync<bool>(
				 "WebVellaBlazorTrace.removeF1KeyEventListener");
		}
		catch
		{
		}
		return false;
	}

	public async Task SetUnprotectedLocalStorageAsync(string key, string value)
	{
		await JSRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
	}

	public async Task RemoveUnprotectedLocalStorageAsync(string key)
	{
		await JSRuntime.InvokeVoidAsync("localStorage.removeItem", key);
	}

	public async Task<string?> GetUnprotectedLocalStorageAsync(string key)
	{
		try
		{
			return await JSRuntime.InvokeAsync<string?>("localStorage.getItem", key);
		}
		catch
		{
			return null;
		}

	}

	public async Task<List<string>> GetUnprotectedLocalStorageKeysAsync(string prefix)
	{
		try
		{
			return await JSRuntime.InvokeAsync<List<string>>("WebVellaBlazorTrace.getLocalStorageKeysWithPrefix", prefix);
		}
		catch
		{
			return new List<string>();
		}
	}

	public async Task SetUnprotectedSessionStorageAsync(string key, string value)
	{
		await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
	}

	public async Task RemoveUnprotectedSessionStorageAsync(string key)
	{
		await JSRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
	}

	public async Task<string?> GetUnprotectedSessionStorageAsync(string key)
	{
		try
		{
			return await JSRuntime.InvokeAsync<string?>("sessionStorage.getItem", key);
		}
		catch
		{
			return null;
		}
	}
}
