using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebVella.BlazorTrace;

//how to add fody support
//1. add module with name ot attribute
//2. add in project file nuget ref -> <PackageReference Include="MethodDecorator.Fody" Version="1.1.1" />
//3. add file FodyWeavers.xml

[module: WvBlazorTrace] //This is important to be before the namespace declaration
namespace WebVella.BlazorTrace.Site;
public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
		builder.Services.AddBlazorTrace(new WvBlazorTraceConfiguration()
		{
#if DEBUG
			EnableTracing = true,
			AutoShowModal = true,
#else
			EnableTracing = false,
#endif
			EnableF1Shortcut = true,
			MemoryIncludeAssemblyList = new(){
				"WebVella.BlazorTrace.Site"
			}
		});

		await builder.Build().RunAsync();
	}
}

