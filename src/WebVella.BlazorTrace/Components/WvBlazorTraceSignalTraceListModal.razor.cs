using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceSignalTraceListModal : WvBlazorTraceComponentBase, IAsyncDisposable
{
	// INJECTS
	//////////////////////////////////////////////////
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; } = default!;

	// PARAMETERS
	//////////////////////////////////////////////////

	[CascadingParameter(Name = "WvBlazorTraceBody")]
	public WvBlazorTraceBody WvBlazorTraceBody { get; set; } = default!;
	[Parameter] public int NestLevel { get; set; } = 1;

	// LOCAL VARIABLES
	//////////////////////////////////////////////////
	private Guid _componentId = Guid.NewGuid();
	private DotNetObjectReference<WvBlazorTraceSignalTraceListModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvSignalTraceRow? _row = null;
	private WvBlazorTraceLimitInfoSignalModal? _limitInfoModal = null;
	private WvBlazorTraceMuteSignalTraceModal? _traceMuteModal = null;
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
	public async Task Show(WvSignalTraceRow row)
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
		sb.Append($"<span>{_row.SignalName}</span>");

		return sb.ToString();
	}
	private async Task _showLimitInfoModal(WvTraceSessionSignalTrace trace, bool isOnEnter = true)
	{
		if (_limitInfoModal is null) return;
		await _limitInfoModal.Show(trace.Options);
	}

	private async Task _onMute(WvTraceSessionSignalTrace dataField)
	{
		if (dataField is null || _row is null) return;
		if (_traceMuteModal is null) return;
		await _traceMuteModal.Show(_row, dataField);
	}

	private async Task _muteChanged()
	{
		var data = WvBlazorTraceBody.GetData();
		if (data is null || _row is null) return;

		var row = data.SignalTraceRows.FirstOrDefault(x => x.Id == _row.Id);
		if (row is null)
			_row.TraceList = new();
		else
			_row = row;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}
}