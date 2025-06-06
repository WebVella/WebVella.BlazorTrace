﻿[![Documentation](https://img.shields.io/badge/Documentation-blue?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/wiki)
[![Dotnet](https://img.shields.io/badge/platform-.NET-blue?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.BlazorTrace?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/stargazers)
[![Nuget version](https://img.shields.io/nuget/v/WebVella.BlazorTrace?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![Nuget download](https://img.shields.io/nuget/dt/WebVella.BlazorTrace?style=for-the-badge)](https://www.nuget.org/packages/WebVella.BlazorTrace)
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/LICENSE)

## What is BlazorTrace?
An easy to add library that will enable you to get detailed information about your Blazor components rerenders and memory, as well as compare it with different snapshots that you created. It is targeting Blazor UI developers and presents the information in a simple and focused way. BlazorTrace will help you build better, faster and more consistent user experience with your Blazor applications.

## About Us
We are a small team of early Blazor adopters that created several complex Blazor applications that work in production. We prefer working with WebAssembly deployments but SSR is getting a favorite fast. We have 15+ experience in creating .net projects. Here is some examples of our work

| | |
|---|---|
| [WebVella ERP](https://github.com/WebVella/WebVella-ERP) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella-ERP?style=for-the-badge)](https://github.com/WebVella/WebVella-ERP/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.ERP?style=for-the-badge)](https://www.nuget.org/packages/WebVella.ERP)
| [Document Templates Library](https://github.com/WebVella/WebVella.DocumentTemplates) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.DocumentTemplates?style=for-the-badge)](https://github.com/WebVella/WebVella.DocumentTemplates/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.DocumentTemplates?style=for-the-badge)](https://www.nuget.org/packages/WebVella.DocumentTemplates)
| [Tefter](https://github.com/WebVella/WebVella.Tefter) | [![GitHub Repo stars](https://img.shields.io/github/stars/WebVella/WebVella.Tefter?style=for-the-badge)](https://github.com/WebVella/WebVella.Tefter/stargazers) | [![Nuget download](https://img.shields.io/nuget/dt/WebVella.Tefter?style=for-the-badge)](https://www.nuget.org/packages/WebVella.Tefter)

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
builder.Services.AddBlazorTrace(new WvBlazorTraceConfiguration()
	{
#if DEBUG
	EnableTracing = true,
#else
	EnableTracing = false,
#endif
	});
#if DEBUG
//Snapshots require bigger hub message size
builder.Services.Configure<HubOptions>(options =>
{
options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
});
//To get the message size error if it got bigger than the above
builder.Services.AddSignalR(o =>
{
 o.EnableDetailedErrors = true;
});
#endif
```

3. Add the BlazorTrace component at the end of your ```App.razor``` or ```Routes.razor``` component

``` razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
</Router>
<WvBlazorTrace/> @* <-- INSERT HERE *@
```
4. Inject the service in your components
You can inject this in the component directly, or in _Imports.razor to make it available in all components.(Thanks LlamaNL :))
``` csharp
[Inject] protected IWvBlazorTraceService WvBlazorTraceService { get; set; }
```
5. Add tracers in your methods (start from LifeCycle methods). They will feed runtime data to the library and reveal the broader picture or where to look into details for issues.
There are several arguments that you can call them with, but here is an example with the only required one (component):

``` csharp
	protected override void OnInitialized()
	{
		WvBlazorTraceService.OnEnter(component: this);
		base.OnInitialized();
		//Do something
		WvBlazorTraceService.OnExit(component: this);
	}
```
if there are many returns in the method you can also do it with try/finally block

``` csharp
	protected override void OnInitialized()
	{
		try
		{
			WvBlazorTraceService.OnEnter(component: this);
			base.OnInitialized();
			//Do something
		}
		finally
		{
			WvBlazorTraceService.OnExit(component: this);
		}
	}
```

6. Add signals in your methods. They are a way to track events in your components or look in details about what and how is going on.
There are several arguments that you can call them with, but here is an example with the only required one (component):

``` csharp
	private void _countTest()
	{
		_counter++;
		WvBlazorTraceService.OnSignal(caller: this, signalName: "counter");
	}
```

7. Thats it. You can start reviewing the data. PRO TIP: Use the F1 (show) and Esc (hide) to save time.

### Method OnEnter/OnExit call information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-methods.png)

### Log signals information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-signals.png)

### Trace calls detail information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-list-modal.png)

### Memory detail information

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/memory-modal.png)

### Limit hits

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/limits-modal.png)

### Snapshots

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-snapshots.png)

### Muted traces

![BlazorTrace modal](https://github.com/WebVella/WebVella.BlazorTrace/blob/main/images/trace-modal-muted.png)

## License
BlazorTrace is distributed under the MIT license.
