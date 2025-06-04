using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceMuteLimitModal : WvBlazorTraceComponentBase
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
	private DotNetObjectReference<WvBlazorTraceMuteLimitModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvMethodTraceRow? _row = null;
	private WvTraceSessionLimitHit? _limitHit = null;
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
	public async Task Show(WvMethodTraceRow row, WvTraceSessionLimitHit dataField)
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_escapeListenerEnabled = true;
		_row = row;
		_limitHit = dataField;
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
		_limitHit = null;
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
		if (_limitHit is null) return String.Empty;

		var sb = new StringBuilder();
		sb.Append($"<span>Mute method trace</span>");

		return sb.ToString();
	}

	private async Task _typeClick(WvTraceMute item)
	{
		await WvBlazorTraceBody.MuteTraceChange(item);
		_selectedTypes = WvBlazorTraceBody.GetTraceMutes();
		await OnChange.InvokeAsync();
		RegenRenderLock();
	}

	private void _initMuteOptions()
	{
		_applicableTypes = new();
		if (_limitHit is not null && _row is not null)
		{
			_applicableTypes = new(){
				new WvTraceMute(WvTraceMuteType.Limit, _row, _limitHit),
				new WvTraceMute(WvTraceMuteType.LimitInModule, _row, _limitHit),
				new WvTraceMute(WvTraceMuteType.LimitInComponent, _row, _limitHit),
				new WvTraceMute(WvTraceMuteType.LimitInComponentInstance, _row, _limitHit),
			};
		}
		_selectedTypes = WvBlazorTraceBody.GetTraceMutes();
	}
}