using Microsoft.AspNetCore.Components;

namespace WebVella.BlazorTrace.Site.Components;

[WvBlazorTrace]
public partial class Test1 : ComponentBase
{
	[Parameter] public string? InstanceTag { get; set; }
	private int _counter = 0;
	[Inject] public IWvBlazorTraceService WvBlazorTraceService { get; set; } = default!;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		base.OnAfterRender(firstRender);
		await Task.Delay(50);
	}

	private void _countTest1()
	{
		WvBlazorTraceService.OnEnter(component: this,instanceTag: InstanceTag);
		_counter++;
		WvBlazorTraceService.OnSignal(caller: this, signalName: "counter", instanceTag: InstanceTag,
			customData: $"counter:{_counter}",
			options: new Models.WvTraceSignalOptions
			{
				CallLimit = 0
			});
		WvBlazorTraceService.OnExit(component: this,instanceTag: InstanceTag);
	}
}