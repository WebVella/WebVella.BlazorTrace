using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Services;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial class WvBlazorMemoryModal : WvBlazorTraceComponentBase
{
	// PARAMS
	//////////////////////////////////////////////////
	[Parameter] public WvTraceRow? Row { get; set; } = null;
	[Parameter] public EventCallback OnHide { get; set; }

	// LOCAL VARIABLES
	//////////////////////////////////////////////////
	private Guid _currentRenderLock = Guid.Empty;
	private Guid _oldRenderLock = Guid.Empty;

	// UI HANDLERS
	//////////////////////////////////////////////////
	private async Task _hide(){ 
		await OnHide.InvokeAsync();
	}

	//PRIVATE
	/////////////////////////////////////////////////
	private string _getTitle(){ 
		if(Row is null) return String.Empty;

		var sb = new StringBuilder();
		sb.Append($"<span>{Row.Component}</span>");
		if(!String.IsNullOrWhiteSpace(Row.Tag)){ 
			sb.Append($" <span class='tag' style='margin-left:5px'>{Row.Tag}</span>");
		}
		sb.Append("<span class='wv-trace-modal__divider'></span>");
		sb.Append($"<span>{Row.Method}</span>");

		return sb.ToString();
	}

}