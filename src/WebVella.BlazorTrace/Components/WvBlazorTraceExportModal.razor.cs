using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceExportModal : WvBlazorTraceComponentBase
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
	private DotNetObjectReference<WvBlazorTraceExportModal> _objectRef = default!;
	private bool _escapeListenerEnabled = false;
	private bool _modalVisible = false;
	private WvTraceModalData? _data = null;
	private string? _csv = null;
	private string? _json = null;
	private string? _content = null;
	private WvBlazorTraceExportModalMode _mode = WvBlazorTraceExportModalMode.Csv;
	private string _copyBtnText = "Copy";
	private string _copyBtnClass = "wv-mute";

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
	public async Task Show(WvTraceModalData data)
	{
		if (data is null) return;
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_escapeListenerEnabled = true;
		_initData(data);
		_modalVisible = true;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}
	public async Task Hide(bool invokeStateChanged = true)
	{
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape", _componentId.ToString());
		_escapeListenerEnabled = false;
		_modalVisible = false;
		_data = null;
		_content = null;
		_json = null;
		_csv = null;
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


	private void _initData(WvTraceModalData data)
	{
		_json = null;
		_csv = null;
		_content = null;
		_data = null;
		if (data.MethodTraceRows is not null && data.MethodTraceRows.Count > 0)
		{
			_csv = WvTraceUtility.ExportToCsv(data.MethodTraceRows);
			_json = JsonSerializer.Serialize(data.MethodTraceRows, new JsonSerializerOptions { WriteIndented = true });
		}
		else if (data.SignalTraceRows is not null && data.SignalTraceRows.Count > 0)
		{
			_csv = WvTraceUtility.ExportToCsv(data.SignalTraceRows);
			_json = JsonSerializer.Serialize(data.SignalTraceRows, new JsonSerializerOptions { WriteIndented = true });
		}

		if (_mode == WvBlazorTraceExportModalMode.Csv)
			_content = _csv;

		else if (_mode == WvBlazorTraceExportModalMode.Json)
			_content = _json;
	}

	private void _setMode(WvBlazorTraceExportModalMode mode)
	{
		if (_mode == mode) return;
		_mode = mode;
		if (mode == WvBlazorTraceExportModalMode.Csv)
			_content = _csv;
		else if (mode == WvBlazorTraceExportModalMode.Json)
			_content = _json;
		RegenRenderLock();
	}

	private async Task _copyClipboard()
	{
		await JSRuntimeSrv.InvokeVoidAsync("WebVellaBlazorTrace.copyToClipboard", _content);
		_copyBtnText = "Copied!";
		_copyBtnClass = "wv-cyan";
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
		await Task.Delay(2000);
		_copyBtnText = "Copy";
		_copyBtnClass = "wv-mute";
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}
}

public enum WvBlazorTraceExportModalMode
{
	Csv = 0,
	Json = 1
}