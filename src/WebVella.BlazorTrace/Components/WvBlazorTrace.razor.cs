using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Services;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTrace : WvBlazorTraceComponentBase, IAsyncDisposable
{
	// INJECTS
	//////////////////////////////////////////////////
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;
	[Inject] public IWvBlazorTraceConfigurationService WvBlazorTraceConfigurationService { get; set; } = default!;
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; } = default!;

	// PARAMS
	//////////////////////////////////////////////////
	[Parameter] public WvButtonPosition Position { get; set; } = WvButtonPosition.RightTop;
	[Parameter] public string ButtonColor { get; set; } = "#be123c";

	// LOCAL VARIABLES
	//////////////////////////////////////////////////
#if !DEBUG
private bool _visible = false;
#else
	private bool _visible = true;
#endif
	private Guid _componentId = new Guid("08e73835-f485-453f-ad9f-7c462cbbe70c");
	private DotNetObjectReference<WvBlazorTrace> _objectRef = default!;
	private bool _modalVisible = false;
	private bool _loadingData = false;
	private WvTraceModalData? _data = null;
	private string _jsContent = string.Empty;
	private string _cssContent = string.Empty;
	private string _buttonStyles = string.Empty;
	private string _buttonClasses = string.Empty;
	private int _infiniteLoopDelaySeconds = 1;
	private Task? _infiniteLoop;
	private CancellationTokenSource? _infiniteLoopCancellationTokenSource;
	private WvBlazorTraceConfiguration _configuration = default!;
	private WvTraceRow? _traceListModalRow = null;
	private WvTraceRow? _memoryModalRow = null;
	//LIFECYCLE
	//////////////////////////////////////////////////
	public async ValueTask DisposeAsync()
	{
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape");
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("F1");
		_objectRef?.Dispose();
		if (_infiniteLoopCancellationTokenSource is not null)
			_infiniteLoopCancellationTokenSource.Cancel();
	}
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_objectRef = DotNetObjectReference.Create(this);
		_loadResource();
		if (!String.IsNullOrWhiteSpace(ButtonColor))
			_buttonStyles = $"background-color:{ButtonColor};";
		_buttonClasses = $" wv-trace-button {Position.ToDescriptionString()} ";
		_configuration = WvBlazorTraceConfigurationService.GetConfiguraion();
		EnableRenderLock();
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (_configuration.EnableF1Shortcut)
				await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "F1");

#if DEBUG
			await _show();
			_data!.Request.IsAutoRefresh = true;
			await InvokeAsync(StateHasChanged);
#endif
		}
	}

	// UI HANDLERS
	////////////////////////////////////////////////
	private async Task _show()
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape");
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("F1");
		_modalVisible = true;
		await _getData();
		_runLoop();
		RegenRenderLock();
	}
	private async Task _hide()
	{
		if (_configuration.EnableF1Shortcut)
			await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "F1");
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape");
		if (_infiniteLoopCancellationTokenSource is not null)
			_infiniteLoopCancellationTokenSource.Cancel();
		_modalVisible = false;
		RegenRenderLock();
	}

	private void _enableAutoReload()
	{
		_data!.Request.IsAutoRefresh = true;
		RegenRenderLock();
	}

	private void _disableAutoReload()
	{
		_data!.Request.IsAutoRefresh = false;
		RegenRenderLock();
	}

	[JSInvokable("OnShortcutKey")]
	public async Task OnShortcutKey(string code)
	{
		if (code == "Escape")
		{
			if (_traceListModalRow is not null)
			{
				_traceListModalRow = null;
				RegenRenderLock();
			}
			if (_memoryModalRow is not null)
			{
				_memoryModalRow = null;
				RegenRenderLock();
			}
			else
				await _hide();

			await InvokeAsync(StateHasChanged);
		}
		else if (code == "F1")
		{
			await _show();
			await InvokeAsync(StateHasChanged);
		}

	}

	private async Task _submitFilter()
	{
		RegenRenderLock();
	}
	private async Task _clearFilter()
	{
		if (_data is null) return;
		_data.Request.ModuleFilter = null;
		_data.Request.ComponentFilter = null;
		_data.Request.MethodFilter = null;
		_data.Request.MemoryFilter = null;
		_data.Request.DurationFilter = null;
		_data.Request.CallsFilter = null;
		_data.Request.LimitsFilter = null;
		RegenRenderLock();
		await _submitFilter();
	}

	// LOGIC
	////////////////////////////////////////////////
	private async Task _getData(bool fromLoop = false)
	{
		if (_loadingData) return;
		_loadingData = true;
		try
		{
			if (_data is null || !fromLoop)
			{
				_data = await WvBlazorTraceService.GetModalData(_data?.Request);
			}
			else
			{
				var data = await WvBlazorTraceService.GetModalData(_data?.Request);
				_data!.TraceRows = data.TraceRows;
			}
			_loadingData = false;
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
	}
	private void _loadResource()
	{
		// Get the assembly containing the CSS file
		var assembly = this.GetType().Assembly;
		// Name of the embedded resource (including namespace)
		var resources = assembly.GetManifestResourceNames();
		{
			string? resourceName = resources.FirstOrDefault(x => x.Contains("styles.css"));
			if (String.IsNullOrEmpty(resourceName))
				throw new Exception("BlazorTrace styles not found as resources");
			// Read the stream from the embedded resource
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream != null)
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						_cssContent = reader.ReadToEnd();
					}
				}
			}
		}
		{
			string? resourceName = resources.FirstOrDefault(x => x.Contains("scripts.js"));
			if (String.IsNullOrEmpty(resourceName))
				throw new Exception("BlazorTrace scripts not found as resources");
			// Read the stream from the embedded resource
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream != null)
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						_jsContent = reader.ReadToEnd();
					}
				}
			}
		}
	}
	private void _runLoop()
	{
		_infiniteLoopCancellationTokenSource = new CancellationTokenSource();
		_infiniteLoop = Task.Run(async () =>
		{
			while (!_infiniteLoopCancellationTokenSource.IsCancellationRequested)
			{
				await Task.Delay(_infiniteLoopDelaySeconds * 1000);
				if (_infiniteLoopCancellationTokenSource.IsCancellationRequested)
					return;
				if (!_modalVisible || !_data!.Request.IsAutoRefresh)
					continue;
				await _getData(fromLoop: true);
			}
		}, _infiniteLoopCancellationTokenSource.Token);
	}
	private bool _hasFilter()
	{
		if (_data is null) return false;
		return !String.IsNullOrWhiteSpace(_data.Request.ModuleFilter)
		|| !String.IsNullOrWhiteSpace(_data.Request.ComponentFilter)
		|| !String.IsNullOrWhiteSpace(_data.Request.MethodFilter)
		|| _data.Request.MemoryFilter is not null
		|| _data.Request.DurationFilter is not null
		|| _data.Request.CallsFilter is not null;
	}

	private void _showTraceListModal(WvTraceRow row)
	{
		_traceListModalRow = row;
		RegenRenderLock();
	}

	private void _hideTraceListModal()
	{
		_traceListModalRow = null;
		RegenRenderLock();
	}

	private void _showMemoryModal(WvTraceRow row)
	{
		_memoryModalRow = row;
		RegenRenderLock();
	}

	private void _hideMemoryModal()
	{
		_memoryModalRow = null;
		RegenRenderLock();
	}

}