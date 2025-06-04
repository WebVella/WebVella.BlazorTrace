using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceMuteTraceModal : WvBlazorTraceComponentBase
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
	private DotNetObjectReference<WvBlazorTraceMuteTraceModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvMethodTraceRow? _row = null;
	private WvTraceSessionMethodTrace? _methodTrace = null;
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
	public async Task Show(WvMethodTraceRow row, WvTraceSessionMethodTrace dataField)
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_escapeListenerEnabled = true;
		_row = row;
		_methodTrace = dataField;
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
		_methodTrace = null;
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
		if (_methodTrace is null) return String.Empty;

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
		if (_methodTrace is not null && _row is not null)
		{
			_applicableTypes = new();
			if (!String.IsNullOrWhiteSpace(_methodTrace.OnEnterCustomData))
			{
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnEnterCustomData, _row, _methodTrace.OnEnterCustomData));
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnEnterCustomDataInModule, _row, _methodTrace.OnEnterCustomData));
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnEnterCustomDataInComponent, _row, _methodTrace.OnEnterCustomData));
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnEnterCustomDataInComponentInstance, _row, _methodTrace.OnEnterCustomData));
			}
			if (!String.IsNullOrWhiteSpace(_methodTrace.OnExitCustomData))
			{
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnExitCustomData, _row, _methodTrace.OnExitCustomData));
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnExitCustomDataInModule, _row, _methodTrace.OnExitCustomData));
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnExitCustomDataInComponent, _row, _methodTrace.OnExitCustomData));
				_applicableTypes.Add(new WvTraceMute(WvTraceMuteType.OnExitCustomDataInComponentInstance, _row, _methodTrace.OnExitCustomData));
			}

		}
		_selectedTypes = WvBlazorTraceBody.GetTraceMutes();
	}
}