using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebVella.BlazorTrace.Services;
public class JsService
{
	protected IJSRuntime JSRuntime { get; }
	public JsService(IJSRuntime jsRuntime)
	{
		JSRuntime = jsRuntime;
	}
	public async ValueTask<bool> AddEscapeKeyEventListener(DotNetObjectReference<WvBlazorTrace> objectRef, string listenerId, string methodName)
	{
		try
		{
			return await JSRuntime.InvokeAsync<bool>(
				 "WebVellaBlazorTrace.addEscapeKeyEventListener",
				 objectRef, listenerId, methodName);
		}
		catch 
		{
		}
		return false;
	}

	public async ValueTask<bool> RemoveEscapeKeyEventListener(string listenerId)
	{
		try
		{
			return await JSRuntime.InvokeAsync<bool>(
			 "WebVellaBlazorTrace.removeEscapeKeyEventListener",
			 listenerId);
		}
		catch 
		{
		}
		return false;
	}
}
