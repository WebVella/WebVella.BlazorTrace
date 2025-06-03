using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Services;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceMuteMethodModal : WvBlazorTraceComponentBase
{
	// INJECTS
	//////////////////////////////////////////////////
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; } = default!;

	// PARAMETERS
	//////////////////////////////////////////////////
	[CascadingParameter(Name = "WvBlazorTraceBody")]
	public WvBlazorTraceBody WvBlazorTraceBody { get; set; } = default!;
	[Parameter] public int NestLevel { get; set; } = 1;
	[Parameter] public EventCallback OnChange { get; set; }


	// LOCAL VARIABLES
	//////////////////////////////////////////////////
	private Guid _componentId = Guid.NewGuid();
	private DotNetObjectReference<WvBlazorTraceMuteMethodModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvMethodTraceRow? _row = null;
	private List<WvTraceMute> _applicableTypes = new();
	private List<WvTraceMute> _selectedTypes = new();

	// LIFECYCLE
	/// //////////////////////////////////////////////
	public async ValueTask DisposeAsync()
	{
		if (_escapeListenerEnabled)
			await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape", _componentId.ToString());
		_objectRef?.Dispose();
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_objectRef = DotNetObjectReference.Create(this);
		EnableRenderLock();
	}

	protected override void OnParametersSet()
	{
		base.OnParametersSet();
		RegenRenderLock();
	}

	// PUBLIC
	//////////////////////////////////////////////////
	public async Task Show(WvMethodTraceRow row)
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_escapeListenerEnabled = true;
		_row = row;
		_initMuteOptions();
		_modalVisible = true;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}
	public async Task Hide(bool invokeStateChanged = true)
	{
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape", _componentId.ToString());
		_escapeListenerEnabled = false;
		_row = null;
		_modalVisible = false;
		RegenRenderLock();
		if (invokeStateChanged)
		{
			await InvokeAsync(StateHasChanged);
		}
	}

	[JSInvokable("OnShortcutKey")]
	public async Task OnShortcutKey(string code)
	{
		await Hide();
	}

	//PRIVATE
	/////////////////////////////////////////////////
	private string _getTitle()
	{
		if (_row is null) return String.Empty;

		var sb = new StringBuilder();
		sb.Append($"<span>Mute method trace</span>");

		return sb.ToString();
	}

	private async Task _typeClick(WvTraceMute item)
	{
		await WvBlazorTraceBody.MuteTraceChange(item);
		_selectedTypes = WvBlazorTraceBody.GetTraceMutes();
		await OnChange.InvokeAsync();
	}

	private void _initMuteOptions()
	{
		_applicableTypes = new();
		if (_row is not null)
		{
			_applicableTypes = new(){
			new WvTraceMute(WvTraceMuteType.MethodInComponentInstance,_row),
			new WvTraceMute(WvTraceMuteType.MethodInComponent,_row),
			new WvTraceMute(WvTraceMuteType.MethodInModule,_row),
			new WvTraceMute(WvTraceMuteType.Method,_row),
			new WvTraceMute(WvTraceMuteType.ComponentInstance,_row),
			new WvTraceMute(WvTraceMuteType.Component,_row),
			new WvTraceMute(WvTraceMuteType.Module,_row),
		};
			if (_row!.IsPinned)
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.PinnedMethods, _row));
			else
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.NotPinnedMethods, _row));
		}

		_selectedTypes = WvBlazorTraceBody.GetTraceMutes();
	}
}