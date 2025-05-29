[![Documentation](https://img.shields.io/badge/Homepage-blue?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/wiki)
[![Dotnet](https://img.shields.io/badge/platform-.NET-blue?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.BlazorTrace?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/WebVella.BlazorTrace?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![Nuget download](https://img.shields.io/nuget/dt/WebVella.BlazorTrace?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![WebVella Document Templates License](https://img.shields.io/badge/LICENSE%20MIT?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/LICENSE)

## What is BlazorTrace?
An easy to add library that will enable you to get detailed information about your Blazor components rerenders and memory, as well as compare it with different snapshots that you created. It is targeting Blazor UI developers and presents the information in a simple and focused way. BlazorTrace will help you build better, faster and more consistent user experience with your Blazor applications.

## How to get it
You can either clone this repository or get the [Nuget package](https://www.nuget.org/packages/WebVella.BlazorTrace)

## Please help by giving a star
GitHub stars guide developers toward great tools. If you find this project valuable, please give it a star – it helps the community and takes just a second!⭐

## Documentation
You can find our documentation in the [Wiki section of this repository](https://github.com/WebVella/WebVella.BlazorTrace/wiki)

## Get Started
To start using BlazorTrace you need to do the following simple steps:

1. Install the latest version of the [WebVella.BlazorTrace Nuget package](https://www.nuget.org/packages/WebVella.BlazorTrace)
2. Add the following line in your ```Program.cs``` file. You can get more info about options to fine tune it in the wiki.
``` csharp
builder.Services.AddBlazorTrace();
```
3. Add the BlazorTrace component at the end of your ```App.razor`` component
``` csharp
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
</Router>
<WvBlazorTrace/>
```
4. Add tracers in your methods. They will feed runtime data to the library. There are several arguments that you can call them with, but here is an example with the only required one (component):
``` csharp
	protected override void OnInitialized()
	{
		WvBlazorTraceService.OnEnter(component: this);
		base.OnInitialized();
		//Do something
		WvBlazorTraceService.OnExit(component: this);
	}
```
5. Add signals in your methods. They are a way to track events in your components. There are several arguments that you can call them with, but here is an example with the only required one (component):
``` csharp
	private void _countTest()
	{
		_counter++;
		WvBlazorTraceService.OnSignal(caller: this, signalName: "counter");
	}
```
6. Thats it. You can start reviewing the data by pressing the "F1" key or click on the red button on the top right corner of your screen.

![BlazorTrace modal](/images/trace-modal-methods.png)

## License
BlazorTrace is distributed under the MIT license.