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
	public async ValueTask<bool> AddKeyEventListener(DotNetObjectReference<WvBlazorTrace> objectRef, string methodName, string keyCode)
	{
		try
		{
			if (keyCode == "Escape")
				return await JSRuntime.InvokeAsync<bool>(
					 "WebVellaBlazorTrace.addEscapeKeyEventListener",
					 objectRef, methodName);
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

	public async ValueTask<bool> RemoveKeyEventListener(string keyCode)
	{
		try
		{
			if (keyCode == "Escape")
				return await JSRuntime.InvokeAsync<bool>(
				 "WebVellaBlazorTrace.removeEscapeKeyEventListener");
			else if (keyCode == "F1")
				return await JSRuntime.InvokeAsync<bool>(
				 "WebVellaBlazorTrace.removeF1KeyEventListener");
		}
		catch
		{
		}
		return false;
	}
}
