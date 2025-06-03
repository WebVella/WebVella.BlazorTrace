using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Services;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceMemoryModal : WvBlazorTraceComponentBase
{
	// INJECTS
	//////////////////////////////////////////////////
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; } = default!;
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;

	// PARAMETERS
	//////////////////////////////////////////////////
	[CascadingParameter(Name = "WvBlazorTraceBody")]
	public WvBlazorTraceBody WvBlazorTraceBody { get; set; } = default!;
	[Parameter] public int NestLevel { get; set; } = 1;

	// LOCAL VARIABLES
	//////////////////////////////////////////////////
	private Guid _componentId = Guid.NewGuid();
	private DotNetObjectReference<WvBlazorTraceMemoryModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvMethodTraceRow? _row = null;
	private Guid? _traceId = null;
	private bool _isOnEnter = true;
	private List<WvSnapshotMemoryComparisonDataField> _items = new();
	private WvBlazorTraceMuteMemoryModal? _traceMuteModal = null;

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
	public async Task Show(WvMethodTraceRow row, Guid? traceId = null, bool isOnEnter = true)
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_escapeListenerEnabled = true;
		_initData(row, traceId, isOnEnter);
		_modalVisible = true;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}
	public async Task Hide(bool invokeStateChanged = true)
	{
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape", _componentId.ToString());
		_escapeListenerEnabled = false;
		_row = null;
		_items = new();
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

	private async Task _onMute(WvSnapshotMemoryComparisonDataField row)
	{
		if (row == null) return;
		if (_traceMuteModal is null) return;
		await _traceMuteModal.Show(row);
	}

	private async Task _muteChanged()
	{
		var data = WvBlazorTraceBody.GetData();
		if (data is null || _row is null) return;

		var row = data.MethodTraceRows.FirstOrDefault(x => x.Id == _row.Id);
		if (row is null) return;
		_initData(row, _traceId, _isOnEnter);
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}

	private void _initData(WvMethodTraceRow row, Guid? traceId, bool isOnEnter = true){ 
		_row = row;
		_items = new();
		_traceId = null;
		_isOnEnter = true;
		if(_row is null) return;

		if (traceId is not null)
		{
			var trace = _row.TraceList.FirstOrDefault(x => x.TraceId == traceId);
			if (trace is not null && isOnEnter)
			{
				_traceId = traceId;
				_isOnEnter = isOnEnter;
				_items = trace.OnEnterMemoryInfo.ToMemoryDataFields();
			}
			else if (trace is not null && !isOnEnter)
			{
				_traceId = traceId;
				_isOnEnter = isOnEnter;
				_items = trace.OnExitMemoryInfo.ToMemoryDataFields();
			}
		}
		else
			_items = _row.MemoryComparison.Fields;	
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


}