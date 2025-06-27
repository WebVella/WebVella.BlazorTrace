using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebVella.BlazorTrace;

namespace WebVella.BlazorTrace.Site;
public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//		builder.Services.AddBlazorTrace(new WvBlazorTraceConfiguration()
//		{
//#if DEBUG
//			EnableTracing = true,
//			AutoShowModal = true,
//#else
//			EnableTracing = false,
//#endif
//			EnableF1Shortcut = true,
//			MemoryIncludeAssemblyList = new(){
//				"WebVella.BlazorTrace.Site"
//			}
//		});

		await builder.Build().RunAsync();
	}
}

