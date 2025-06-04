using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceLimitModal : WvBlazorTraceComponentBase
{
	// INJECTS
	//////////////////////////////////////////////////
	[CascadingParameter(Name = "WvBlazorTraceBody")]
	public WvBlazorTraceBody WvBlazorTraceBody { get; set; } = default!;
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; } = default!;

	// PARAMETERS
	//////////////////////////////////////////////////
	[Parameter] public int NestLevel { get; set; } = 1;

	// LOCAL VARIABLES
	//////////////////////////////////////////////////
	private Guid _componentId = Guid.NewGuid();
	private DotNetObjectReference<WvBlazorTraceLimitModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvMethodTraceRow? _row = null;
	private WvBlazorTraceMuteLimitModal? _limitMuteModal = null;

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

	// PUBLIC
	//////////////////////////////////////////////////
	public async Task Show(WvMethodTraceRow row)
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_escapeListenerEnabled = true;
		_row = row;
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
		sb.Append($"<span>{_row.Component}</span>");
		if (!String.IsNullOrWhiteSpace(_row.InstanceTag))
		{
			sb.Append($" <span class='wv-tag' style='margin-left:5px'>{_row.InstanceTag}</span>");
		}
		sb.Append("<span class='wv-trace-modal__divider'></span>");
		sb.Append($"<span>{_row.Method}</span>");

		return sb.ToString();
	}

	private async Task _onMute(WvTraceSessionLimitHit dataField)
	{
		if (dataField is null || _row is null) return;
		if (_limitMuteModal is null) return;
		await _limitMuteModal.Show(_row, dataField);
	}

	private async Task _muteChanged()
	{
		var data = WvBlazorTraceBody.GetData();
		if (data is null || _row is null) return;

		var row = data.MethodTraceRows.FirstOrDefault(x => x.Id == _row.Id);
		if (row is null)
		{
			_row.LimitHits = new();
		}
		else
			_row = row;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}

}