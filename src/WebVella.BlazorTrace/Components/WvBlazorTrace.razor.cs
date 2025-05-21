using Microsoft.AspNetCore.Components;
using WebVella.BlazorTrace.Models;

namespace WebVella.BlazorTrace;
public partial class WvBlazorTrace : ComponentBase
{
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;
	[Parameter] public WvButtonPosition Position { get; set; } = WvButtonPosition.RightTop;
	[Parameter] public string ButtonColor { get; set; } = "#be123c";

#if !DEBUG
private bool _visible = false;
#else
	private bool _visible = true;
#endif
	private bool _modalVisible = false;
	private WvTraceModalData? _data = null;
	private string _cssContent = string.Empty;
	private string _buttonStyles = string.Empty;
	private string _buttonClasses = string.Empty;
	protected override void OnInitialized()
	{
		base.OnInitialized();
		_loadCssResource();
		if(!String.IsNullOrWhiteSpace(ButtonColor))
			_buttonStyles = $"background-color:{ButtonColor};";

		_buttonClasses = $" wv-trace-button {Position.ToDescriptionString()} ";
	}

	private async Task _show()
	{
		_modalVisible = true;
		await _getData();
	}

	private async Task _getData()
	{
		_data = null;
		await InvokeAsync(StateHasChanged);
		_data = await WvBlazorTraceService.GetModalData(_data?.Request);
		await InvokeAsync(StateHasChanged);
	}

	private void _hide()
	{
		_modalVisible = false;
	}

	private void _loadCssResource()
	{
		// Get the assembly containing the CSS file
		var assembly = this.GetType().Assembly;
		// Name of the embedded resource (including namespace)
		var resources = assembly.GetManifestResourceNames();
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
}