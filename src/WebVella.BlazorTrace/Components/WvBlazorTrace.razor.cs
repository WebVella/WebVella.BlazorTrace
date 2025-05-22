using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Services;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTrace : ComponentBase, IAsyncDisposable
{
	// INJECTS
	//////////////////////////////////////////////////
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;
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
	private bool _isAutorefresh = false;
	private WvTraceModalData? _data = null;
	private string _jsContent = string.Empty;
	private string _cssContent = string.Empty;
	private string _buttonStyles = string.Empty;
	private string _buttonClasses = string.Empty;
	private int _infiniteLoopDelaySeconds = 1;
	private Task _infiniteLoop;
	private CancellationTokenSource _infiniteLoopCancellationTokenSource;
	public Guid _currentRenderLock { get; private set; } = Guid.Empty;
	public Guid _oldRenderLock { get; private set; } = Guid.Empty;

	//LIFECYCLE
	//////////////////////////////////////////////////
	public async ValueTask DisposeAsync()
	{
		WvBlazorTraceService.OnEnter(component: this);
		await new JsService(JSRuntimeSrv).RemoveEscapeKeyEventListener(_componentId.ToString());
		_objectRef?.Dispose();
		if (_infiniteLoopCancellationTokenSource is not null)
			_infiniteLoopCancellationTokenSource.Cancel();
		WvBlazorTraceService.OnExit(component: this);
	}
	protected override void OnInitialized()
	{
		WvBlazorTraceService.OnEnter(component: this);
		base.OnInitialized();
		_objectRef = DotNetObjectReference.Create(this);
		_loadResource();
		if (!String.IsNullOrWhiteSpace(ButtonColor))
			_buttonStyles = $"background-color:{ButtonColor};";

		_buttonClasses = $" wv-trace-button {Position.ToDescriptionString()} ";
		WvBlazorTraceService.OnExit(component: this);
	}
	protected override void OnAfterRender(bool firstRender)
	{ 
		WvBlazorTraceService.OnEnter(component: this);
		base.OnAfterRender(firstRender);
		WvBlazorTraceService.OnExit(component: this);
	}
	protected override void OnParametersSet()
	{
		WvBlazorTraceService.OnEnter(component: this);
		base.OnParametersSet();
		WvBlazorTraceService.OnExit(component: this);
	}

	protected override bool ShouldRender()
	{
		if (_currentRenderLock == _oldRenderLock)
			return false;
		_oldRenderLock = _currentRenderLock;
		return true;
	}

	// UI HANDLERS
	////////////////////////////////////////////////
	private async Task _show()
	{
		await new JsService(JSRuntimeSrv).AddEscapeKeyEventListener(_objectRef, _componentId.ToString(), "OnEscapeKey");
		_modalVisible = true;
		await _getData();
		_runLoop();
		_regenRenderLock();
	}
	private async Task _hide()
	{
		await new JsService(JSRuntimeSrv).RemoveEscapeKeyEventListener(_componentId.ToString());
		_modalVisible = false;
		_regenRenderLock();
	}

	private void _enableAutoReload()
	{
		_isAutorefresh = true;
		_regenRenderLock();
	}

	private void _disableAutoReload()
	{
		_isAutorefresh = false;
		_regenRenderLock();
	}


	[JSInvokable("OnEscapeKey")]
	public async Task OnEscapeKey()
	{
		await _hide();
		await InvokeAsync(StateHasChanged);
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
				_isAutorefresh = _data?.Request?.IsAutoRefresh ?? false;
			}
			else
			{
				var data = await WvBlazorTraceService.GetModalData(_data?.Request);
				_data!.TraceRows = data.TraceRows;
			}
			_loadingData = false;
			_regenRenderLock();
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
				if (!_modalVisible || !_isAutorefresh)
					continue;
				await _getData(fromLoop: true);
			}
		}, _infiniteLoopCancellationTokenSource.Token);
	}
	private void _regenRenderLock()
	{
		_currentRenderLock = Guid.NewGuid();
	}

}