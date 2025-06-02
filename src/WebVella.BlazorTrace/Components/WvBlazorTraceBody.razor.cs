using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using WebVella.BlazorTrace.Models;
using WebVella.BlazorTrace.Services;
using WebVella.BlazorTrace.Utility;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTraceBody : WvBlazorTraceComponentBase, IAsyncDisposable
{
	// INJECTS
	//////////////////////////////////////////////////
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;
	[Inject] public IWvBlazorTraceConfigurationService WvBlazorTraceConfigurationService { get; set; } = default!;
	[Inject] protected IJSRuntime JSRuntimeSrv { get; set; } = default!;

	// PARAMS
	//////////////////////////////////////////////////
	/// <summary>
	/// the 'heart button' position on the screen
	/// </summary>
	[Parameter] public WvButtonPosition Position { get; set; } = WvButtonPosition.RightTop;
	/// <summary>
	/// the background color of the 'heart button'
	/// </summary>
	[Parameter] public string ButtonColor { get; set; } = "#be123c";

	// LOCAL VARIABLES
	//////////////////////////////////////////////////

	private Guid _componentId = Guid.NewGuid();
	private DotNetObjectReference<WvBlazorTraceBody> _objectRef = default!;
	private bool _f1ListenerEnabled = false;
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
	private WvBlazorTraceListModal? _traceListModal = null;
	private WvBlazorTraceSignalTraceListModal? _signalTraceListModal = null;
	private WvBlazorTraceMemoryModal? _memoryModal = null;
	private WvBlazorTraceLimitModal? _limitModal = null;
	private WvBlazorTraceSignalLimitModal? _signalLimitModal = null;
	private WvBlazorTraceMuteMethodModal? _traceMuteModal = null;
	private WvBlazorTraceMuteSignalModal? _signalMuteModal = null;
	private string? _primarySnHighlightClass = null;

	private List<WvTraceModalMenuItem> _methodMenu = new();
	private List<WvTraceModalMenuItem> _signalMenu = new();
	private List<WvTraceModalMenuItem> _asideMenu = new();
	private string _helpLink = "https://github.com/WebVella/WebVella.BlazorTrace/wiki/Methods-Modal";

	private WvSnapshotSavingState _savingState = WvSnapshotSavingState.NotSaving;

	private List<WvTraceMute> _currentMutes = new();

	//LIFECYCLE
	//////////////////////////////////////////////////
	public async ValueTask DisposeAsync()
	{
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape", _componentId.ToString());
		if (_f1ListenerEnabled)
			await new JsService(JSRuntimeSrv).RemoveKeyEventListener("F1");
		_objectRef?.Dispose();
		if (_infiniteLoopCancellationTokenSource is not null)
			_infiniteLoopCancellationTokenSource.Cancel();
	}
	protected override async Task OnInitializedAsync()
	{
		base.OnInitialized();
		_configuration = WvBlazorTraceConfigurationService.GetConfiguraion();
		_objectRef = DotNetObjectReference.Create(this);
		_loadResource();
		if (!String.IsNullOrWhiteSpace(ButtonColor))
			_buttonStyles = $"background-color:{ButtonColor};";
		_buttonClasses = $" wv-trace-button {Position.WvBTToDescriptionString()} ";
		_currentMutes = await WvBlazorTraceService.GetAllTraceMutesAsync();
		_initMenu();
		EnableRenderLock();
	}
	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		if (firstRender)
		{
			if (_configuration.EnableF1Shortcut)
			{
				await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "F1");
				_f1ListenerEnabled = true;
			}
		}
	}

	// UI HANDLERS
	////////////////////////////////////////////////
	private async Task _show()
	{
		await new JsService(JSRuntimeSrv).AddKeyEventListener(_objectRef, "OnShortcutKey", "Escape", _componentId.ToString());
		_modalVisible = true;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
		await _getData();
		_runLoop();
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);

	}
	private async Task _hide()
	{
		await new JsService(JSRuntimeSrv).RemoveKeyEventListener("Escape", _componentId.ToString());
		if (_infiniteLoopCancellationTokenSource is not null)
			_infiniteLoopCancellationTokenSource.Cancel();
		_modalVisible = false;
		_loadingData = false;
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
			await _hide();
			await InvokeAsync(StateHasChanged);
		}
		else if (code == "F1")
		{
			if (_modalVisible) return;
			await _show();
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _submitFilter()
	{
		RegenRenderLock();
		await _getData();
	}
	private async Task _clearFilter(Type filterType)
	{
		if (_data is null) return;
		if(filterType == typeof(WvTraceModalRequestMethodsFilter)){ 
			_data.Request.MethodsFilter = new();
		}
		else if(filterType == typeof(WvTraceModalRequestSignalsFilter)){ 
			_data.Request.SignalsFilter = new();
		}
		else if(filterType == typeof(WvTraceModalRequestMutedFilter)){ 
			_data.Request.MutedFilter = new();
		}
		RegenRenderLock();
		await _submitFilter();
	}

	private async Task _menuClick(WvTraceModalMenuItem item)
	{
		if (_data?.Request is null) return;
		_data.Request.Menu = item.Id;
		await _getData();
	}

	// LOGIC
	////////////////////////////////////////////////

	private void _initMenu()
	{
		_methodMenu = new(){
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.MethodCalls},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.MethodMemory},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.MethodDuration},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.MethodLimits},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.MethodName},
		};
		_signalMenu = new(){
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.SignalCalls},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.SignalLimits},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.SignalName}
		};
		_asideMenu = new(){
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.TraceMutes, Counter = _currentMutes.Count},
			new WvTraceModalMenuItem{ Id = WvTraceModalMenu.Snapshots},
		};


		foreach (var item in _methodMenu)
			item.OnClick = async () => await _menuClick(item);

		foreach (var item in _signalMenu)
			item.OnClick = async () => await _menuClick(item);

		foreach (var item in _asideMenu)
			item.OnClick = async () => await _menuClick(item);
	}
	private async Task _getData(bool fromLoop = false)
	{
		if (_loadingData) return;
		if (!fromLoop)
		{
			_loadingData = true;
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
		try
		{
			if (!fromLoop)
			{
				_data = await WvBlazorTraceService.GetModalDataAsync(_data?.Request);
				_initSnapshotActions();
			}
			else
			{
				var data = await WvBlazorTraceService.GetModalDataAsync(_data?.Request);
				_data!.MethodTraceRows = data.MethodTraceRows;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally
		{
			if (!fromLoop)
				_loadingData = false;
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
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
			using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
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
			using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
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
	private async Task _showTraceListModal(object row)
	{
		if (row == null) return;
		if (row is WvMethodTraceRow)
		{
			if (_traceListModal is null) return;
			await _traceListModal.Show((WvMethodTraceRow)row);
		}
		else if (row is WvSignalTraceRow)
		{
			if (_signalTraceListModal is null) return;
			await _signalTraceListModal.Show((WvSignalTraceRow)row);
		}
	}
	private async Task _showMemoryModal(WvMethodTraceRow row)
	{
		if (_memoryModal is null) return;
		await _memoryModal.Show(row);
	}
	private async Task _showLimitModal(object row)
	{
		if (row == null) return;
		if (row is WvMethodTraceRow)
		{
			if (_limitModal is null) return;
			await _limitModal.Show((WvMethodTraceRow)row);
		}
		else if (row is WvSignalTraceRow)
		{
			if (_signalLimitModal is null) return;
			await _signalLimitModal.Show((WvSignalTraceRow)row);
		}
	}

	private async Task _showTraceMuteModal(object row)
	{
		if (row == null) return;
		if (row is WvMethodTraceRow)
		{
			if (_traceMuteModal is null) return;
			await _traceMuteModal.Show((WvMethodTraceRow)row);
		}
		else if (row is WvSignalTraceRow)
		{
			if (_signalMuteModal is null) return;
			await _signalMuteModal.Show((WvSignalTraceRow)row);
		}
	}

	private async Task _bookmarkClicked(object rowObject)
	{
		if (rowObject is WvMethodTraceRow)
		{
			var row = (WvMethodTraceRow)rowObject;
			if (row.IsBookmarked)
			{
				await WvBlazorTraceService.RemoveBookmarkAsync(row.Id);
				row.IsBookmarked = false;
			}
			else
			{
				await WvBlazorTraceService.AddBookmarkAsync(row.Id);
				row.IsBookmarked = true;
			}
		}
		else if (rowObject is WvSignalTraceRow)
		{
			var row = (WvSignalTraceRow)rowObject;
			if (row.IsBookmarked)
			{
				await WvBlazorTraceService.RemoveBookmarkAsync(row.Id);
				row.IsBookmarked = false;
			}
			else
			{
				await WvBlazorTraceService.AddBookmarkAsync(row.Id);
				row.IsBookmarked = true;
			}
		}

		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
	}
	private async Task _saveSnapshot()
	{
		if (_savingState != WvSnapshotSavingState.NotSaving) return;
		_savingState = WvSnapshotSavingState.Saving;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
		await Task.Delay(1);
		try
		{
			var snapshot = await WvBlazorTraceService.CreateSnapshotAsync();
			_data!.SnapshotList.Add(new WvSnapshotListItem
			{
				Id = snapshot.Id,
				Name = snapshot.Name,
				CreatedOn = snapshot.CreatedOn,

			});
			_data!.SnapshotList = _data!.SnapshotList.OrderBy(x => x.Name).ToList();
			_initSnapshotActions();
			if (_data.Request.PrimarySnapshotId is null)
			{
				_data.Request.PrimarySnapshotId = snapshot.Id;
				await _highlightPrimarySnapshot();
				await _getData(true);
			}

			_savingState = WvSnapshotSavingState.Saved;
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
			await Task.Delay(1);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally
		{
			_savingState = WvSnapshotSavingState.NotSaving;
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _clearCurrent()
	{
		try
		{
			WvBlazorTraceService.ClearCurrentSession();
			await _getData();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally
		{
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
	}
	private async Task _highlightPrimarySnapshot()
	{
		_primarySnHighlightClass = "wv-highlight";
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);
		await Task.Delay(500);
		_primarySnHighlightClass = null;
		RegenRenderLock();
		await InvokeAsync(StateHasChanged);

	}

	private void _initSnapshotActions()
	{
		if (_data is null || _data.SnapshotList is null) return;
		foreach (var item in _data.SnapshotList)
		{
			item.OnRemove = async () => await _removeSnapshot(item);
			item.OnRename = async () => await _renameSnapshot(item);
		}
	}

	private async Task _removeSnapshot(WvSnapshotListItem sn)
	{
		if (!await JSRuntimeSrv.InvokeAsync<bool>("confirm", "Are you sure that you need this snapshot removed?"))
			return;
		try
		{
			await WvBlazorTraceService.RemoveSnapshotAsync(sn.Id);
			//check update list
			//if the removed is selected in primary or secondary in request
			//set to null
			_data!.SnapshotList = _data!.SnapshotList.Where(x => x.Id != sn.Id).ToList();
			if (_data.Request.PrimarySnapshotId == sn.Id)
				_data.Request.PrimarySnapshotId = null;
			if (_data.Request.SecondarySnapshotId == sn.Id)
				_data.Request.SecondarySnapshotId = null;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally
		{
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _renameSnapshot(WvSnapshotListItem sn)
	{

		try
		{
			var snapshot = await WvBlazorTraceService.RenameSnapshotAsync(sn.Id, sn.Name);
			//Check an update list with the new name
			var snIndex = _data!.SnapshotList.FindIndex(x => x.Id != sn.Id);
			if (snIndex > -1)
			{
				_data!.SnapshotList[snIndex].Name = snapshot.Name;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString());
		}
		finally
		{
			RegenRenderLock();
			await InvokeAsync(StateHasChanged);
		}
	}

	private async Task _muteTraceChange(WvTraceMute item)
	{
		if (item is null) return;
		await WvBlazorTraceService.ToggleTraceMuteAsync(item);
		_currentMutes = await WvBlazorTraceService.GetAllTraceMutesAsync();
		await _getData();
		_initMenu();
		RegenRenderLock();
		//await InvokeAsync(StateHasChanged);
	}
}